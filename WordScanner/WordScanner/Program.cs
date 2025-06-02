using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using WordScanner.Factories;
using WordScanner.Services;
using WordScanner.UI;
using WordScanner.WordAnalysis;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var ignoreWordsPath = configuration["IgnoreWordsPath"];
if (string.IsNullOrWhiteSpace(ignoreWordsPath))
{
    Console.WriteLine("IgnoreWordsPath was not set");
    return;
}

StatisticsCollector _wordStatistics = new();
FileProcessorFactory _fileProcessorFactory = new();
FileProcessingService _fileProcessingService = new(_wordStatistics, _fileProcessorFactory);

var ui = new UIHandler(_wordStatistics, ignoreWordsPath, _fileProcessingService);

ui.RunConsoleLoop();