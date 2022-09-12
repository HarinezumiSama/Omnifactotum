using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

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
        if (nodes is null)
        {
            throw new ArgumentNullException(nameof(nodes));
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
    public DirectedGraphNode<T> AddNode(T nodeValue)
    {
        var node = new DirectedGraphNode<T>(nodeValue);
        Add(node);
        return node;
    }

    /// <summary>
    ///     Gets nodes of this graph sorted topologically, leaving the source graph and its nodes unaffected.
    /// </summary>
    /// <param name="compareEquipollentNodes">
    ///     A reference to a method that is called to compare equipollent nodes, or <see langword="null"/> to sort
    ///     equipollent nodes by their values, using the default comparer for the type <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    ///     An array of the nodes sorted topologically.
    /// </returns>
    public DirectedGraphNode<T>[] SortTopologically([CanBeNull] Comparison<DirectedGraphNode<T>?>? compareEquipollentNodes = null)
    {
        var internalNodeMap = new Dictionary<DirectedGraphNode<T>, InternalNode<T>>(Count);

        IEnumerable<DirectedGraphNode<T>> GetItems(DirectedGraphNode<T> item) => item.Heads.Concat(item.Tails);

        RecursiveProcessingDirective ProcessItem(DirectedGraphNode<T> item)
        {
            if (internalNodeMap.ContainsKey(item))
            {
                return RecursiveProcessingDirective.NoRecursionForItem;
            }

            internalNodeMap.Add(item, new InternalNode<T>(item));
            return RecursiveProcessingDirective.Continue;
        }

        foreach (var node in this)
        {
            Factotum.ProcessRecursively(node, GetItems, ProcessItem);
        }

        foreach (var internalNode in internalNodeMap.Values)
        {
            var internalNodeCopy = internalNode;
            internalNode.Node.Tails.DoForEach(item => internalNodeCopy.Tails.Add(internalNodeMap[item]));
        }

        var resultList = new List<DirectedGraphNode<T>>(Count);
        var comparer = new NodeComparer<T>(compareEquipollentNodes);

        var remainingNodes = internalNodeMap.Values.ToList();
        while (remainingNodes.Count != 0)
        {
            var candidate = remainingNodes
                .Where(item => item.Tails.Count == 0)
                .OrderBy(item => item.Node, comparer)
                .FirstOrDefault();

            if (candidate is null)
            {
                throw new InvalidOperationException("Topological sorting cannot be performed since the graph has a cycle.");
            }

            resultList.Add(candidate.Node);

            remainingNodes.DoForEach(item => item.Tails.Remove(candidate));
            remainingNodes.Remove(candidate);
        }

        return resultList.ToArray();
    }

    internal override DirectedGraph<T>? Graph
    {
        [DebuggerStepThrough]
        get => this;

        set
        {
            if (value != this)
            {
                throw new InvalidOperationException($@"Internal error: the graph cannot be changed for {GetType().GetQualifiedName().ToUIString()}.");
            }
        }
    }

    /// <inheritdoc />
    protected override void OnItemRemoved(DirectedGraphNode<T> item)
    {
        base.OnItemRemoved(item);

        item.ResetGraph();
        this.DoForEach(obj => obj.Tails.Remove(item));
        this.DoForEach(obj => obj.Heads.Remove(item));
    }

    private sealed class InternalNode<TValue>
    {
        public InternalNode(DirectedGraphNode<TValue> node)
        {
            Node = node.EnsureNotNull();
            Tails = new List<InternalNode<TValue>>(node.Tails.Count);
        }

        public DirectedGraphNode<TValue> Node { get; }

        public List<InternalNode<TValue>> Tails { get; }
    }

    private sealed class NodeComparer<TValue> : IComparer<DirectedGraphNode<TValue>>
    {
        private static readonly Comparison<DirectedGraphNode<TValue>?> DefaultCompareNodesMethod = DefaultCompareNodes;

        private readonly Comparison<DirectedGraphNode<TValue>?> _comparison;

        public NodeComparer([CanBeNull] Comparison<DirectedGraphNode<TValue>?>? comparison) => _comparison = comparison ?? DefaultCompareNodesMethod;

        public int Compare(DirectedGraphNode<TValue>? left, DirectedGraphNode<TValue>? right) => _comparison(left, right);

        private static int DefaultCompareNodes(DirectedGraphNode<TValue>? left, DirectedGraphNode<TValue>? right)
            => ReferenceEquals(left, right)
                ? OmnifactotumConstants.ComparisonResult.Equal
                : left is null
                    ? OmnifactotumConstants.ComparisonResult.LessThan
                    : right is null
                        ? OmnifactotumConstants.ComparisonResult.GreaterThan
                        : Comparer<TValue>.Default.Compare(left.Value, right.Value);
    }
}