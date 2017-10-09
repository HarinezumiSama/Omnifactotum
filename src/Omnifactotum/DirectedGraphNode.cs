using System;
using System.Globalization;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents a node in a directed graph.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value associated with the graph node.
    /// </typeparam>
    [Serializable]
    public sealed class DirectedGraphNode<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraphNode{T}"/> class.
        /// </summary>
        /// <param name="value">
        ///     The value associated with the graph node.
        /// </param>
        public DirectedGraphNode([CanBeNull] T value)
        {
            Heads = new DirectedGraphNodeCollection<T>(this, DirectedGraphOwnerRelation.Tail);
            Tails = new DirectedGraphNodeCollection<T>(this, DirectedGraphOwnerRelation.Head);
            Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraphNode{T}"/> class
        ///     using the default value for the type <typeparamref name="T"/>.
        /// </summary>
        public DirectedGraphNode()
            : this(default(T))
        {
            // Nothing to do
        }

        /// <summary>
        ///     Gets the graph which this node belongs to.
        /// </summary>
        [CanBeNull]
        public DirectedGraph<T> Graph
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets or sets the value associated with this <see cref="DirectedGraphNode{T}"/>.
        /// </summary>
        [CanBeNull]
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the collection of the heads of this node, that is, the nodes to which this node is directed to.
        /// </summary>
        [NotNull]
        public DirectedGraphNodeCollection<T> Heads
        {
            get;
        }

        /// <summary>
        ///     Gets the collection of the tails of this node, that is, the nodes which are directed to this node.
        /// </summary>
        [NotNull]
        public DirectedGraphNodeCollection<T> Tails
        {
            get;
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="DirectedGraphNode{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="DirectedGraphNode{T}"/>.
        /// </returns>
        public override string ToString() => $@"{GetType().GetQualifiedName()}. Value = {Value.ToStringSafely()}";

        internal void AssignGraph([CanBeNull] DirectedGraph<T> graph)
        {
            if (graph == null)
            {
                return;
            }

            Factotum.ProcessRecursively(
                this,
                item => item.Heads.Concat(item.Tails),
                node =>
                {
                    if (node.Graph == graph)
                    {
                        return RecursiveProcessingDirective.NoRecursionForItem;
                    }

                    if (node.Graph != null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "The directed graph node {{{0}}} belongs to another graph.",
                                node));
                    }

                    node.Graph = graph;
                    if (!graph.Contains(node))
                    {
                        graph.AddInternal(node);
                    }

                    return RecursiveProcessingDirective.Continue;
                });
        }
    }
}