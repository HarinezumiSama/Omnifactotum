using System;
using System.Diagnostics;
using Omnifactotum.Abstractions;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Omnifactotum;

/// <summary>
///     The <see cref="Stopwatch"/> based implementation of <see cref="IElapsedTimeProvider"/>.
/// </summary>
[DebuggerDisplay("{ToDebuggerString(),nq}")]
public sealed class StopwatchElapsedTimeProvider : IElapsedTimeProvider
{
    private readonly Stopwatch _stopwatch;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StopwatchElapsedTimeProvider"/> class.
    /// </summary>
    public StopwatchElapsedTimeProvider() => _stopwatch = new Stopwatch();

    /// <inheritdoc />
    [Pure]
    public bool IsRunning
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get => _stopwatch.IsRunning;
    }

    /// <inheritdoc />
    [Pure]
    public TimeSpan Elapsed
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get => _stopwatch.Elapsed;
    }

    /// <inheritdoc />
    public void Start() => _stopwatch.Start();

    /// <inheritdoc />
    public void Stop() => _stopwatch.Stop();

    /// <inheritdoc />
    public void Reset() => _stopwatch.Reset();

    internal string ToDebuggerString() => $"{nameof(IsRunning)} = {IsRunning}";
}