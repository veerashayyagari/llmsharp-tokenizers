using LLMSharp.Tokenizers.Shared;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LLMSharp.OpenAi.Tokenizer
{
    /// <summary>
    /// TikTokenizer implementation for OpenAi GPT ChatCompletion models (GPT 3.5/GPT 4)
    /// </summary>
    public class OpenAiChatCompletionsTokenizer: ILLMSharpTokenizer
    {
        private readonly TikTokenizer tokenizer;

        /// <summary>
        /// Creates an instance of OpenAi chat completions Tokenizer
        /// Reads the binary serialized bpe rank maps and regex pattern string
        /// Uses rankmaps and pattern string to create an instance of tiktokenizer
        /// </summary>
        public OpenAiChatCompletionsTokenizer()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("LLMSharp.OpenAi.Tokenizer.gpt-chatcompletions-token-maps.bin"))
            {
                var tokenMaps = TokenizerMaps.Parser.ParseFrom(stream);
                var regexes = new Regex(tokenMaps.RegexPattern, RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                tokenizer = new TikTokenizer(tokenMaps, regexes);
            }
        }

        /// <summary>
        /// Encodes a string into tokens
        /// Special tokens are artificial tokens used to unlock capabilities from a model,
        /// such as fill-in-the-middle.So we want to be careful about accidentally encoding special
        /// tokens, since they can be used to trick a model into doing something we don't want it to do.
        /// This method uses the default implementation and throws an error if the text contains any valid special tokens.
        /// For more granular control of encoding special tokens use 'EncodeWithSpecialTokens'
        /// </summary>
        /// <param name="text">text to encode using openai chat completions tokenizer</param>
        /// <returns>encoded tokens list of input text</returns>
        public IReadOnlyList<int> Encode(string text)
        {
            return this.tokenizer.Encode(text, new HashSet<string>(), null);
        }

        /// <summary>
        /// Counts number of byte pair encoded tokens for the given text input
        /// Special tokens are artificial tokens used to unlock capabilities from a model,
        /// such as fill-in-the-middle.So we want to be careful about accidentally encoding special
        /// tokens, since they can be used to trick a model into doing something we don't want it to do.
        /// This method uses the default implementation and throws an error if the text contains any valid special tokens.
        /// For more granular control of counting special tokens use 'CountWithSpecialTokens'
        /// </summary>
        /// <param name="text">text input for counting tokens using gpt chatcompletions tokenizer</param>
        /// <returns>number of tokens in the given text</returns>
        /// <exception cref="InvalidOperationException">thrown when any of the disallowed special tokens are found in the text</exception>
        public int CountTokens(string text)
        {
            return this.tokenizer.CountTokens(text, new HashSet<string>(), null);
        }

        /// <summary>
        /// Encodes a string into tokens using openai chat completions bpe ranks
        /// Special tokens are artificial tokens used to unlock capabilities from a model,
        /// such as fill-in-the-middle.So we want to be careful about accidentally encoding special
        /// tokens, since they can be used to trick a model into doing something we don't want it to do.
        /// 1. If the tokenizer needs to allow all special tokens : pass null for allowedSpecialTokens and disallowedSpecialTokens
        /// 2. If the tokenizer needs to allow only a limited set of special tokens : use the allowedSpecialTokens for allowed and disallowedSpecialTokens for tokens to be disallowed
        /// 3. allowedSpecialTokens and disallowedSpecialTokens should contain only valid supported tokens by the model
        /// </summary>
        /// <param name="text">text to encode using openai chat completions tokenizer</param>
        /// <param name="allowedSpecialTokens">special tokens that are allowed for tokenization. If null, all the special tokens supported by the model are allowed. If empty, none of the special tokens are allowed.</param>
        /// <param name="disallowedSpecialTokens">special tokens that should be disallowed for tokenization. If null, any special token that is not allowed will be considered disallowed.</param>
        /// <returns>number of tokens for the given text</returns>
        /// <exception cref="InvalidOperationException">thrown when any of the disallowed special tokens are found in the text</exception>

        public IReadOnlyList<int> EncodeWithSpecialTokens(string text, IEnumerable<string> allowedSpecialTokens, IEnumerable<string> disallowedSpecialTokens)
        {
            return this.tokenizer.Encode(
                text,
                (allowedSpecialTokens == null) ? null : new HashSet<string>(allowedSpecialTokens),
                (disallowedSpecialTokens == null) ? null : new HashSet<string>(disallowedSpecialTokens));
        }

        /// <summary>
        /// Counts number of byte pair encoded tokens for the given text input
        /// Special tokens are artificial tokens used to unlock capabilities from a model,
        /// such as fill-in-the-middle.So we want to be careful about accidentally encoding special
        /// tokens, since they can be used to trick a model into doing something we don't want it to do.
        /// 1. If the tokenizer needs to allow all special tokens : pass null for allowedSpecialTokens and disallowedSpecialTokens
        /// 2. If the tokenizer needs to allow only a limited set of special tokens : use the allowedSpecialTokens for allowed and disallowedSpecialTokens for tokens to be disallowed
        /// 3. allowedSpecialTokens and disallowedSpecialTokens should contain only valid supported tokens by the model
        /// </summary>
        /// <param name="text">text input for counting tokens using gpt chatcompletions tokenizer</param>
        /// <param name="allowedSpecialTokens">special tokens that are allowed for tokenization. If null, all the special tokens supported by the model are allowed. If empty, none of the special tokens are allowed.</param>
        /// <param name="disallowedSpecialTokens">special tokens that should be disallowed for tokenization. If null, any special token that is not allowed will be considered disallowed.</param>
        /// <returns>number of tokens for the given text</returns>
        /// <exception cref="InvalidOperationException">thrown when any of the disallowed special tokens are found in the text</exception>

        public int CountWithSpecialTokens(string text, IEnumerable<string> allowedSpecialTokens, IEnumerable<string> disallowedSpecialTokens)
        {
            return this.tokenizer.CountTokens(
                text,
                (allowedSpecialTokens == null) ? null : new HashSet<string>(allowedSpecialTokens),
                (disallowedSpecialTokens == null) ? null : new HashSet<string>(disallowedSpecialTokens));
        }

        /// <summary>
        /// Decodes a list of tokens into a string using openai chat completions bpe ranks
        /// Useful for visualizing tokenization
        /// </summary>
        /// <param name="tokens">list of tokens to decode</param>
        /// <returns>decoded string using the tokens</returns>
        public string Decode(IEnumerable<int> tokens)
        {
            return this.tokenizer.Decode(tokens);
        }
    }
}
