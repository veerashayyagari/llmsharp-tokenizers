using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using LLMSharp.OpenAi.Tokenizer;

namespace Benchmarks
{
    [SimpleJob(RuntimeMoniker.Net70, baseline:true)]    
    [MarkdownExporterAttribute.GitHub]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [MemoryDiagnoser]
    [HideColumns("Error", "StdDev", "StdDev", "RatioSD")]
    public class OpenAiChatComplTokenizerBenchmarks
    {
        private OpenAiChatCompletionsTokenizer? tokenizer;

        [Params(Constants.OpenAiPluginsDocumentation)]
        public string StringToEncode = string.Empty;

        [GlobalSetup]
        public void Setup()
        {
            tokenizer = new OpenAiChatCompletionsTokenizer();
        }

        [Benchmark]
        public IReadOnlyList<int> Encode() => tokenizer!.Encode(StringToEncode);

        [Benchmark]
        public int CountTokens() => tokenizer!.CountTokens(StringToEncode);
    }
}
