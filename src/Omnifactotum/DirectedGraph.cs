using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the directed graph.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of values associated with graph nodes.
    /// </typeparam>
    [Serializable]
    public sealed class DirectedGraph<T> : DirectedGraphNodeCollectionBase<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraph{T}"/> class.
        /// </summary>
        public DirectedGraph()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraph{T}"/> class.
        /// </summary>
        /// <param name="nodes">
        ///     The collection of nodes to initialize the <see cref="DirectedGraph{T}"/> with.
        /// </param>
        public DirectedGraph([NotNull] IEnumerable<DirectedGraphNode<T>> nodes)
            : this()
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            nodes.DoForEach(Add);
        }

        /// <summary>
        ///     Creates a node with the specified value and adds the created node to this graph.
        /// </summary>
        /// <param name="nodeValue">
        ///     The value that will be associated with the created and added node.
        /// </param>
        /// <returns>
        ///     A created node.
        /// </returns>
        public DirectedGraphNode<T> AddNode([CanBeNull] T nodeValue)
        {
            var node = new DirectedGraphNode<T>(nodeValue);
            Add(node);
            return node;
        }

        /// <summary>
        ///     Gets nodes of this graph sorted topologically, leaving the source graph and its nodes unaffected.
        /// </summary>
        /// <param name="compareEquipollentNodes">
        ///     A reference to a method that is called to compare equipollent nodes, or <c>null</c> to sort
        ///     equipollent nodes by their values, using the default comparer for the type <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     An array of the nodes sorted topologically.
        /// </returns>
        public DirectedGraphNode<T>[] SortTopologically(
            [CanBeNull] Comparison<DirectedGraphNode<T>> compareEquipollentNodes)
        {
            var internalNodeMap = new Dictionary<DirectedGraphNode<T>, InternalNode<T>>(Count);

            foreach (var node in this)
            {
                Factotum.ProcessRecursively(
                    node,
                    item => item.Heads.Concat(item.Tails),
                    item =>
                    {
                        if (internalNodeMap.ContainsKey(item))
                        {
                            return RecursiveProcessingDirective.NoRecursionForItem;
                        }

                        internalNodeMap.Add(item, new InternalNode<T>(item));
                        return RecursiveProcessingDirective.Continue;
                    });
            }

            foreach (var internalNode in internalNodeMap.Values)
            {
                var internalNodeCopy = internalNode;
                internalNode.Node.Heads.DoForEach(item => internalNodeCopy.Heads.Add(internalNodeMap[item]));
                internalNode.Node.Tails.DoForEach(item => internalNodeCopy.Tails.Add(internalNodeMap[item]));
            }

            var resultList = new List<DirectedGraphNode<T>>(Count);
            var comparer = new NodeComparer<T>(compareEquipollentNodes);

            var remainingNodes = internalNodeMap.Values.ToList();
            while (remainingNodes.Any())
            {
                var candidate = remainingNodes
                    .Where(item => item.Tails.Count == 0)
                    .OrderBy(item => item.Node, comparer)
                    .FirstOrDefault();
                if (candidate == null)
                {
                    throw new InvalidOperationException(
                        "Topological sorting cannot be performed since the graph has a cycle.");
                }

                resultList.Add(candidate.Node);

                remainingNodes.DoForEach(item => item.Tails.Remove(candidate));
                remainingNodes.Remove(candidate);
            }

            return resultList.ToArray();
        }

        /// <summary>
        ///     Gets the nodes of this graph sorted topologically, remaining the source graph and its nodes unaffected.
        ///     Equipollent nodes are sorted by their values, using the default comparer for
        ///     the type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        ///     An array of the nodes sorted topologically.
        /// </returns>
        public DirectedGraphNode<T>[] SortTopologically()
        {
            return SortTopologically(null);
        }

        internal override DirectedGraph<T> Graph
        {
            [DebuggerStepThrough]
            get
            {
                return this;
            }

            set
            {
                if (value != null && value != this)
                {
                    throw new InvalidOperationException("Internal error: the graph cannot be changed.");
                }
            }
        }

        /// <summary>
        ///     Called right after an item has been removed from this collection.
        /// </summary>
        /// <param name="item">
        ///     The item that has been removed.
        /// </param>
        protected override void OnItemRemoved(DirectedGraphNode<T> item)
        {
            base.OnItemRemoved(item);

            item.Graph = null;
            this.DoForEach(obj => obj.Tails.Remove(item));
            this.DoForEach(obj => obj.Heads.Remove(item));
        }

        private sealed class InternalNode<TValue>
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="InternalNode{TValue}"/> class.
            /// </summary>
            public InternalNode(DirectedGraphNode<TValue> node)
            {
                Node = node.EnsureNotNull();
                Heads = new List<InternalNode<TValue>>(node.Heads.Count);
                Tails = new List<InternalNode<TValue>>(node.Tails.Count);
            }

            public DirectedGraphNode<TValue> Node
            {
                get;
                private set;
            }

            public List<InternalNode<TValue>> Heads
            {
                get;
                private set;
            }

            public List<InternalNode<TValue>> Tails
            {
                get;
                private set;
            }
        }

        private sealed class NodeComparer<TValue> : IComparer<DirectedGraphNode<TValue>>
        {
            private readonly Comparison<DirectedGraphNode<TValue>> _comparison;

            /// <summary>
            ///     Initializes a new instance of the <see cref="NodeComparer{TValue}"/> class.
            /// </summary>
            public NodeComparer([CanBeNull] Comparison<DirectedGraphNode<TValue>> comparison)
            {
                _comparison = comparison ?? ((x, y) => Comparer<TValue>.Default.Compare(x.Value, y.Value));
            }

            public int Compare(DirectedGraphNode<TValue> x, DirectedGraphNode<TValue> y)
            {
                return _comparison(x, y);
            }
        }
    }
}