using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents an abstract virtual tree node, that is, a container that has zero or more children
    ///     of the same type as itself.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the node value.
    /// </typeparam>
    [Serializable]
    public abstract class VirtualTreeNodeBase<T>
    {
        private VirtualTreeNodeCollection<T> _children;

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNodeBase{T}"/> class.
        /// </summary>
        protected VirtualTreeNodeBase()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNodeBase{T}"/> class
        ///     using the specified collection of the child nodes.
        /// </summary>
        /// <param name="children">
        ///     The children to initialize the <see cref="VirtualTreeNodeBase{T}"/> instance with.
        /// </param>
        protected VirtualTreeNodeBase([NotNull] ICollection<VirtualTreeNode<T>> children)
            : this()
        {
            if (children is null)
            {
                throw new ArgumentNullException(nameof(children));
            }

            if (children.Any(item => item is null))
            {
                throw new ArgumentException(@"The collection contains a null element.", nameof(children));
            }

            _children = new VirtualTreeNodeCollection<T>(this, children);
        }

        /// <summary>
        ///     Gets the collection of the child nodes.
        /// </summary>
        [NotNull]
        public VirtualTreeNodeCollection<T> Children
        {
            [DebuggerNonUserCode]
            get
            {
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_children is null)
                {
                    _children = new VirtualTreeNodeCollection<T>(this);
                }

                return _children;
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="VirtualTreeNodeBase{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="VirtualTreeNodeBase{T}"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}. Children.Count = {1}",
                GetType().GetQualifiedName(),
                Children.Count);
        }
    }
}