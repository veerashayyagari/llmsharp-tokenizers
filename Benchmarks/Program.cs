using BenchmarkDotNet.Running;
using Benchmarks;

var summary = BenchmarkRunner.Run<OpenAiChatComplTokenizerBenchmarks>();
Console.WriteLine(summary);
