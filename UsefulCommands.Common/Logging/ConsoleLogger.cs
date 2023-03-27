using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UsefulCommands.Common;

public class ConsoleLogger : ILogger
{
    private readonly LoggerOptions _options;

    public ConsoleLogger(LoggerOptions options)
    {
        _options = options;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel)
    {
        if (_options.LogLevelSettings.TryGetValue(logLevel, out bool enabled)) 
        {
            return enabled;
        }
        return false;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        lock (Console.Out)
        {
            Console.ForegroundColor = logLevel.ToColour();
            Console.WriteLine($"{formatter(state, exception)}");
            Console.ResetColor();
        }
    }
}