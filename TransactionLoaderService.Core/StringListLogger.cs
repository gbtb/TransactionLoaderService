using Microsoft.Extensions.Logging;

namespace TransactionLoaderService.Core;

public class StringListLogger: ILogger
{
    private readonly ILogger _loggerImplementation;
    private readonly Lazy<List<string>> _errors;

    public StringListLogger(ILogger loggerImplementation)
    {
        _loggerImplementation = loggerImplementation;
        _errors = new Lazy<List<string>>();
    }

    public List<string> Errors => _errors.Value;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        _errors.Value.Add(message);
        _loggerImplementation.Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _loggerImplementation.IsEnabled(logLevel);
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _loggerImplementation.BeginScope(state);
    }
}