using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LLMSharp.Tokenizers.Shared
{
    /// <summary>
    /// TikToken implementation in C#, an experimental implementation
    /// Inspired by the original source at : https://github.com/openai/tiktoken/tree/main
    /// </summary>
    public class TikTokenizer
    {
        private readonly TokenizerMaps tokenMaps;
        private readonly Regex patternStringRegex;

        public TikTokenizer(TokenizerMaps modelTokenMaps, Regex modelPatternString)
        {
            this.tokenMaps = modelTokenMaps;
            this.patternStringRegex = modelPatternString;
        }

        /// <summary>
        /// Byte Pair Encodes a string into tokens
        /// Special tokens are artificial tokens used to unlock capabilities from a model,
        /// such as fill-in-the-middle.So we want to be careful about accidentally encoding special
        /// tokens, since they can be used to trick a model into doing something we don't want it to do.
        /// Hence, by default, encode will raise an error if it encounters text that corresponds
        /// to a special token.This can be controlled on a per-token level using the `allowed_special`
        /// and `disallowed_special` parameters.In particular:
        /// Setting 'disallowedSpecial' to null will prevent this function from raising errors and
        /// cause all text corresponding to special tokens to be encoded as natural text.
        /// Setting 'allowedSpecial' to null will cause this function to treat all text
        /// corresponding to special tokens to be encoded as special tokens.
        /// </summary>
        /// <param name="text">text input for counting number of tokens</param>
        /// <param name="allowedSpecial">special tokens that are allowed for tokenization. If null, all the special tokens supported by the model are allowed. If empty, none of the special tokens are allowed.</param>
        /// <param name="disallowedSpecial">special tokens that should be disallowed for tokenization. If null, any special token that is not allowed will be considered disallowed.</param>
        /// <returns>list of byte pair encoded tokens for the text</returns>
        /// <exception cref="InvalidOperationException">thrown when any of the disallowed special tokens are found in the text</exception>
        public IReadOnlyList<int> Encode(
            string text,
            HashSet<string> allowedSpecial,
            HashSet<string> disallowedSpecial
            )
        {
            HashSet<string> allowedSpecialSet = new HashSet<string>(tokenMaps.SpecialTokens.Keys);
            HashSet<string> disallowedSpecialSet = disallowedSpecial;

            if (allowedSpecial != null)
            {
                allowedSpecialSet.IntersectWith(allowedSpecial);
            }
            
            if(disallowedSpecialSet == null)
            {
                disallowedSpecialSet = new HashSet<string>(tokenMaps.SpecialTokens.Keys.Where(k => !allowedSpecialSet.Contains(k)));
            }

            Regex specialTokenRegex = tokenMaps.SpecialTokens.Keys.CreateRegexFromTokens();

            // validate if the text contains a disallowed special token
            if (disallowedSpecialSet.Count > 0)
            {
                Regex disallowedSpecailRegex = disallowedSpecialSet.CreateRegexFromTokens();
                Match disallowedSpecialMatch = disallowedSpecailRegex.Match(text);

                if (disallowedSpecialMatch.Success)
                {
                    throw new InvalidOperationException($"The text contains a special token that is not allowed: {disallowedSpecialMatch.Value}");
                }
            }

            // assuming each token is approximately 4 bytes, let's declare an initial capacity
            List<int> result = new List<int>(text.Length / 4);
            int start = 0;

            /**
             * We will perform the following in a loop till the end of string
             * 1. Identify the slice of string between two special tokens
             * 2. Run that slice through the pattern matching string regex to obtain matches
             * 3. Pass those matches and their corresponding ranks from tokenMap to BPE algorithm
             * 4. Get the corresponding compressed representation as tokens, append to our result
             */
            while (true)
            {
                int startFind = start;
                Match nextSpecial = null;

                // 1. Identify the index of next special token to slice
                while (true)
                {
                    nextSpecial = specialTokenRegex.Match(text, startFind);

                    // we didn't find any special token or the special token we found is allowed. Then break
                    if (!nextSpecial.Success || allowedSpecialSet.Contains(nextSpecial.Value)) break;
                    startFind = nextSpecial.Index + 1;
                }

                int end = (nextSpecial?.Success == true) ? nextSpecial.Index : text.Length;                

                // 2. Run the slice of the string through pattern matching
                foreach (Match match in patternStringRegex.Matches(text.Substring(start, end - start)))
                {
                    var matchBytes = ByteString.CopyFromUtf8(match.Value);

                    // if there is rank available for this slice, add it to result. If not perform BPE
                    if (tokenMaps.RankMap.TryGetValue(matchBytes.ToBase64(), out int rank))
                    {
                        result.Add(rank);
                        continue;
                    }

                    result.AddRange(BytePairEncode(matchBytes));
                }

                if (nextSpecial?.Success != true) break;
                result.Add(tokenMaps.SpecialTokens[nextSpecial.Value]);
                start = nextSpecial.Index + nextSpecial.Length;
            }

            return result;
        }

        /// <summary>
        /// Counts number of byte pair encoded tokens for the given text input
        /// Special tokens are artificial tokens used to unlock capabilities from a model,
        /// such as fill-in-the-middle.So we want to be careful about accidentally encoding special
        /// tokens, since they can be used to trick a model into doing something we don't want it to do.
        /// Hence, by default, encode will raise an error if it encounters text that corresponds
        /// to a special token.This can be controlled on a per-token level using the `allowed_special`
        /// and `disallowed_special` parameters.In particular:
        /// Setting 'disallowedSpecial' to null will prevent this function from raising errors and
        /// cause all text corresponding to special tokens to be encoded as natural text.
        /// Setting 'allowedSpecial' to null will cause this function to treat all text
        /// corresponding to special tokens to be encoded as special tokens.
        /// </summary>
        /// <param name="text">text input for counting number of tokens</param>
        /// <param name="allowedSpecial">special tokens that are allowed for tokenization. If null, all the special tokens supported by the model are allowed. If empty, none of the special tokens are allowed.</param>
        /// <param name="disallowedSpecial">special tokens that should be disallowed for tokenization. If null, any special token that is not allowed will be considered disallowed.</param>
        /// <returns>number of tokens for the given text</returns>
        /// <exception cref="InvalidOperationException">thrown when any of the disallowed special tokens are found in the text</exception>

        public int CountTokens(string text, HashSet<string> allowedSpecial, HashSet<string> disallowedSpecial)
        {
            HashSet<string> allowedSpecialSet = new HashSet<string>(tokenMaps.SpecialTokens.Keys);
            HashSet<string> disallowedSpecialSet = disallowedSpecial;
            int tokenCount = 0;

            if (allowedSpecial != null)
            {
                allowedSpecialSet.IntersectWith(allowedSpecial);
            }
            
            if(disallowedSpecialSet == null)
            {
                disallowedSpecialSet = new HashSet<string>(tokenMaps.SpecialTokens.Keys.Where(k => !allowedSpecialSet.Contains(k)));
            }

            Regex specialTokenRegex = tokenMaps.SpecialTokens.Keys.CreateRegexFromTokens();

            // validate if the text contains a disallowed special token
            if (disallowedSpecialSet.Count > 0)
            {
                Regex disallowedSpecailRegex = disallowedSpecialSet.CreateRegexFromTokens();
                Match disallowedSpecialMatch = disallowedSpecailRegex.Match(text);

                if (disallowedSpecialMatch.Success)
                {
                    throw new InvalidOperationException($"The text contains a special token that is not allowed: {disallowedSpecialMatch.Value}");
                }
            }

            int start = 0;
            while (true)
            {
                int startFind = start;
                Match nextSpecial = null;

                // Identify the index of the next special token to slice
                while (true)
                {
                    nextSpecial = specialTokenRegex.Match(text, startFind);
                    if (!nextSpecial.Success || allowedSpecialSet.Contains(nextSpecial.Value)) break;
                    startFind = nextSpecial.Index + 1;
                }

                int end = (nextSpecial?.Success == true) ? nextSpecial.Index : text.Length;

                foreach (Match match in patternStringRegex.Matches(text.Substring(start, end - start)))
                {
                    var matchBytes = ByteString.CopyFromUtf8(match.Value);
                    if (tokenMaps.RankMap.ContainsKey(matchBytes.ToBase64()))
                    {
                        tokenCount++;
                    }
                    else
                    {
                        tokenCount += CountBytePairEncodeTokens(matchBytes);  // A method similar to BytePairEncode, but just counts tokens
                    }
                }

                if (nextSpecial?.Success != true) break;
                tokenCount++;
                start = nextSpecial.Index + nextSpecial.Length;
            }

            return tokenCount;
        }

        /// <summary>
        /// Decodes a list of tokens into a string
        /// Useful for visualizing tokenization
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>decoded string using the tokens</returns>
        public string Decode(IEnumerable<int> tokens)
        {
            List<byte> result = new List<byte>();

            foreach (int token in tokens)
            {
                if (tokenMaps.TextMap.TryGetValue(token, out ByteString value))
                {
                    result.AddRange(value);
                }
                else if (tokenMaps.InverseSpecialTokens.TryGetValue(token, out ByteString inverse))
                {
                    result.AddRange(inverse);
                }
            }

            return Encoding.UTF8.GetString(result.ToArray());
        }

        /// <summary>
        /// Perform Byte Pair encoding of the byte array
        /// </summary>
        /// <param name="piece">byte array slice for performing BPE</param>
        /// <returns>readonly list of tokens after performing BPE</returns>
        private IReadOnlyList<int> BytePairEncode(ByteString piece)
        {
            List<int> ranks = new List<int>();

            if (piece.Length == 1)
            {
                if(tokenMaps.RankMap.TryGetValue(piece.ToBase64(), out int rank))
                {
                    ranks.Add(rank);
                }

                return ranks;
            }

            var bpm = BytePairMerge(piece);            
            foreach (var (start, end) in bpm)
            {
                var slice = Convert.ToBase64String(piece.Span.Slice(start, end - start).ToArray());                
                if (tokenMaps.RankMap.TryGetValue(slice, out int rank))
                {
                    ranks.Add(rank);
                }
            }

            return ranks;
        } 

        /// <summary>
        /// Counts number of byte pair encoded tokens in a given bytestring
        /// </summary>
        /// <param name="piece">bytestring used for counting tokens</param>
        /// <returns>count of tokens</returns>
        private int CountBytePairEncodeTokens(ByteString piece)
        {
            int count = 0;
            if (piece.Length == 1)
            {
                if (tokenMaps.RankMap.ContainsKey(piece.ToBase64()))
                {
                    count++;
                }
                return count;
            }

            var bpm = BytePairMerge(piece);
            foreach (var (start, end) in bpm)
            {
                var slice = Convert.ToBase64String(piece.Span.Slice(start, end - start).ToArray());
                if (tokenMaps.RankMap.ContainsKey(slice))
                {
                    count++;
                }
            }

            return count;
        }       

        /// <summary>
        /// Perform a byte pair merge of the given byte array using the rank map
        /// </summary>
        /// <param name="piece"></param>
        /// <returns>A readonly list of (start, end) index slices after merging in the ranked order</returns>
        private IReadOnlyList<(int start, int end)> BytePairMerge(ByteString piece)
        {
            var parts = Enumerable.Range(0, piece.Length)
                .Select(i => (start: i, end: i + 1))
                .ToList();

            while (parts.Count > 0)
            {
                var minRank = (rank: int.MaxValue, index: -1);
                for (int i = 0; i < parts.Count - 1; i++)
                {
                    var slice = piece.Span.Slice(parts[i].start, parts[i + 1].end - parts[i].start).ToArray();
                    if (!tokenMaps.RankMap.TryGetValue(Convert.ToBase64String(slice), out int rank))
                    {
                        continue;
                    }

                    if (rank < minRank.rank)
                    {
                        minRank.rank = rank;
                        minRank.index = i;
                    }
                }

                if (minRank.index > -1)
                {
                    int idx = minRank.index;
                    parts[idx] = (parts[idx].start, parts[idx + 1].end);
                    parts.RemoveAt(idx + 1);
                }
                else
                {
                    break;
                }
            }

            return parts;
        }        
    }
}
