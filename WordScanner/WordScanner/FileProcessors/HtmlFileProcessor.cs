using System.Text.RegularExpressions;
using WordScanner.Interfaces;

namespace WordScanner.FileProcessors
{
    public class HtmlFileProcessor : IFileProcessor
    {
        public string ReadContent(string filePath)
        {
            var html = File.ReadAllText(filePath);
            return Regex.Replace(html, "<.*?>", string.Empty);
        }
    }
}
