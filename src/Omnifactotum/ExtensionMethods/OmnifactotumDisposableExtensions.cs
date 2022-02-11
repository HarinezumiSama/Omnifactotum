#nullable enable

using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System
{
    /// <summary>
    ///     Contains extension methods for <see cref="IDisposable"/> interface.
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeSafely<T>([CanBeNull] this T? disposable)
            where T : struct, IDisposable
            => disposable?.Dispose();
    }
}