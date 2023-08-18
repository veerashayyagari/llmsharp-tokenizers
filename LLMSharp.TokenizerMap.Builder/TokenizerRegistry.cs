using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LLMSharp.TokenizerMap.Builder
{
    public class TokenizerRegistry
    {
        [JsonPropertyName("explicit_n_vocab")]
        public int ExplicitVocabCount { get; set; }

        [JsonPropertyName("pat_str")]
        public string? PatternString { get; set; }

        [JsonPropertyName("special_tokens")]
        public IDictionary<string, int>? SpecialTokens { get; set; }

        [JsonPropertyName("bpe_ranks")]
        public string? BpeRanks { get; set; }
    }
}
