using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Omnifactotum;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="IDisposable"/> and <see cref="IAsyncDisposable"/> interfaces.
/// </summary>
public static class OmnifactotumDisposableExtensions
{
    /// <summary>
    ///     Calls the <see cref="IDisposable.Dispose"/> method of the specified instance implementing
    ///     the <see cref="IDisposable"/> interface, if this instance is not <see langword="null"/>;
    ///     otherwise, does nothing.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the disposable instance.
    /// </typeparam>
    /// <param name="disposable">
    ///     A reference to an object to dispose.
    /// </param>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static void DisposeSafely<T>([CanBeNull] this T? disposable)
        where T : class, IDisposable
        => disposable?.Dispose();

    /// <summary>
    ///     Calls the <see cref="IDisposable.Dispose"/> method of the specified instance implementing
    ///     the <see cref="IDisposable"/> interface, if this instance is not <see langword="null"/>;
    ///     otherwise, does nothing.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the disposable instance.
    /// </typeparam>
    /// <param name="disposable">
    ///     A reference to an object to dispose.
    /// </param>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static void DisposeSafely<T>([CanBeNull] this T? disposable)
        where T : struct, IDisposable
        => disposable?.Dispose();

    /// <summary>
    ///     Configures an async disposable so that the awaits on the tasks returned from this disposable do not attempt
    ///     to marshal the continuation back to the original context captured.
    /// </summary>
    /// <param name="source">
    ///     The source async disposable.
    /// </param>
    /// <returns>
    ///     The configured async disposable.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ConfiguredAsyncDisposable ConfigureAwaitNoCapturedContext([NotNull] this IAsyncDisposable source)
        => (source ?? throw new ArgumentNullException(nameof(source))).ConfigureAwait(false);
}