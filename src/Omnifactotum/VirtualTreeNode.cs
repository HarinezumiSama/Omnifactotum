using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the virtual tree node, that is, a container that has a value associated with it,
    ///     zero or more child nodes, and, optionally, a parent node.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the node value.
    /// </typeparam>
    [Serializable]
    public sealed class VirtualTreeNode<T> : VirtualTreeNodeBase<T>
    {
        private VirtualTreeNodeBase<T> _owner;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNode{T}"/> class.
        /// </summary>
        public VirtualTreeNode()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNode{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     A value to initialize the <see cref="VirtualTreeNode{T}"/> instance with.
        /// </param>
        public VirtualTreeNode([CanBeNull] T value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNode{T}"/> class
        ///     using the specified collection of the child nodes.
        /// </summary>
        /// <param name="children">
        ///     The children to initialize the <see cref="VirtualTreeNode{T}"/> instance with.
        /// </param>
        public VirtualTreeNode([NotNull] ICollection<VirtualTreeNode<T>> children)
            : base(children)
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNode{T}"/> class
        ///     using the specified value and collection of the child nodes.
        /// </summary>
        /// <param name="value">
        ///     A value to initialize the <see cref="VirtualTreeNode{T}"/> instance with.
        /// </param>
        /// <param name="children">
        ///     The children to initialize the <see cref="VirtualTreeNode{T}"/> instance with.
        /// </param>
        public VirtualTreeNode([CanBeNull] T value, [NotNull] ICollection<VirtualTreeNode<T>> children)
            : this(children)
        {
            Value = value;
        }

        /// <summary>
        ///     Gets the parent node of the current node, or <c>null</c> if the node does not have a parent.
        /// </summary>
        [CanBeNull]
        public VirtualTreeNodeBase<T> Parent
        {
            [DebuggerStepThrough]
            get
            {
                return _owner;
            }

            [DebuggerStepThrough]
            internal set
            {
                _owner = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value associated with the current node.
        /// </summary>
        [CanBeNull]
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="VirtualTreeNode{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="VirtualTreeNode{T}"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}. Children.Count = {1}, Value = {2}",
                GetType().GetQualifiedName(),
                Children.Count,
                Value);
        }
    }
}