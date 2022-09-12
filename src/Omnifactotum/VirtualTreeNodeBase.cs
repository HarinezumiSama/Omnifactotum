using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

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
    private VirtualTreeNodeCollection<T>? _children;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VirtualTreeNodeBase{T}"/> class
    ///     using the specified collection of the child nodes.
    /// </summary>
    /// <param name="children">
    ///     The children to initialize the <see cref="VirtualTreeNodeBase{T}"/> instance with.
    /// </param>
    private protected VirtualTreeNodeBase([NotNull] IReadOnlyCollection<VirtualTreeNode<T>> children)
    {
        if (children is null)
        {
            throw new ArgumentNullException(nameof(children));
        }

        //// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract :: Argument validation
        if (children.Any(item => item is null))
        {
            throw new ArgumentException(@"The collection contains a null element.", nameof(children));
        }

        _children = children.Count == 0 ? null : new VirtualTreeNodeCollection<T>(this, children);
    }

    /// <summary>
    ///     Gets the collection of the child nodes.
    /// </summary>
    [NotNull]
    public VirtualTreeNodeCollection<T> Children
    {
        [DebuggerNonUserCode]
        get => _children ??= new VirtualTreeNodeCollection<T>(this);
    }

    /// <summary>
    ///     Returns a <see cref="System.String"/> that represents this <see cref="VirtualTreeNodeBase{T}"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.String"/> that represents this <see cref="VirtualTreeNodeBase{T}"/>.
    /// </returns>
    public override string ToString() => AsInvariant($@"{GetType().GetQualifiedName()}: {nameof(Children)}.{nameof(Children.Count)} = {Children.Count}");
}