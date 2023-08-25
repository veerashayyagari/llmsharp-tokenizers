using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using LLMSharp.Anthropic.Tokenizer;
using LLMSharp.OpenAi.Tokenizer;

namespace Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net70, baseline:true)]
    [SimpleJob(RuntimeMoniker.Net60)]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [MemoryDiagnoser]
    [HideColumns("Error", "StdDev", "StdDev", "RatioSD")]
    public class TokenizerBenchmarks
    {
        private OpenAiChatCompletionsTokenizer? openAiChatCompletionsTokenizer;
        private ClaudeTokenizer? claudeTokenizer;

        [Params(Constants.OpenAiPluginsDocumentation)]
        public string StringToEncode = string.Empty;

        [GlobalSetup]
        public void Setup()
        {
            openAiChatCompletionsTokenizer = new OpenAiChatCompletionsTokenizer();
            claudeTokenizer = new ClaudeTokenizer();
        }

        [Benchmark]
        public IReadOnlyList<int> OpenAiChatCompletionsTokenizerEncode() => openAiChatCompletionsTokenizer!.Encode(StringToEncode);

        [Benchmark]
        public int OpenAiChatCompletionsTokenizerCountTokens() => openAiChatCompletionsTokenizer!.CountTokens(StringToEncode);

        [Benchmark]
        public IReadOnlyList<int> ClaudeTokenizerEncode() => claudeTokenizer!.Encode(StringToEncode);

        [Benchmark]
        public int ClaudeTokenizerCountTokens() => claudeTokenizer!.CountTokens(StringToEncode);
    }
}
