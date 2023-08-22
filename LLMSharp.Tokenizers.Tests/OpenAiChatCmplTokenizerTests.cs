using LLMSharp.OpenAi.Tokenizer;

namespace LLMSharp.Tokenizers.Tests
{
    [TestClass]
    public class OpenAiChatCmplTokenizerTests
    {
        private readonly OpenAiChatCompletionsTokenizer chatCompletionsTokenizer;

        public OpenAiChatCmplTokenizerTests() { this.chatCompletionsTokenizer = new OpenAiChatCompletionsTokenizer(); }

        [TestMethod]
        public void TestEncoding()
        {
            foreach (var test in TestData.GptChatCompletionsStrings)
            {
                var encodedBytes = chatCompletionsTokenizer.Encode(test.Value);
                var decodedText = chatCompletionsTokenizer.Decode(encodedBytes);
                Assert.AreEqual(decodedText, test.Value, $"Encoding for {test.Key} failed");
            }
        }

        [TestMethod]
        public void TestCountTokens()
        {
            foreach (var test in TestData.GptChatCompletionsStrings)
            {
                var encodedBytes = chatCompletionsTokenizer.Encode(test.Value);
                var tokenCount = chatCompletionsTokenizer.CountTokens(test.Value);
                Assert.AreEqual(encodedBytes.Count, tokenCount, $"Count Tokens for {test.Key} failed");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSpecialTokensWithDefaultEncoding_Should_Throw_Exception()
        {
            chatCompletionsTokenizer.Encode(TestData.GptChatCompletionSpecialCharacters);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSpecialTokensWithDefaultCountTokens_Should_Throw_Exception()
        {
            chatCompletionsTokenizer.CountTokens(TestData.GptChatCompletionSpecialCharacters);
        }

        [TestMethod]
        public void ShouldEncodeSpecialTokens_WhenUsing_EncodeWithSpecialTokens()
        {
            var encodedBytes = chatCompletionsTokenizer.EncodeWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, null, null);
            var decodedText = chatCompletionsTokenizer.Decode(encodedBytes);
            Assert.AreEqual(decodedText, TestData.GptChatCompletionSpecialCharacters);
        }

        [TestMethod]
        public void ShouldCountSpecialTokens_WhenUsing_CountWithSpecialTokens()
        {
            var encodedBytes = chatCompletionsTokenizer.EncodeWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, null, null);
            var tokenCount = chatCompletionsTokenizer.CountWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, null, null);
            Assert.AreEqual(encodedBytes.Count, tokenCount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowException_WhenSpecialToken_IsNotIncluded_WhenUsing_EncodeWithSpecialTokens()
        {
            chatCompletionsTokenizer.EncodeWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, new string[] { "<|fim_suffix|>" }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowException_WhenSpecialToken_IsNotIncluded_WhenUsing_CountWithSpecialTokens()
        {
            chatCompletionsTokenizer.CountWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, new string[] { "<|fim_suffix|>" }, null);
        }

        [TestMethod]
        public void ShouldEncode_WhenSpecialTokens_AreIncluded_WhenUsing_EncodeWithSpecialTokens()
        {
            var encodedBytes = chatCompletionsTokenizer.EncodeWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, new string[] { "<|endoftext|>", "<|fim_suffix|>" }, null);
            var decodedText = chatCompletionsTokenizer.Decode(encodedBytes);
            Assert.AreEqual(decodedText, TestData.GptChatCompletionSpecialCharacters);
        }

        [TestMethod]
        public void ShouldEncode_WhenSpecialTokens_AreIncluded_WhenUsing_CountWithSpecialTokens()
        {
            var encodedBytes = chatCompletionsTokenizer.EncodeWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, new string[] { "<|endoftext|>", "<|fim_suffix|>" }, null);
            var tokenCount = chatCompletionsTokenizer.CountWithSpecialTokens(TestData.GptChatCompletionSpecialCharacters, new string[] { "<|endoftext|>", "<|fim_suffix|>" }, null);
            Assert.AreEqual(encodedBytes.Count, tokenCount);
        }
    }
}
