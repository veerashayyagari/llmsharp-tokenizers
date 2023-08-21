# LLMSharp Tokenizers

[![build and test](https://github.com/veerashayyagari/llmsharp-tokenizers/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/veerashayyagari/llmsharp-tokenizers/actions/workflows/build-and-test.yml)

- **LLMSharp.Anthropic.Tokenizer** : Unofficial implementation of tokenizer for Anthropic claude in dotnet. Install this nuget package for Encoding using Claude Tokenizer.
- **LLMSharp.OpenAi.Tokenizer** : Unofficial implementation of tokenizer for GPT-3.5/GPT-4 models in dotnet. Install this nuget package for Encoding using GPT Chat Completions Model Tokenizer.

## Usage

Install the appropriate nuget package

- Create an instance of the tokenizer

```csharp
// Claude Tokenizer
using LLMSharp.Anthropic.Tokenizer;

var tokenizer = new Claude();


// OpenAi ChatCompletion Models Tokenizer
using LLMSharp.OpenAi.Tokenizer;

var tokenizer = new ChatCompletions();
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
