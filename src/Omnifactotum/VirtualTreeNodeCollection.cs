﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     The collection of the <see cref="VirtualTreeNode{T}"/> instances.
/// </summary>
/// <typeparam name="T">
///     The type of the node value.
/// </typeparam>
[Serializable]
public sealed class VirtualTreeNodeCollection<T> : IList<VirtualTreeNode<T>>
{
    private readonly VirtualTreeNodeBase<T> _owner;
    private readonly List<VirtualTreeNode<T>> _list;

    /// <summary>
    ///     Initializes a new instance of the <see cref="VirtualTreeNodeCollection{T}"/> class.
    /// </summary>
    /// <param name="owner">
    ///     The owner of the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </param>
    /// <param name="collection">
    ///     The collection of the nodes to initialize the current collection with.
    /// </param>
    internal VirtualTreeNodeCollection([NotNull] VirtualTreeNodeBase<T> owner, [CanBeNull] IReadOnlyCollection<VirtualTreeNode<T>>? collection = null)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        _list = new List<VirtualTreeNode<T>>(collection?.Count ?? 0);

        collection?.DoForEach(Add);
    }

    /// <summary>
    ///     Gets the number of elements contained in the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </summary>
    public int Count
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [DebuggerStepThrough]
        get => _list.Count;
    }

    /// <summary>
    ///     Gets a value indicating whether the <see cref="VirtualTreeNodeCollection{T}"/> is read-only.
    /// </summary>
    /// <returns>
    ///     The implementation of <see cref="VirtualTreeNodeCollection{T}"/> returns <see langword="false"/>.
    /// </returns>
    public bool IsReadOnly
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [DebuggerStepThrough]
        get => false;
    }

    /// <summary>
    ///     Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">
    ///     The zero-based index of the element to get or set.
    /// </param>
    /// <returns>
    ///     The element at the specified index.
    /// </returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///     <paramref name="index"/> is not a valid index.
    /// </exception>
    [NotNull]
    public VirtualTreeNode<T> this[int index]
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get => _list[index];

        set
        {
            if (index < 0 || index >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $@"The specified index ({index}) is out of range.");
            }

            if (ReferenceEquals(_list[index], value))
            {
                return;
            }

            CheckItemBeingAdded(value);
            _list[index].Parent = null;
            _list[index] = value;
            value.Parent = _owner;
        }
    }

    /// <summary>
    ///     Returns a <see cref="System.String"/> that represents
    ///     this <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.String"/> that represents this <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </returns>
    public override string ToString() => AsInvariant($@"{GetType().GetQualifiedName()}: {nameof(Count)} = {Count}");

    /// <summary>
    ///     Adds the elements of the specified collection to the end of
    ///     this <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </summary>
    /// <param name="collection">
    ///     The collection whose elements should be added
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///     <para><paramref name="collection"/> is <see langword="null"/>.</para>
    ///     <para>-or-</para>
    ///     <para>An item within the specified <paramref name="collection"/> is <see langword="null"/>.</para>
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///     An item within the specified <paramref name="collection"/> already belongs to this or
    ///     another collection.
    /// </exception>
    public void AddRange([NotNull] IEnumerable<VirtualTreeNode<T>> collection)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        collection.DoForEach(Add);
    }

    /// <summary>
    ///     Determines the index of a specific item.
    /// </summary>
    /// <param name="item">
    ///     The object to locate.
    /// </param>
    /// <returns>
    ///     The index of <paramref name="item"/> if found; otherwise, <c>-1</c>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    //// ReSharper disable once AnnotationConflictInHierarchy
    public int IndexOf([NotNull] VirtualTreeNode<T> item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        return _list.IndexOf(item);
    }

    /// <summary>
    ///     Inserts an item to the <see cref="VirtualTreeNodeCollection{T}"/> at the specified index.
    /// </summary>
    /// <param name="index">
    ///     The zero-based index at which <paramref name="item"/> should be inserted.
    /// </param>
    /// <param name="item">
    ///     The object to insert into the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///     <paramref name="index"/> is not a valid index in the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="item"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///     <paramref name="item"/> already belongs to this or another collection.
    /// </exception>
    //// ReSharper disable once AnnotationConflictInHierarchy
    public void Insert(int index, [NotNull] VirtualTreeNode<T> item)
    {
        CheckItemBeingAdded(item);

        _list.Insert(index, item);
        item.Parent = _owner;
    }

    /// <summary>
    ///     Removes the item at the specified index.
    /// </summary>
    /// <param name="index">
    ///     The zero-based index of the item to remove.
    /// </param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///     <paramref name="index"/> is not a valid index in the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </exception>
    public void RemoveAt(int index)
    {
        var item = _list[index];
        _list.RemoveAt(index);
        item.Parent = null;
    }

    /// <summary>
    ///     Adds an item to the end of the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </summary>
    /// <param name="item">
    ///     The object to add to the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="item"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///     <paramref name="item"/> already belongs to this or another collection.
    /// </exception>
    //// ReSharper disable once AnnotationConflictInHierarchy
    public void Add([NotNull] VirtualTreeNode<T> item)
    {
        CheckItemBeingAdded(item);

        _list.Add(item);
        item.Parent = _owner;
    }

    /// <summary>
    ///     Removes all items from the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </summary>
    public void Clear()
    {
        foreach (var item in _list)
        {
            item.Parent = null;
        }

        _list.Clear();
    }

    /// <summary>
    ///     Determines whether the <see cref="VirtualTreeNodeCollection{T}"/> contains the specific item.
    /// </summary>
    /// <param name="item">
    ///     The object to locate in the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="item"/> is found in the <see cref="VirtualTreeNodeCollection{T}"/>;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    //// ReSharper disable once AnnotationConflictInHierarchy
    public bool Contains([NotNull] VirtualTreeNode<T> item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        return _list.Contains(item);
    }

    /// <summary>
    ///     Copies the elements of the current <see cref="VirtualTreeNodeCollection{T}"/> to
    ///     an <see cref="System.Array"/>, starting at a particular array index.
    /// </summary>
    /// <param name="array">
    ///     The one-dimensional <see cref="System.Array"/> that is the destination of the elements
    ///     copied from the current <see cref="VirtualTreeNodeCollection{T}"/>.
    ///     The array must have zero-based indexing.
    /// </param>
    /// <param name="arrayIndex">
    ///     The zero-based index in array at which copying begins.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="array"/> is <see langword="null"/>.
    /// </exception>
    public void CopyTo([NotNull] VirtualTreeNode<T>[] array, int arrayIndex)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        _list.CopyTo(array, arrayIndex);
    }

    /// <summary>
    ///     Removes the first occurrence of the specified item from
    ///     the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </summary>
    /// <param name="item">
    ///     The object to remove from the <see cref="VirtualTreeNodeCollection{T}"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="item"/> was successfully removed from
    ///     the <see cref="VirtualTreeNodeCollection{T}"/>; otherwise, <see langword="false"/>.
    /// </returns>
    //// ReSharper disable once AnnotationConflictInHierarchy
    public bool Remove([NotNull] VirtualTreeNode<T> item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        var result = _list.Remove(item);
        if (result)
        {
            item.Parent = null;
        }

        return result;
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    ///     A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<VirtualTreeNode<T>> GetEnumerator() => _list.GetEnumerator();

    /// <summary>
    ///     Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEnumerator"/> object that can be used to iterate through the collection.
    /// </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void CheckItemBeingAdded([NotNull] VirtualTreeNode<T> item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (item.Parent is null)
        {
            return;
        }

        if (item.Parent == _owner)
        {
            throw new ArgumentException($@"The item {{ {item} }} already belongs to this collection.", nameof(item));
        }

        throw new ArgumentException($@"The item {{ {item} }} already belongs to another collection.", nameof(item));
    }
}