using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the root of virtual tree nodes. That is, <see cref="VirtualTreeNodeRoot{T}"/> is just
    ///     the container that has zero or more child nodes.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the node value.
    /// </typeparam>
    [Serializable]
    public sealed class VirtualTreeNodeRoot<T> : VirtualTreeNodeBase<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNodeRoot{T}"/> class.
        /// </summary>
        public VirtualTreeNodeRoot()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNodeRoot{T}"/> class
        ///     using the specified collection of the child nodes.
        /// </summary>
        /// <param name="children">
        ///     The children to initialize the <see cref="VirtualTreeNodeRoot{T}"/> instance with.
        /// </param>
        public VirtualTreeNodeRoot([NotNull] ICollection<VirtualTreeNode<T>> children)
            : base(children)
        {
            // Nothing to do
        }
    }
}