#nullable enable

using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the root of virtual tree nodes. That is, <see cref="VirtualTreeNodeRoot{T}"/> is just the container that has zero or more child nodes.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the node value.
    /// </typeparam>
    [Serializable]
    public sealed class VirtualTreeNodeRoot<T> : VirtualTreeNodeBase<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNodeRoot{T}"/> class using the specified collection of the child nodes.
        /// </summary>
        /// <param name="children">
        ///     The children to initialize the <see cref="VirtualTreeNodeRoot{T}"/> instance with. Can be <see langword="null"/>.
        /// </param>
        public VirtualTreeNodeRoot([CanBeNull] IReadOnlyCollection<VirtualTreeNode<T>>? children = null)
            : base(children ?? Array.Empty<VirtualTreeNode<T>>())
        {
            // Nothing to do
        }
    }
}