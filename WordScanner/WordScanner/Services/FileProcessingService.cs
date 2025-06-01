using WordScanner.Factories;
using WordScanner.Helpers;
using WordScanner.Interfaces;
using WordScanner.WordAnalysis;

namespace WordScanner.Services
{
    public class FileProcessingService
    {
        private readonly StatisticsCollector _wordStatisticsCollector;
        private readonly IFileProcessorFactory _fileProcessorFactory;

        public FileProcessingService(StatisticsCollector wordStatisticsCollector, IFileProcessorFactory fileProcessorFactory)
        {
            _wordStatisticsCollector = wordStatisticsCollector;
            _fileProcessorFactory = fileProcessorFactory;
        }

        public IEnumerable<string> GetFilesToProcess(string folder)
        {
            return Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                             .Where(f => f.EndsWith(".txt") || f.EndsWith(".html"));
        }

        public void ProcessFiles(IEnumerable<string> files, HashSet<string> ignoreWords)
        {
            Parallel.ForEach(files, file =>
            {
                IFileProcessor processor = _fileProcessorFactory.CreateProcessor(file);

                var content = processor.ReadContent(file);
                var words = WordExtractor.ExtractWords(content).Where(w => !ignoreWords.Contains(w));
                _wordStatisticsCollector.AddWords(Path.GetFileName(file), words);
            });
        }
    }
}
