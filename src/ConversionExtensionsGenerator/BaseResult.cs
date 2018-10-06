using System.Collections.Generic;

namespace ConversionExtensionsGenerator
{
    public class BaseResult
    {
        public List<GenerationLogItem> Errors { get; set; } = new List<GenerationLogItem>();
    }

    public enum LogLevel
    {
        Error,
        Warning
    }

    public class GenerationLogItem
    {

        public GenerationLogItem(LogLevel logLevel, string message)
        {
            Message = message;
            LogLevel = logLevel;
        }

        public LogLevel LogLevel { get; }

        public string Message { get; }
    }
}