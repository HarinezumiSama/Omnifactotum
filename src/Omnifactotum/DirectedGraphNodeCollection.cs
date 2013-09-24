using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     The collection of the <see cref="DirectedGraphNode{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the node value.
    /// </typeparam>
    [Serializable]
    [DebuggerDisplay("{GetType().Name,nq}. Count = {Count}")]
    public sealed class DirectedGraphNodeCollection<T> : DirectedGraphNodeCollectionBase<T>
    {
        #region Constants and Fields

        private readonly DirectedGraphNode<T> _owner;
        private readonly DirectedGraphOwnerRelation _ownerRelation;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraphNodeCollection{T}"/> class.
        /// </summary>
        /// <param name="owner">
        ///     The node containing this collection.
        /// </param>
        /// <param name="ownerRelation">
        ///     The relation of the owner node to the items contained in this collection.
        /// </param>
        internal DirectedGraphNodeCollection(DirectedGraphNode<T> owner, DirectedGraphOwnerRelation ownerRelation)
        {
            #region Argument Check

            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            ownerRelation.EnsureDefined();

            #endregion

            _owner = owner;
            _ownerRelation = ownerRelation;
        }

        #endregion

        #region Internal Properties

        internal override DirectedGraph<T> Graph
        {
            [DebuggerNonUserCode]
            get
            {
                return _owner.Graph;
            }

            set
            {
                _owner.AssignGraph(value);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Called right after an item has been added to this collection.
        /// </summary>
        /// <param name="item">
        ///     The item that has been added.
        /// </param>
        protected override void OnItemAdded(DirectedGraphNode<T> item)
        {
            base.OnItemAdded(item);

            GetRelatedCollection(item).AddInternal(_owner);
        }

        /// <summary>
        ///     Called right after an item has been removed from this collection.
        /// </summary>
        /// <param name="item">
        ///     The item that has been removed.
        /// </param>
        protected override void OnItemRemoved(DirectedGraphNode<T> item)
        {
            GetRelatedCollection(item).RemoveInternal(_owner);

            base.OnItemRemoved(item);
        }

        #endregion

        #region Private Methods

        private DirectedGraphNodeCollection<T> GetRelatedCollection(DirectedGraphNode<T> item)
        {
            switch (_ownerRelation)
            {
                case DirectedGraphOwnerRelation.Head:
                    return item.Heads;

                case DirectedGraphOwnerRelation.Tail:
                    return item.Tails;

                default:
                    throw new NotSupportedException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The case for enumeration value {0} is not supported.",
                            _ownerRelation.GetQualifiedName()));
            }
        }

        #endregion
    }
}