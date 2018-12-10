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
        private readonly HashSet<DirectedGraphNode<T>> _items;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraphNodeCollectionBase{T}"/> class.
        /// </summary>
        internal DirectedGraphNodeCollectionBase()
        {
            _items = new HashSet<DirectedGraphNode<T>>();
        }

        /// <inheritdoc />
        public int Count
        {
            [DebuggerStepThrough]
            get => _items.Count;
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get => false;
        }

        /// <inheritdoc />
        //// ReSharper disable once AnnotationConflictInHierarchy
        public void Add([NotNull] DirectedGraphNode<T> item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            AddInternal(item);
            OnItemAdded(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            var removedItems = _items.ToArray();
            ClearInternal();

            foreach (var removedItem in removedItems)
            {
                OnItemRemoved(removedItem);
            }
        }

        /// <inheritdoc />
        public bool Contains(DirectedGraphNode<T> item)
        {
            return item != null && _items.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(DirectedGraphNode<T>[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(DirectedGraphNode<T> item)
        {
            var result = RemoveInternal(item);
            if (result && item != null)
            {
                OnItemRemoved(item);
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerator<DirectedGraphNode<T>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [CanBeNull]
        internal abstract DirectedGraph<T> Graph
        {
            get;
            set;
        }

        internal void ClearInternal()
        {
            _items.Clear();
        }

        internal void AddInternal([NotNull] DirectedGraphNode<T> item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Graph != null && item.Graph != null && item.Graph != Graph)
            {
                throw new ArgumentException("The item is already associated with another graph.", nameof(item));
            }

            if (_items.Contains(item))
            {
                if (item.Graph is null)
                {
                    throw new InvalidOperationException(
                        $@"The node {{{item}}} belongs to the graph but is not associated with this graph.");
                }

                return;
            }

            _items.Add(item);

            var graph = Graph ?? item.Graph;
            if (graph is null)
            {
                return;
            }
            if (item.Graph is null)
            {
                item.AssignGraph(graph);
            }

            if (Graph is null)
            {
                Graph = graph;
            }
        }

        internal bool RemoveInternal([CanBeNull] DirectedGraphNode<T> item)
        {
            return item != null && _items.Remove(item);
        }

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
    }
}