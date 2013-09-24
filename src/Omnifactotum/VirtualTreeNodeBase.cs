using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

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
        #region Constants and Fields

        private VirtualTreeNodeCollection<T> _children;

        #endregion

        #region Constructors

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
        protected VirtualTreeNodeBase(IEnumerable<VirtualTreeNode<T>> children)
            : this()
        {
            #region Argument Check

            if (children == null)
            {
                throw new ArgumentNullException("children");
            }

            #endregion

            var childrenArray = children.ToArraySmart();

            #region Argument Check

            if (childrenArray.Any(item => item == null))
            {
                throw new ArgumentException("The collection contains a null element.", "children");
            }

            #endregion

            _children = new VirtualTreeNodeCollection<T>(this, childrenArray);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the collection of the child nodes.
        /// </summary>
        public VirtualTreeNodeCollection<T> Children
        {
            [DebuggerNonUserCode]
            get
            {
                if (_children == null)
                {
                    _children = new VirtualTreeNodeCollection<T>(this);
                }

                return _children;
            }
        }

        #endregion

        #region Public Methods

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
                this.GetType().GetQualifiedName(),
                this.Children.Count);
        }

        #endregion
    }
}