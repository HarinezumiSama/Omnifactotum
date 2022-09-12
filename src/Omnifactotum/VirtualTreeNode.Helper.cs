using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Provides helper functionality for creating instances of the <see cref="VirtualTreeNode{T}"/> type using type inference in a friendly way.
/// </summary>
public static class VirtualTreeNode
{
    /// <summary>
    ///     Creates a new instance of the <see cref="VirtualTreeNode{T}"/> class using the specified value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value.
    /// </typeparam>
    /// <param name="value">
    ///     The value to initialize <see cref="VirtualTreeNode{T}"/> with.
    /// </param>
    /// <param name="children">
    ///     The children to initialize the <see cref="VirtualTreeNode{T}"/> instance with. Can be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="VirtualTreeNode{T}"/> class containing the specified value.
    /// </returns>
    public static VirtualTreeNode<T> Create<T>(T value, [CanBeNull] IReadOnlyCollection<VirtualTreeNode<T>>? children = null) => new(value, children);
}