using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;

namespace DotNet.VersionManager
{
    class ConsoleLogger : ILogger
    {
        private IConsole _console;
        private bool _verbose;

        private static readonly IDictionary<LogLevel, ConsoleColor?> _colors
            = new Dictionary<LogLevel, ConsoleColor?>
            {
                [LogLevel.Critical] = ConsoleColor.Red,
                [LogLevel.Error] = ConsoleColor.Red,
                [LogLevel.Warning] = ConsoleColor.Yellow,
                [LogLevel.Information] = null,
                [LogLevel.Debug] = ConsoleColor.DarkGray,
                [LogLevel.Trace] = ConsoleColor.DarkGray,
            };

        public ConsoleLogger(IConsole console, bool verbose)
        {
            _console = console;
            _verbose = verbose;
        }

        public IDisposable BeginScope<TState>(TState state)
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel <= LogLevel.Debug && !_verbose)
            {
                return;
            }

            _console.WriteLine(formatter(state, exception), _colors[logLevel]);
        }
    }
}
