using Microsoft.VisualBasic;
using WordScanner.Common;
using WordScanner.FileProcessors;
using WordScanner.Interfaces;

namespace WordScanner.Factories
{
    public interface IFileProcessorFactory
    {
        IFileProcessor CreateProcessor(string filePath);
    }

    public class FileProcessorFactory : IFileProcessorFactory
    {
        public IFileProcessor CreateProcessor(string filePath)
        {
            if (filePath.EndsWith(Common.Constants.HtmlExtension, StringComparison.OrdinalIgnoreCase))
                return new HtmlFileProcessor();

            if (filePath.EndsWith(Common.Constants.TxtExtension, StringComparison.OrdinalIgnoreCase))
                return new TxtFileProcessor();

            throw new NotSupportedException($"File type of {filePath} is not supported.");
        }
    }
}
