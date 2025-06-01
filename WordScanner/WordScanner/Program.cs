using WordScanner.Factories;
using WordScanner.Services;
using WordScanner.UI;
using WordScanner.WordAnalysis;

string _ignoreWordsPath = @"D:\WordScanner\WordScanner\WordScanner\Resources\ignorewords.txt";
StatisticsCollector _wordStatistics = new();
FileProcessorFactory _fileProcessorFactory = new();
FileProcessingService _fileProcessingService = new(_wordStatistics, _fileProcessorFactory);

var ui = new UIHandler(_wordStatistics, _ignoreWordsPath, _fileProcessingService);

ui.RunConsoleLoop();