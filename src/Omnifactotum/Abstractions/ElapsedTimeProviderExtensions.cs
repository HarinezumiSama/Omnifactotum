using System;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

namespace Omnifactotum.Abstractions;

/// <summary>
///     Contains extension methods for the <see cref="IElapsedTimeProvider"/> interface.
/// </summary>
public static class ElapsedTimeProviderExtensions
{
    /// <summary>
    ///     Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
    /// </summary>
    public static void Restart(this IElapsedTimeProvider elapsedTimeProvider)
    {
        if (elapsedTimeProvider is null)
        {
            throw new ArgumentNullException(nameof(elapsedTimeProvider));
        }

        elapsedTimeProvider.Reset();
        elapsedTimeProvider.Start();
    }

    /// <summary>
    ///     Gets the total elapsed time measured by the specified <see cref="IElapsedTimeProvider"/> that was stopped before measuring.
    /// </summary>
    /// <param name="elapsedTimeProvider">
    ///     <see cref="IElapsedTimeProvider"/> to stop and measure the total elapsed time of.
    /// </param>
    /// <returns>
    ///     The total elapsed time measured by the specified <see cref="IElapsedTimeProvider"/> that was stopped before measuring.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static TimeSpan GetStoppedElapsed([NotNull] this IElapsedTimeProvider elapsedTimeProvider)
    {
        if (elapsedTimeProvider is null)
        {
            throw new ArgumentNullException(nameof(elapsedTimeProvider));
        }

        elapsedTimeProvider.Stop();
        return elapsedTimeProvider.Elapsed;
    }
}