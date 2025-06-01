using WordScanner.Interfaces;

namespace WordScanner.FileProcessors
{
    public class TxtFileProcessor : IFileProcessor
    {
        public string ReadContent(string filePath)
        {
            return File.ReadAllText(filePath);
        }
    }
}
