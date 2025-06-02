using WordScanner.Common;
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

                if (word.Equals(Constants.ExitCommand, StringComparison.OrdinalIgnoreCase)) 
                    break;

                try
                {
                    var (totalCount, countsPerFile) = _wordStatisticsCollector.GetWordStatisticsData(word);
                    DisplayWordStatistics(word, totalCount, countsPerFile);
                }
                catch (Exception ex)
                {
                    ShowError($"Failed to process word '{word}': {ex.Message}");
                }
            }
        }

        private void DisplayWordStatistics(string word, int totalCount, Dictionary<string, int> countsPerFile)
        {
            Console.WriteLine($"Statistics for word '{word}':");
            Console.WriteLine($"Total count in all files: {totalCount}");

            Console.WriteLine($"{"File", Constants.OutputLayout.FileColumnWidth} {"Count", Constants.OutputLayout.CountColumnWidth}");
            Console.WriteLine(new string('-', Constants.OutputLayout.TableWidth));
            int maxFileNameLength = countsPerFile.Keys.Max(name => name.Length);
            foreach (var pair in countsPerFile.OrderByDescending(kv => kv.Value))
            {
                Console.WriteLine(FormatFileLine(pair.Key, pair.Value));
            }
        }

        private string FormatFileLine(string fileName, int count)
        {
            string cleanName = fileName.Replace("\n", "").Replace("\r", "").Trim();

            if (cleanName.Length > Constants.OutputLayout.FileNameWidth)
                cleanName = cleanName.Substring(0, Constants.OutputLayout.FileNameWidth - 3) + "...";

            return string.Format("  {0,-" + Constants.OutputLayout.FileNameWidth + "} | {1," + Constants.OutputLayout.CountColumnWidth + "}", cleanName, count);
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
