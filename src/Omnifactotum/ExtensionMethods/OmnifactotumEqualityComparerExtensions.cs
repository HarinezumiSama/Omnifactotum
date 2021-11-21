#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Collections.Generic
{
    /// <summary>
    ///     Contains extension methods for the <see cref="IEqualityComparer{T}"/> interface.
    /// </summary>
    [SuppressMessage("ReSharper", "RedundantNullnessAttributeWithNullableReferenceTypes", Justification = "Multiple target frameworks.")]
    public static class OmnifactotumEqualityComparerExtensions
    {
        /// <summary>
        ///     Gets a hash code of the specified value safely, that is, a <see langword="null"/> value does not cause an exception.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a hash code of.
        /// </typeparam>
        /// <param name="equalityComparer">
        ///     The equality comparer to use for getting a hash code of the value.
        /// </param>
        /// <param name="value">
        ///     The value to get a hash code of. Can be <see langword="null"/>.
        /// </param>
        /// <param name="nullValueHashCode">
        ///     The value to return if the <paramref name="value"/> parameter is <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     A hash code of the specified value obtained by calling <see cref="IEqualityComparer{T}.GetHashCode(T)"/> for this value
        ///     if it is not <see langword="null"/>; otherwise, the value specified by the <paramref name="nullValueHashCode"/>
        ///     parameter.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static int GetHashCodeSafely<T>([NotNull] this IEqualityComparer<T> equalityComparer, T value, int nullValueHashCode = 0)
            => equalityComparer is null
                ? throw new ArgumentNullException(nameof(equalityComparer))
                : value is null
                    ? nullValueHashCode
                    : equalityComparer.GetHashCode(value);
    }
}