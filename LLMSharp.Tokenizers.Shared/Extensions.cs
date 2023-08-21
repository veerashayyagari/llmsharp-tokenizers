using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LLMSharp.Tokenizers.Shared
{
    /// <summary>
    /// Helpful extensions for string manipulation
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Escapes any special chracters in the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns>returns the string with special characters escaped</returns>
        internal static string EscapeRegex(this string input)
        {
            return Regex.Replace(input, @"[\\^$*+?.()|[\]{}]", "\\$&");
        }


        /// <summary>
        /// Generates a regex that matches existence of any token in tokens array
        /// </summary>
        /// <param name="tokens">an array of tokens for generating regex</param>
        /// <returns>returns the generated regex</returns>
        internal static Regex CreateRegexFromTokens(this IEnumerable<string> tokens)
        {
            var builder = new StringBuilder();
            foreach (var token in tokens)
            {
                if (builder.Length > 0)
                {
                    builder.Append('|');
                }
                builder.Append(token.EscapeRegex());
            }
            var pattern = builder.ToString();
            return new Regex(pattern);
        }        
    }
}
