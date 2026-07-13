using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Collections.Immutable;

/// <summary>
///     Contains extension methods for <see cref="ImmutableArray{T}"/>.
/// </summary>
public static class OmnifactotumImmutableArrayExtensions
{
    /// <summary>
    ///     Returns the specified <see cref="ImmutableArray{T}"/> if it is initialized
    ///     (that is, if <see cref="ImmutableArray{T}.IsDefault"/> is <see langword="false"/>);
    ///     otherwise (that is, if <see cref="ImmutableArray{T}.IsDefault"/> is <see langword="true"/>),
    ///     returns <see cref="ImmutableArray{T}.Empty"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the immutable array.
    /// </typeparam>
    /// <param name="source">
    ///     The immutable array to handle.
    /// </param>
    /// <returns>
    ///     The specified <see cref="ImmutableArray{T}"/> if it is initialized
    ///     (that is, if <see cref="ImmutableArray{T}.IsDefault"/> is <see langword="false"/>);
    ///     otherwise (that is, if <see cref="ImmutableArray{T}.IsDefault"/> is <see langword="true"/>),
    ///     returns <see cref="ImmutableArray{T}.Empty"/>.
    /// </returns>
    /// <seealso cref="ImmutableArray{T}.IsDefault"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ImmutableArray<T> EmptyIfDefault<T>([NoEnumeration] this ImmutableArray<T> source)
        => source is { IsDefault: true } ? ImmutableArray<T>.Empty : source;

    /// <summary>
    ///     Returns the specified <see cref="ImmutableArray{T}"/> if it is not <see langword="null"/> and is initialized
    ///     (that is, <see cref="ImmutableArray{T}.IsDefault"/> is <see langword="false"/>); otherwise, returns <see cref="ImmutableArray{T}.Empty"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the immutable array.
    /// </typeparam>
    /// <param name="source">
    ///     The nullable immutable array to handle.
    /// </param>
    /// <returns>
    ///     The specified <see cref="ImmutableArray{T}"/> if it is not <see langword="null"/> and is initialized
    ///     (that is, <see cref="ImmutableArray{T}.IsDefault"/> is <see langword="false"/>); otherwise, returns <see cref="ImmutableArray{T}.Empty"/>.
    /// </returns>
    /// <seealso cref="ImmutableArray{T}.IsDefault"/>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ImmutableArray<T> EmptyIfNullOrDefault<T>([CanBeNull] [NoEnumeration] this ImmutableArray<T>? source)
        => source is { IsDefault: false } array ? array : ImmutableArray<T>.Empty;
}