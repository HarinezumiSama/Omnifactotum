using System;
using System.Diagnostics;

namespace Omnifactotum.Abstractions;

/// <summary>
///     Provides a set of methods and properties that can be used to accurately measure elapsed time. This interface is typically
///     used to avoid dependency on <see cref="Stopwatch"/>.
/// </summary>
public interface IElapsedTimeProvider
{
    /// <summary>
    ///     Gets a value indicating whether the <see cref="T:IElapsedTimeProvider"/> timer is running.
    /// </summary>
    /// <returns>
    ///     <see langword="true"/> if the <see cref="T:IElapsedTimeProvider"/> instance is currently running and measuring elapsed
    ///     time for an interval; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsRunning { get; }

    /// <summary>
    ///     Gets the total elapsed time measured by the current instance.
    /// </summary>
    /// <returns>
    ///     A read-only <see cref="T:System.TimeSpan"/> representing the total elapsed time measured by the current instance.
    /// </returns>
    TimeSpan Elapsed { get; }

    /// <summary>
    ///     Starts, or resumes, measuring elapsed time for an interval.
    /// </summary>
    void Start();

    /// <summary>
    ///     Stops measuring elapsed time for an interval.
    /// </summary>
    void Stop();

    /// <summary>
    ///     Stops time interval measurement and resets the elapsed time to zero.
    /// </summary>
    void Reset();
}