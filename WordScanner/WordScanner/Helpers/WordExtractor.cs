using System.Text.RegularExpressions;

namespace WordScanner.Helpers
{
    public static class WordExtractor
    {
        public static IEnumerable<string> ExtractWords(string text)
        {
            return Regex.Matches(text.ToLowerInvariant(), @"\b[a-zA-Z]+\b")
                        .Select(m => m.Value);
        }
    }
}
