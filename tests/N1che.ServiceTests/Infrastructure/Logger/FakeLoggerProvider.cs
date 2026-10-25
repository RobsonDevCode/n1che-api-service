using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace N1che.ServiceTests.Infrastructure.Logger;

public record FakeLog(LogLevel LogLevel, EventId EventId, string Message, Exception? Exception, IReadOnlyDictionary<string, object> Scopes);

public class FakeLoggerProvider : ILoggerProvider
{
    public ConcurrentQueue<FakeLog> Logs { get; } = new();

    public ILogger CreateLogger(string categoryName) => new FakeLogger(Logs);

    public void Dispose() { }

    private class FakeLogger(ConcurrentQueue<FakeLog> logs) : ILogger
    {
        private readonly AsyncLocal<Dictionary<string, object>?> _ambientScope = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            var parent = _ambientScope.Value;
            var scope = parent is null
                ? new Dictionary<string, object>()
                : new Dictionary<string, object>(parent);

            if (state is IEnumerable<KeyValuePair<string, object>> values)
            {
                foreach (var (key, value) in values)
                {
                    scope[key] = value;
                }
            }

            _ambientScope.Value = scope;
            return new ScopeDisposable(() => _ambientScope.Value = parent);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var scope = _ambientScope.Value ?? new Dictionary<string, object>();
            logs.Enqueue(new FakeLog(logLevel, eventId, formatter(state, exception), exception, scope));
        }
    }

    private sealed class ScopeDisposable(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }
}
