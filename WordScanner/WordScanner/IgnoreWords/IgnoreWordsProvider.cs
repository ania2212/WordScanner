using WordScanner.Interfaces;

namespace WordScanner.IgnoreWords
{
    static public class IgnoreWordsProvider 
    {
        public static HashSet<string> LoadIgnoreWords(string path)
        {
            return File.ReadAllLines(path)
                       .Select(line => line.Trim().ToLowerInvariant())
                       .Where(line => !string.IsNullOrEmpty(line))
                       .ToHashSet();
        }
    }
}