using WordScanner.IgnoreWords;
using WordScanner.Services;
using WordScanner.WordAnalysis;

namespace WordScanner.UI
{
    public class UIHandler
    {
        private readonly StatisticsCollector _wordStatisticsCollector;
        private readonly string _ignoreWordsPath;
        private readonly FileProcessingService _fileProcessingService;
        private const string ExitCommand = "/exit";

        public UIHandler(StatisticsCollector wordStatistics, string ignoreWordsPath, FileProcessingService fileProcessingService)
        {
            _wordStatisticsCollector = wordStatistics;
            _ignoreWordsPath = ignoreWordsPath;
            _fileProcessingService = fileProcessingService;
        }

        public void RunConsoleLoop()
        {
            var folder = GetFolderPath();
            var ignoreWords = LoadIgnoreWords(_ignoreWordsPath);

            var files = _fileProcessingService.GetFilesToProcess(folder);
            if (!files.Any())
            {
                ShowError("No .txt or .html files found in the folder.");
                return;
            }

            _fileProcessingService.ProcessFiles(files, ignoreWords);

            while (true)
            {
                Console.Write("\nEnter a word to search or '/exit' to complete: ");
                var word = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(word))
                {
                    ShowError("Please enter a word.");
                    continue;
                }

                if (word.Equals(ExitCommand, StringComparison.OrdinalIgnoreCase)) 
                    break;

                var (totalCount, countsPerFile) = _wordStatisticsCollector.GetWordStatisticsData(word);
                DisplayWordStatistics(word, totalCount, countsPerFile);
            }
        }

        private void DisplayWordStatistics(string word, int totalCount, Dictionary<string, int> countsPerFile)
        {
            Console.WriteLine($"Statistics for word '{word}':");
            Console.WriteLine($"Total count in all files: {totalCount}");
            foreach (var pair in countsPerFile)
            {
                Console.WriteLine($"  {pair.Key}: {pair.Value}");
            }
        }

        private string GetFolderPath()
        {
            Console.WriteLine("Enter the folder path:");
            var folderPath = Console.ReadLine()!;
            while (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                ShowError("Invalid folder path. Please try again:");
                folderPath = Console.ReadLine();
            }
            return folderPath;
        }

        private void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }

        private HashSet<string> LoadIgnoreWords(string path)
        {
            return IgnoreWordsProvider.LoadIgnoreWords(path);
        }
    }
}
