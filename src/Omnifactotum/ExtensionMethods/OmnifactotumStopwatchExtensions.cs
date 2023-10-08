using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Diagnostics;

/// <summary>
///     Contains extension methods for the <see cref="Stopwatch"/> type.
/// </summary>
public static class OmnifactotumStopwatchExtensions
{
    /// <summary>
    ///     Gets the total elapsed time measured by the specified <see cref="Stopwatch"/> that was stopped before measuring.
    /// </summary>
    /// <param name="stopwatch">
    ///     <see cref="Stopwatch"/> to stop and measure the total elapsed time of.
    /// </param>
    /// <returns>
    ///     The total elapsed time measured by the specified <see cref="Stopwatch"/> that was stopped before measuring.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static TimeSpan GetStoppedElapsed([NotNull] this Stopwatch stopwatch)
    {
        if (stopwatch is null)
        {
            throw new ArgumentNullException(nameof(stopwatch));
        }

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }
}