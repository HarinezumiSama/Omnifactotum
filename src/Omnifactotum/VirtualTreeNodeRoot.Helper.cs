using System.Collections.Generic;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Provides helper functionality for creating instances of the <see cref="VirtualTreeNodeRoot{T}"/> type using type inference in a friendly way.
/// </summary>
public static class VirtualTreeNodeRoot
{
    /// <summary>
    ///     Creates a new instance of the <see cref="VirtualTreeNodeRoot{T}"/> class using the specified value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value.
    /// </typeparam>
    /// <param name="children">
    ///     The children to initialize the <see cref="VirtualTreeNodeRoot{T}"/> instance with. Can be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="VirtualTreeNodeRoot{T}"/> class containing the specified value.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static VirtualTreeNodeRoot<T> Create<T>([CanBeNull] IReadOnlyCollection<VirtualTreeNode<T>>? children = null) => new(children);
}