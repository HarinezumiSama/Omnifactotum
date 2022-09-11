using System;
using System.Collections.Generic;
using System.Diagnostics;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

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
        private VirtualTreeNodeBase<T>? _owner;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNode{T}"/> class
        ///     using the specified value and collection of the child nodes.
        /// </summary>
        /// <param name="value">
        ///     A value to initialize the <see cref="VirtualTreeNode{T}"/> instance with.
        /// </param>
        /// <param name="children">
        ///     The children to initialize the <see cref="VirtualTreeNode{T}"/> instance with. Can be <see langword="null"/>.
        /// </param>
        public VirtualTreeNode(T value, [CanBeNull] IReadOnlyCollection<VirtualTreeNode<T>>? children = null)
            : base(children ?? Array.Empty<VirtualTreeNode<T>>())
            => Value = value;

        /// <summary>
        ///     Gets the parent node of the current node, or <see langword="null"/> if the node does not have a parent.
        /// </summary>
        [CanBeNull]
        public VirtualTreeNodeBase<T>? Parent
        {
            [DebuggerStepThrough]
            get => _owner;

            [DebuggerStepThrough]
            internal set => _owner = value;
        }

        /// <summary>
        ///     Gets or sets the value associated with the current node.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="VirtualTreeNode{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="VirtualTreeNode{T}"/>.
        /// </returns>
        public override string ToString()
            => AsInvariant($@"{GetType().GetQualifiedName()}: {nameof(Children)}.{nameof(Children.Count)} = {Children.Count}, {nameof(Value)} = {Value}");
    }
}