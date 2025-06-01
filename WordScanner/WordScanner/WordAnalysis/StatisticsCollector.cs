using System.Collections.Concurrent;

namespace WordScanner.WordAnalysis
{
    public class StatisticsCollector
    {
        private readonly ConcurrentDictionary<string, WordInfo> _wordMap = new(StringComparer.OrdinalIgnoreCase);

        private class WordInfo
        {
            public int TotalCount;
            public ConcurrentDictionary<string, int> PerFileCount = new();
        }

        public void AddWords(string fileName, IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                var info = _wordMap.GetOrAdd(word, _ => new WordInfo());

                Interlocked.Increment(ref info.TotalCount);

                info.PerFileCount.AddOrUpdate(fileName, 1, (_, count) => count + 1);
            }
        }

        public (int totalCount, Dictionary<string, int> countsPerFile) GetWordStatisticsData(string word)
        {
            var totalCount = GetTotalCount(word);
            var countsPerFile = GetWordCountsPerFile(word);
            return (totalCount, countsPerFile);
        }

        private int GetTotalCount(string word)
        {
            return _wordMap.TryGetValue(word, out var info) ? info.TotalCount : 0;
        }

        private Dictionary<string, int> GetWordCountsPerFile(string word)
        {
            return _wordMap.TryGetValue(word, out var info)
                ? info.PerFileCount.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                : new Dictionary<string, int>();
        }
    }
}