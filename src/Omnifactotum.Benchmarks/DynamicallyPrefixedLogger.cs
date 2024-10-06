using System;
using BenchmarkDotNet.Loggers;

namespace Omnifactotum.Benchmarks;

internal class DynamicallyPrefixedLogger(ILogger logger, Func<LogKind?, string, string> getPrefix) : ILogger
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Func<LogKind?, string, string> _getPrefix = getPrefix ?? throw new ArgumentNullException(nameof(getPrefix));

    public virtual string Id => nameof(DynamicallyPrefixedLogger);

    public virtual int Priority => _logger.Priority;

    public static DynamicallyPrefixedLogger CreateTimestampPrefixedLogger(ILogger logger)
        => new(logger, (_, _) => $"[{DateTimeOffset.Now.ToFixedStringWithMilliseconds()}]\x0020");

    public void Write(LogKind logKind, string text)
    {
        var prefix = _getPrefix(logKind, text);
        _logger.Write(logKind, prefix + text);
    }

    public void WriteLine()
    {
        var prefix = _getPrefix(null, string.Empty);
        _logger.WriteLine(prefix);
    }

    public void WriteLine(LogKind logKind, string text)
    {
        var prefix = _getPrefix(logKind, text);
        _logger.WriteLine(logKind, prefix + text);
    }

    public void Flush() => _logger.Flush();
}