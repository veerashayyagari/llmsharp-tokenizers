using Google.Protobuf;

namespace LLMSharp.TokenizerMap.Builder
{
    /// <summary>
    /// Constructs TokenMaps and Serailizes them using Protobuf into .bin files
    /// </summary>
    internal class TokenizerMapSerializer
    {
        private readonly Uri claudeJsonUrl = new("https://raw.githubusercontent.com/anthropics/anthropic-tokenizer-typescript/main/claude.json");
        private readonly string claudeSerializedFilePath = "claude-token-maps.bin";

        private readonly string gptJsonFilePath = "cl100k_base.json";
        private readonly string gptSerializedFilePath = "gpt-chatcompletions-token-maps.bin";

        private readonly TokenizerMapFactory factory;
        
        internal TokenizerMapSerializer() 
        {
            factory = new TokenizerMapFactory();
        }

        /// <summary>
        /// Constructs token maps using gpt-4/3.5 bpe ranks and serailizes them using Protobuf into gpt-token-maps.bin file
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Completed Task</returns>
        internal async Task SerializeGptTokenMapsAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Started: SerializeGptTokenMapsAsync");
            var tokenMaps = await factory.ConstructFromJsonFileAsync(gptJsonFilePath, cancellationToken);
            if (tokenMaps == null)
            {
                Console.WriteLine("Error: SerializeGptTokenMapsAsync");
                return;
            }

            using var output = File.Create(gptSerializedFilePath);
            tokenMaps.WriteTo(output);
            Console.WriteLine("Success: SerializeGptTokenMapsAsync");
        }

        /// <summary>
        /// Constructs token maps using claude bpe ranks and serailizes them using Protobuf into anthropic-token-maps.bin file
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        internal async Task SerializeClaudeTokenMapsAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Started: SerializeClaudeTokenMapsAsync");
            var tokenMaps = await factory.ConstructFromRemoteJsonFileAsync(claudeJsonUrl, cancellationToken);
            if(tokenMaps == null)
            {
                Console.WriteLine("Error: SerializeClaudeTokenMapsAsync");
                return;
            }

            using var output = File.Create(claudeSerializedFilePath);
            tokenMaps.WriteTo(output);
            Console.WriteLine("Success: SerializeClaudeTokenMapsAsync");
        }
    }
}
