# LLMSharp Tokenizers

[![build and test](https://github.com/veerashayyagari/llmsharp-tokenizers/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/veerashayyagari/llmsharp-tokenizers/actions/workflows/build-and-test.yml) [![CodeQL](https://github.com/veerashayyagari/llmsharp-tokenizers/actions/workflows/codeql.yml/badge.svg)](https://github.com/veerashayyagari/llmsharp-tokenizers/actions/workflows/codeql.yml)

- **LLMSharp.Anthropic.Tokenizer** : Unofficial implementation of tokenizer for Anthropic claude in dotnet. Install this nuget package for Encoding using Claude Tokenizer.
- **LLMSharp.OpenAi.Tokenizer** : Unofficial implementation of tokenizer for GPT-3.5/GPT-4 models in dotnet. Install this nuget package for Encoding using GPT Chat Completions Model Tokenizer.

## Usage

- Install the latest version of nuget package

```
dotnet add package LLMSharp.Anthropic.Tokenizer

dotnet add package LLMSharp.OpenAi.Tokenizer
```

- Create an instance of the tokenizer

```csharp
// Claude Tokenizer
using LLMSharp.Anthropic.Tokenizer;

var tokenizer = new ClaudeTokenizer();


// OpenAi ChatCompletion Models Tokenizer
using LLMSharp.OpenAi.Tokenizer;

var tokenizer = new OpenAiChatCompletionsTokenizer();
```

- **Encode** : tokenizes a given text, this is the default implementation that throws an exception if the text contains any special tokens

```csharp
var encodedTokens = tokenizer.Encode("hello world");
```

- **CountTokens** : count tokens in a given text, this is the default implementation that throws an exception if the text contains any special tokens

```csharp
var tokenCount = tokenizer.CountTokens("hello world");
```

- **EncodeWithSpecialTokens** : tokenizes a given text, including all or specific special tokens

```csharp
// passing 'null' for allowedSpecial , will help tokenize all special tokens
var encodedBytes = tokenizer.EncodeWithSpecialTokens(
    text:"<META_START>some data<META_END>",
    allowedSpecial: null,
    disallowedSpecial: null);


// passing an array of strings for allowedSpecial , will help tokenize only those special tokens
// any other special tokens found in the text will throw an exception
var encodedBytes = tokenizer.EncodeWithSpecialTokens(
    text:"<META_START>some data<META_END>",
    allowedSpecial: new string[]{"<META_START>", "<META_END>"},
    disallowedSpecial: null);
```

- **CountWithSpecialTokens** : count tokens in a given text, including all or specific special tokens

```csharp
var tokenCount = tokenizer.CountWithSpecialTokens(
    text:"<META_START>some data<META_END>",
    allowedSpecial: new string[]{"<META_START>", "<META_END>"},
    disallowedSpecial: null);
```

## Benchmarks

Encoding and CountTokens for 4200 tokens (~16 KB) of text

**Linux**

```

BenchmarkDotNet v0.13.7, Ubuntu 22.04.3 LTS (Jammy Jellyfish)
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 7.0.110
  [Host]   : .NET 7.0.10 (7.0.1023.36801), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.21 (6.0.2123.36801), X64 RyuJIT AVX2
  .NET 7.0 : .NET 7.0.10 (7.0.1023.36801), X64 RyuJIT AVX2


```
|                                    Method |      Job |  Runtime |       StringToEncode |     Mean |
|------------------------------------------ |--------- |--------- |--------------------- |---------:|
|      OpenAiChatCompletionsTokenizerEncode | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1.419 ms |
|      OpenAiChatCompletionsTokenizerEncode | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] | 1.370 ms |
|                                           |          |          |                      |          |
| OpenAiChatCompletionsTokenizerCountTokens | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1.444 ms |
| OpenAiChatCompletionsTokenizerCountTokens | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] | 1.326 ms |
|                                           |          |          |                      |          |
|                     ClaudeTokenizerEncode | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1.391 ms |
|                     ClaudeTokenizerEncode | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] | 1.327 ms |
|                                           |          |          |                      |          |
|                ClaudeTokenizerCountTokens | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1.409 ms |
|                ClaudeTokenizerCountTokens | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] | 1.279 ms |



**macOS**

```

BenchmarkDotNet v0.13.7, macOS Ventura 13.4.1 (c) (22F770820d) [Darwin 22.5.0]
Apple M2, 1 CPU, 8 logical and 8 physical cores
.NET SDK 7.0.304
  [Host]   : .NET 7.0.7 (7.0.723.27404), Arm64 RyuJIT AdvSIMD
  .NET 6.0 : .NET 6.0.21 (6.0.2123.36311), Arm64 RyuJIT AdvSIMD
  .NET 7.0 : .NET 7.0.7 (7.0.723.27404), Arm64 RyuJIT AdvSIMD


```
|                                    Method |      Job |  Runtime |       StringToEncode |       Mean |
|      OpenAiChatCompletionsTokenizerEncode | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1,133.5 μs |
|      OpenAiChatCompletionsTokenizerEncode | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] |   738.2 μs |
|                                           |          |          |                      |            |
| OpenAiChatCompletionsTokenizerCountTokens | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1,071.3 μs |
| OpenAiChatCompletionsTokenizerCountTokens | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] |   709.5 μs |
|                                           |          |          |                      |            |
|                     ClaudeTokenizerEncode | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1,186.3 μs |
|                     ClaudeTokenizerEncode | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] |   703.5 μs |
|                                           |          |          |                      |            |
|                ClaudeTokenizerCountTokens | .NET 6.0 | .NET 6.0 | Con(...)e.\n [16926] | 1,143.9 μs |
|                ClaudeTokenizerCountTokens | .NET 7.0 | .NET 7.0 | Con(...)e.\n [16926] |   711.3 μs |

**Windows**

```

BenchmarkDotNet v0.13.7, Windows 11 (10.0.22621.2134/22H2/2022Update/SunValley2)
Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores
.NET SDK 7.0.400
  [Host]   : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.21 (6.0.2123.36311), X64 RyuJIT AVX2
  .NET 7.0 : .NET 7.0.10 (7.0.1023.36312), X64 RyuJIT AVX2


```
|                                    Method |      Job |  Runtime |        StringToEncode |     Mean |
|------------------------------------------ |--------- |--------- |---------------------- |---------:|
|      OpenAiChatCompletionsTokenizerEncode | .NET 6.0 | .NET 6.0 | Con(...).\r\n [17157] | 1.451 ms |
|      OpenAiChatCompletionsTokenizerEncode | .NET 7.0 | .NET 7.0 | Con(...).\r\n [17157] | 1.406 ms |
|                                           |          |          |                       |          |
| OpenAiChatCompletionsTokenizerCountTokens | .NET 6.0 | .NET 6.0 | Con(...).\r\n [17157] | 1.347 ms |
| OpenAiChatCompletionsTokenizerCountTokens | .NET 7.0 | .NET 7.0 | Con(...).\r\n [17157] | 1.313 ms |
|                                           |          |          |                       |          |
|                     ClaudeTokenizerEncode | .NET 6.0 | .NET 6.0 | Con(...).\r\n [17157] | 1.469 ms |
|                     ClaudeTokenizerEncode | .NET 7.0 | .NET 7.0 | Con(...).\r\n [17157] | 1.286 ms |
|                                           |          |          |                       |          |
|                ClaudeTokenizerCountTokens | .NET 6.0 | .NET 6.0 | Con(...).\r\n [17157] | 1.441 ms |
|                ClaudeTokenizerCountTokens | .NET 7.0 | .NET 7.0 | Con(...).\r\n [17157] | 1.289 ms |