#nullable enable

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum
{
    /// <summary>
    ///     The collection of the <see cref="DirectedGraphNode{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the node value.
    /// </typeparam>
    [Serializable]
    public sealed class DirectedGraphNodeCollection<T> : DirectedGraphNodeCollectionBase<T>
    {
        private readonly DirectedGraphNode<T> _owner;
        private readonly DirectedGraphOwnerRelation _ownerRelation;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DirectedGraphNodeCollection{T}"/> class.
        /// </summary>
        /// <param name="owner">
        ///     The node containing this collection.
        /// </param>
        /// <param name="ownerRelation">
        ///     The relation of the owner node to the items contained in this collection.
        /// </param>
        internal DirectedGraphNodeCollection([NotNull] DirectedGraphNode<T> owner, DirectedGraphOwnerRelation ownerRelation)
        {
            if (owner is null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            ownerRelation.EnsureDefined();

            _owner = owner;
            _ownerRelation = ownerRelation;
        }

        /// <inheritdoc />
        [CanBeNull]
        internal override DirectedGraph<T>? Graph
        {
            [DebuggerNonUserCode]
            [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
            get => _owner.Graph;

            [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
            set => _owner.AssignGraph(value);
        }

        /// <inheritdoc />
        protected override void OnItemAdded(DirectedGraphNode<T> item)
        {
            base.OnItemAdded(item);

            GetRelatedCollection(item).AddInternal(_owner);
        }

        /// <inheritdoc />
        protected override void OnItemRemoved(DirectedGraphNode<T> item)
        {
            GetRelatedCollection(item).RemoveInternal(_owner);

            base.OnItemRemoved(item);
        }

        private DirectedGraphNodeCollection<T> GetRelatedCollection([NotNull] DirectedGraphNode<T> item)
            => _ownerRelation switch
            {
                DirectedGraphOwnerRelation.Head => item.Heads,
                DirectedGraphOwnerRelation.Tail => item.Tails,
                _ => throw _ownerRelation.CreateEnumValueNotSupportedException()
            };
    }
}