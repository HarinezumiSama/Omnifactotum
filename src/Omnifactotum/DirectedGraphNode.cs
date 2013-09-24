using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

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
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraphNode{T}"/> class.
        /// </summary>
        /// <param name="value">
        ///     The value associated with the graph node.
        /// </param>
        public DirectedGraphNode(T value)
        {
            this.Heads = new DirectedGraphNodeCollection<T>(this, DirectedGraphOwnerRelation.Tail);
            this.Tails = new DirectedGraphNodeCollection<T>(this, DirectedGraphOwnerRelation.Head);
            this.Value = value;
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

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the graph which this node belongs to.
        /// </summary>
        public DirectedGraph<T> Graph
        {
            get;
            internal set;
        }

        /// <summary>
        ///     Gets or sets the value associated with this <see cref="DirectedGraphNode{T}"/>.
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the collection of the heads of this node, that is, the nodes to which this node is directed to.
        /// </summary>
        public DirectedGraphNodeCollection<T> Heads
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the collection of the tails of this node, that is, the nodes which are directed to this node.
        /// </summary>
        public DirectedGraphNodeCollection<T> Tails
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="DirectedGraphNode{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="DirectedGraphNode{T}"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0}. Value = {1}",
                GetType().GetQualifiedName(),
                this.Value.ToStringSafely());
        }

        #endregion

        #region Internal Methods

        internal void AssignGraph(DirectedGraph<T> graph)
        {
            if (graph == null)
            {
                return;
            }

            Helper.ProcessRecursively(
                this,
                item => item.Heads.Concat(item.Tails),
                obj =>
                {
                    if (obj.Graph == graph)
                    {
                        return RecursiveProcessingDirective.NoRecursionForItem;
                    }

                    if (obj.Graph != null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "The directed graph node {{{0}}} belongs to another graph.",
                                obj));
                    }

                    obj.Graph = graph;
                    if (!graph.Contains(obj))
                    {
                        graph.AddInternal(obj);
                    }

                    return RecursiveProcessingDirective.Continue;
                });
        }

        #endregion
    }
}