

using Google.Protobuf;
using System.Diagnostics.Contracts;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace LLMSharp.TokenizerMap.Builder
{
    /// <summary>
    /// Tokenizer Map Factory class constructs tokenizer maps for various LLMs
    /// </summary>
    internal class TokenizerMapFactory
    {
        /// <summary>
        /// Construct TokenizerMaps by parsing a remote json file. Will return null if the url is invalid or json in the file is invalid.
        /// Expects a JsonFile with 'bpe_ranks' , 'pat_str' properties and an optional 'special_tokens' property.
        /// </summary>
        /// <param name="remoteJsonFileUrl">Url for the remote json file</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Constructed tokenizer maps. null if the json file is not valid</returns>
        internal async Task<TokenizerMaps?> ConstructFromRemoteJsonFileAsync(Uri remoteJsonFileUrl, CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                var jObj = await httpClient.GetFromJsonAsync<TokenizerRegistry>(remoteJsonFileUrl, cancellationToken);
                if (jObj == null)
                {
                    Console.Error.WriteLine($"Invalid Json found @{remoteJsonFileUrl}. Terminating tokenizer maps construction");
                    return null;
                }

                return ConstructFromRegistry(jObj, cancellationToken);
            }
            catch(OperationCanceledException) 
            {
                Console.WriteLine("Operation cancelled. Terminating TokenizerMaps construction");
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unknown error: {ex}");
                return null;
            }
        }

        /// <summary>
        /// Construct TokenizerMaps by parsing a JsonFile. Will return null if the JsonFile has invalid json.
        /// Expects a JsonFile with 'bpe_ranks' , 'pat_str' properties and an optional 'special_tokens' property.
        /// </summary>
        /// <param name="jsonFilePath">JsonFile location for constructing tokenizer maps</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Constructed tokenizer maps. null if the json file is not valid</returns>
        internal async Task<TokenizerMaps?> ConstructFromJsonFileAsync(string jsonFilePath, CancellationToken cancellationToken)
        {
            string json = await File.ReadAllTextAsync(jsonFilePath, System.Text.Encoding.UTF8, cancellationToken).ConfigureAwait(false);
            TokenizerRegistry? jObj = JsonSerializer.Deserialize<TokenizerRegistry>(json);

            if(jObj == null)
            {
                Console.Error.WriteLine($"Invalid Json found @{jsonFilePath}. Terminating tokenizer maps construction");
                return null;
            }

            return ConstructFromRegistry(jObj, cancellationToken);
        }


        /// <summary>
        /// Construct TokenizerMaps by parsing a JsonObject. Will return null if the JsonObject is not valid.
        /// Expects a JsonObject with 'bpe_ranks' , 'pat_str' properties and an optional 'special_tokens' property.
        /// </summary>
        /// <param name="json">Json object to parse and construct tokenizer maps</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Constructed tokenizer maps. null if the json is not valid</returns>
        internal TokenizerMaps? ConstructFromRegistry(TokenizerRegistry reg, CancellationToken cancellationToken)
        {                      
            if (string.IsNullOrEmpty(reg.BpeRanks))
            {
                Console.Error.WriteLine("there is no json property found for bpe_ranks (byte pair encoding ranks). Terminating TokenMap construction");
                return null;
            }            

            if (string.IsNullOrEmpty(reg.PatternString))
            {
                Console.Error.WriteLine("there is no json property found for pat_str (regex pattern string). Terminating TokenMap construction");
                return null;
            }                        

            return Build(reg.BpeRanks, reg.PatternString, reg.SpecialTokens, cancellationToken);
        }

        /// <summary>
        /// Constructs TokenizerMaps taking in bytepairencoding ranks and other additional parameters
        /// </summary>
        /// <param name="bpeRanks">a string formed by joining various base64 encoded strings separated by space.
        /// Rank of each base64 encoded string is incremented from the offset based on the order in which they appear.</param>
        /// <param name="stringPattern">regex string pattern used to split the given text for bpe</param>
        /// <param name="specialTokens">Key value pair of any special tokens to be considered</param>
        /// <param name="cancellation">Cancellation Token</param>
        /// <returns>Constructed TokenizerMaps Object</returns>
        internal TokenizerMaps Build(string bpeRanks, string stringPattern, IDictionary<string, int>? specialTokens, CancellationToken cancellation)
        {
            var maps = new TokenizerMaps();
            Array.ForEach(bpeRanks.Split("\n", StringSplitOptions.RemoveEmptyEntries), line =>
            {
                string[] splits = line.Split(" ");
                int offset = int.Parse(splits[1]);
                for (int i = 2; i < splits.Length; i++)
                {
                    if (cancellation.IsCancellationRequested)
                    {
                        cancellation.ThrowIfCancellationRequested();
                    }

                    int rank = offset + i - 2;
                    maps.TextMap.Add(rank, ByteString.FromBase64(splits[i]));
                    maps.RankMap.Add(splits[i], rank);
                }
            });

            if (specialTokens != null)
            {
                foreach (var token in specialTokens)
                {
                    maps.SpecialTokens.Add(token.Key, token.Value);
                    maps.InverseSpecialTokens.Add(token.Value, ByteString.CopyFrom(System.Text.Encoding.UTF8.GetBytes(token.Key)));
                }
            }

            if (!string.IsNullOrEmpty(stringPattern))
            {
                maps.RegexPattern = stringPattern;
            }

            return maps;
        }
    }
}
