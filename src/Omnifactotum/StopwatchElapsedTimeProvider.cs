using System;
using System.Diagnostics;
using Omnifactotum.Abstractions;

namespace Omnifactotum;

/// <summary>
///     The <see cref="Stopwatch"/> based implementation of <see cref="IElapsedTimeProvider"/>.
/// </summary>
public sealed class StopwatchElapsedTimeProvider : IElapsedTimeProvider
{
    private readonly Stopwatch _stopwatch;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StopwatchElapsedTimeProvider"/> class.
    /// </summary>
    public StopwatchElapsedTimeProvider() => _stopwatch = new Stopwatch();

    /// <inheritdoc />
    public bool IsRunning => _stopwatch.IsRunning;

    /// <inheritdoc />
    public TimeSpan Elapsed => _stopwatch.Elapsed;

    /// <inheritdoc />
    public void Start() => _stopwatch.Start();

    /// <inheritdoc />
    public void Stop() => _stopwatch.Stop();

    /// <inheritdoc />
    public void Reset() => _stopwatch.Reset();
}