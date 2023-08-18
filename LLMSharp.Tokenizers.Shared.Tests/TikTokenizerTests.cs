using System.Reflection;
using System.Text.RegularExpressions;

namespace LLMSharp.Tokenizers.Shared.Tests
{
    [TestClass]
    public class TikTokenizerTests
    {
        private static TikTokenizer? tokenizer;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("LLMSharp.Tokenizers.Shared.Tests.claude-token-maps.bin");
            var tokenMaps = TokenizerMaps.Parser.ParseFrom(stream);
            var regexes = new Regex(tokenMaps.RegexPattern, RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            tokenizer = new TikTokenizer(tokenMaps, regexes);
        }

        [TestMethod]
        public void TestEncoding()
        {
            foreach(var test in TestData.Strings)
            {
                var encodedBytes = tokenizer!.Encode(test.Value);
                var decodedText = tokenizer!.Decode(encodedBytes);
                Assert.AreEqual(decodedText, test.Value, $"Encoding for {test.Key} failed");
            }
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            tokenizer = null;
        }
    }
}