using Microsoft.Extensions.Configuration;
using WordScanner.Factories;
using WordScanner.IgnoreWords;
using WordScanner.Services;
using WordScanner.WordAnalysis;

namespace WordScanner.UI
{
    public partial class WordScannerForm : Form
    {
        private readonly TextBox textBoxFolderPath;
        private readonly Button buttonBrowse;
        private readonly Button buttonProcess;
        private readonly ListView listViewFileCounts;
        private readonly TextBox textBoxSearchWord;
        private readonly Label labelSearchWord;

        private readonly StatisticsCollector statisticsCollector;
        private readonly FileProcessorFactory fileProcessorFactory;
        private readonly FileProcessingService fileProcessingService;

        private readonly IConfiguration Configuration;
        private readonly string? ignoreWordsPath;

        public WordScannerForm()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;

            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            ignoreWordsPath = Configuration["IgnoreWordsPath"];

            if (string.IsNullOrWhiteSpace(ignoreWordsPath))
            {
                MessageBox.Show("Error: The path to the ignore words file is not set in appsettings.json.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1); 
            }

            statisticsCollector = new StatisticsCollector();
            fileProcessorFactory = new FileProcessorFactory();
            fileProcessingService = new FileProcessingService(statisticsCollector, fileProcessorFactory);

            buttonBrowse = new Button { Text = "Browse...", Left = 40, Top = 10, Width = 100, Height = 25 };
            buttonBrowse.Click += ButtonBrowse_Click;
            Controls.Add(buttonBrowse);

            textBoxFolderPath = new TextBox { Name = "textBoxFolderPath", Left = 160, Top = 12, Width = 300 };
            Controls.Add(textBoxFolderPath);

            listViewFileCounts = new ListView
            {
                Left = 40,
                Top = 90,
                Width = 420,
                Height = 200,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };
            listViewFileCounts.Columns.Add("File Name", 350);
            listViewFileCounts.Columns.Add("Count", 100);
            Controls.Add(listViewFileCounts);

            labelSearchWord = new Label
            {
                Text = "Enter word:",
                Left = 40,
                Top = 45,
                Width = 100
            };
            Controls.Add(labelSearchWord);

            textBoxSearchWord = new TextBox
            {
                Left = 160,
                Top = 45,
                Width = 300,
                Text = "" 
            };
            Controls.Add(textBoxSearchWord);

            buttonProcess = new Button { Text = "Process", Left = 40, Top = 300, Width = 100, Height = 25 };
            buttonProcess.Click += ButtonProcess_Click;
            Controls.Add(buttonProcess);
        }

        private void ButtonBrowse_Click(object? sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFolderPath.Text = folderDialog.SelectedPath;
            }
        }

        private void ButtonProcess_Click(object? sender, EventArgs e)
        {
            var folderPath = textBoxFolderPath.Text;

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                MessageBox.Show("Invalid folder path.");
                return;
            }

            var files = fileProcessingService.GetFilesToProcess(folderPath);
            if (!files.Any())
            {
                MessageBox.Show("No .txt or .html files found in the folder.");
                return;
            }

            var ignoreWords = IgnoreWordsProvider.LoadIgnoreWords(ignoreWordsPath!);

            fileProcessingService.ProcessFiles(files, ignoreWords);

            var searchWord = textBoxSearchWord.Text.Trim();

            if (string.IsNullOrEmpty(searchWord))
            {
                MessageBox.Show("Please enter a word to search.");
                return;
            }

            var (totalCount, countsPerFile) = statisticsCollector.GetWordStatisticsData(searchWord);

            listViewFileCounts.Items.Clear();

            foreach (var kvp in countsPerFile.OrderByDescending(kv => kv.Value))
            {
                var item = new ListViewItem(kvp.Key);    
                item.SubItems.Add(kvp.Value.ToString()); 
                listViewFileCounts.Items.Add(item);
            }

            MessageBox.Show($"Total occurrences of '{searchWord}': {totalCount}");
        }
    }
}
