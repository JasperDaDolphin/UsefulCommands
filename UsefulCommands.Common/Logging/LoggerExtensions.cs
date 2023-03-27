using Microsoft.Extensions.Logging;

namespace UsefulCommands.Common
{
    public static class LoggerExtensions
    {
        public static ConsoleColor ToColour(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return ConsoleColor.Green;
                case LogLevel.Information:
                    return ConsoleColor.Gray;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                default: 
                    return ConsoleColor.Gray;
            }
        }
    }
}
