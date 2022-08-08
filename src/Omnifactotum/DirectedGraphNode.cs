#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

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
        public DirectedGraphNode(T value)
        {
            Heads = new DirectedGraphNodeCollection<T>(this, DirectedGraphOwnerRelation.Tail);
            Tails = new DirectedGraphNodeCollection<T>(this, DirectedGraphOwnerRelation.Head);
            Value = value;
        }

        /// <summary>
        ///     Gets the graph which this node belongs to.
        /// </summary>
        [CanBeNull]
        public DirectedGraph<T>? Graph { get; private set; }

        /// <summary>
        ///     Gets or sets the value associated with this <see cref="DirectedGraphNode{T}"/>.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        ///     Gets the collection of the heads of this node, that is, the nodes to which this node is directed to.
        /// </summary>
        [NotNull]
        public DirectedGraphNodeCollection<T> Heads { get; }

        /// <summary>
        ///     Gets the collection of the tails of this node, that is, the nodes which are directed to this node.
        /// </summary>
        [NotNull]
        public DirectedGraphNodeCollection<T> Tails { get; }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="DirectedGraphNode{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="DirectedGraphNode{T}"/>.
        /// </returns>
        public override string ToString() => AsInvariant($@"{GetType().GetQualifiedName()}: {nameof(Value)} = {Value.ToStringSafelyInvariant()}");

        internal void AssignGraph([CanBeNull] DirectedGraph<T>? graph)
        {
            if (graph is null)
            {
                return;
            }

            Factotum.ProcessRecursively(this, GetItems, ProcessItem);

            IEnumerable<DirectedGraphNode<T>> GetItems(DirectedGraphNode<T> item) => item.Heads.Concat(item.Tails);

            RecursiveProcessingDirective ProcessItem(DirectedGraphNode<T> node)
            {
                if (node.Graph == graph)
                {
                    return RecursiveProcessingDirective.NoRecursionForItem;
                }

                if (node.Graph != null)
                {
                    throw new InvalidOperationException(AsInvariant($@"The directed graph node {{ {node} }} belongs to another graph."));
                }

                node.Graph = graph;
                if (!graph.Contains(node))
                {
                    graph.AddInternal(node);
                }

                return RecursiveProcessingDirective.Continue;
            }
        }

        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
        internal void ResetGraph() => Graph = null;

        private string ToDebuggerString() => ToString();
    }
}