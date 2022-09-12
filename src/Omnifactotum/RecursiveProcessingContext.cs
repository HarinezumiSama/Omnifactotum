using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

namespace Omnifactotum;

/// <summary>
///     Represents the context of the recursive processing.
/// </summary>
/// <typeparam name="T">
///     The types of the instances being processed recursively.
/// </typeparam>
[PublicAPI]
public sealed class RecursiveProcessingContext<T>
{
    private static readonly Func<IEqualityComparer<T>?, HashSet<T>?> CreateHashSetMethod = RecursiveProcessingContext.GenerateCreateHashSetMethod<T>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="RecursiveProcessingContext{T}"/> class
    ///     using the specified equality comparer for the instances being processed recursively.
    /// </summary>
    /// <param name="equalityComparer">
    ///     The equality comparer to use for eliminating duplicated instances,
    ///     or <see langword="null"/> to use <see cref="ByReferenceEqualityComparer{T}"/>.
    /// </param>
    /// <seealso cref="ByReferenceEqualityComparer{T}"/>
    public RecursiveProcessingContext([CanBeNull] IEqualityComparer<T>? equalityComparer = null)
        => ItemsBeingProcessed = CreateHashSetMethod(equalityComparer);

    [CanBeNull]
    internal HashSet<T>? ItemsBeingProcessed { get; }
}