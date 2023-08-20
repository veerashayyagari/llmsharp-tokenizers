using LLMSharp.Anthropic.Tokenizer;

namespace LLMSharp.Tokenizers.Tests
{
    [TestClass]
    public class ClaudeTokenizerTests
    {
        private readonly Claude claudeTokenizer;

        public ClaudeTokenizerTests() { this.claudeTokenizer = new Claude(); }

        [TestMethod]
        public void TestEncoding()
        {
            foreach (var test in TestData.ClaudeStrings)
            {
                var encodedBytes = claudeTokenizer.Encode(test.Value);
                var decodedText = claudeTokenizer.Decode(encodedBytes);
                Assert.AreEqual(decodedText, test.Value, $"Encoding for {test.Key} failed");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSpecialTokensWithDefaultEncoding_Should_Throw_Exception()
        {
            claudeTokenizer.Encode(TestData.AnthropicClaudeSpecialCharacters);
        }

        [TestMethod]
        public void ShouldEncodeSpecialTokens_WhenUsing_EncodeWithSpecialTokens()
        {
            var encodedBytes = claudeTokenizer.EncodeWithSpecialTokens(TestData.AnthropicClaudeSpecialCharacters, null, null);
            var decodedText = claudeTokenizer.Decode(encodedBytes);
            Assert.AreEqual(decodedText, TestData.AnthropicClaudeSpecialCharacters);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowException_WhenSpecialToken_IsNotIncluded_WhenUsing_EncodeWithSpecialTokens()
        {
            claudeTokenizer.EncodeWithSpecialTokens(TestData.AnthropicClaudeSpecialCharacters, new string[] { "<META_START>" }, null);
        }

        [TestMethod]
        public void ShouldEncode_WhenSpecialTokens_AreIncluded_WhenUsing_EncodeWithSpecialTokens()
        {
            var encodedBytes = claudeTokenizer.EncodeWithSpecialTokens(TestData.AnthropicClaudeSpecialCharacters, new string[] { "<META_START>", "<META_END>" }, null);
            var decodedText = claudeTokenizer.Decode(encodedBytes);
            Assert.AreEqual(decodedText, TestData.AnthropicClaudeSpecialCharacters);
        }
    }
}