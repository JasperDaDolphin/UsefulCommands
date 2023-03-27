using Microsoft.Extensions.Logging;

namespace UsefulCommands.Common
{
    public class LoggerOptions
    {
        public Dictionary<LogLevel, bool> LogLevelSettings { get; set; }
    }
}
