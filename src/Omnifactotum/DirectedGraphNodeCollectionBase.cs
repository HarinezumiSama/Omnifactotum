using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

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
    public abstract class DirectedGraphNodeCollectionBase<T> : ICollection<DirectedGraphNode<T>>
    {
        #region Constants and Fields

        private readonly HashSet<DirectedGraphNode<T>> _items;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraphNodeCollectionBase{T}"/> class.
        /// </summary>
        internal DirectedGraphNodeCollectionBase()
        {
            _items = new HashSet<DirectedGraphNode<T>>();
        }

        #endregion

        #region ICollection<DirectedGraphNode<T>> Members

        /// <summary>
        ///     Gets the number of elements contained in this collection.
        /// </summary>
        public int Count
        {
            [DebuggerStepThrough]
            get { return _items.Count; }
        }

        /// <summary>
        ///     Gets a value indicating whether the this collection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        ///     Adds an item to this collection.
        /// </summary>
        /// <param name="item">
        ///     An item to add to this collection.
        /// </param>
        public void Add([NotNull] DirectedGraphNode<T> item)
        {
            AddInternal(item);
            OnItemAdded(item);
        }

        /// <summary>
        ///     Removes all items from this collection.
        /// </summary>
        public void Clear()
        {
            var removedItems = _items.ToArray();
            ClearInternal();

            foreach (var removedItem in removedItems)
            {
                OnItemRemoved(removedItem);
            }
        }

        /// <summary>
        ///     Determines whether this collection contains the specified item.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in this collection.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item"/> is found in this collection; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains([CanBeNull] DirectedGraphNode<T> item)
        {
            return item != null && _items.Contains(item);
        }

        /// <summary>
        ///     Copies the elements of this collection to an <see cref="System.Array"/>,
        ///     starting at a particular array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="System.Array"/> that is the destination of the elements
        ///     copied from this collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in array at which copying begins.
        /// </param>
        public void CopyTo(DirectedGraphNode<T>[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from this collection.
        /// </summary>
        /// <param name="item">
        ///     The object to remove from this collection.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item"/> was removed from this collection;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public bool Remove([CanBeNull] DirectedGraphNode<T> item)
        {
            var result = RemoveInternal(item);
            if (result && item != null)
            {
                OnItemRemoved(item);
            }

            return result;
        }

        #endregion

        #region IEnumerable<DirectedGraphNode<T>> Members

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{TNode}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DirectedGraphNode<T>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Internal Properties

        [CanBeNull]
        internal abstract DirectedGraph<T> Graph
        {
            get;
            set;
        }

        #endregion

        #region Internal Methods

        internal void ClearInternal()
        {
            _items.Clear();
        }

        internal void AddInternal([NotNull] DirectedGraphNode<T> item)
        {
            #region Argument Check

            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (this.Graph != null && item.Graph != null && item.Graph != this.Graph)
            {
                throw new ArgumentException("The item is already associated with another graph.", "item");
            }

            #endregion

            if (_items.Contains(item))
            {
                if (item.Graph == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "The node {{{0}}} belongs to the graph but is not associated with this graph.",
                            item));
                }

                return;
            }

            _items.Add(item);

            var graph = this.Graph ?? item.Graph;
            if (graph != null)
            {
                if (item.Graph == null)
                {
                    item.AssignGraph(graph);
                }

                if (this.Graph == null)
                {
                    this.Graph = graph;
                }
            }
        }

        internal bool RemoveInternal([CanBeNull] DirectedGraphNode<T> item)
        {
            return item != null && _items.Remove(item);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Called right after an item has been added to this collection.
        /// </summary>
        /// <param name="item">
        ///     The item that has been added.
        /// </param>
        protected virtual void OnItemAdded([NotNull] DirectedGraphNode<T> item)
        {
            // Nothing to do; for overriding only
        }

        /// <summary>
        ///     Called right after an item has been removed from this collection.
        /// </summary>
        /// <param name="item">
        ///     The item that has been removed.
        /// </param>
        protected virtual void OnItemRemoved([NotNull] DirectedGraphNode<T> item)
        {
            // Nothing to do; for overriding only
        }

        #endregion
    }
}