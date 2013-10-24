﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     The collection of the <see cref="VirtualTreeNode{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the node value.
    /// </typeparam>
    [Serializable]
    public sealed class VirtualTreeNodeCollection<T> : IList<VirtualTreeNode<T>>
    {
        #region Constants and Fields

        private readonly VirtualTreeNodeBase<T> _owner;
        private readonly List<VirtualTreeNode<T>> _list;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNodeCollection{T}"/> class.
        /// </summary>
        /// <param name="owner">
        ///     The owner of the <see cref="VirtualTreeNodeCollection{T}"/>.
        /// </param>
        internal VirtualTreeNodeCollection(VirtualTreeNodeBase<T> owner)
        {
            #region Argument Check

            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            #endregion

            _owner = owner;
            _list = new List<VirtualTreeNode<T>>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="VirtualTreeNodeCollection{T}"/> class
        ///     using the specified collection of the nodes.
        /// </summary>
        /// <param name="owner">
        ///     The node which is the owner of the collection.
        /// </param>
        /// <param name="collection">
        ///     The collection of the nodes to initialize the current collection with.
        /// </param>
        internal VirtualTreeNodeCollection(VirtualTreeNodeBase<T> owner, IEnumerable<VirtualTreeNode<T>> collection)
            : this(owner)
        {
            AddRange(collection);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents
        ///     this <see cref="VirtualTreeNodeCollection{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="VirtualTreeNodeCollection{T}"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}. Count = {1}",
                this.GetType().GetQualifiedName(),
                this.Count);
        }

        /// <summary>
        ///     Adds the elements of the specified collection to the end of
        ///     this <see cref="VirtualTreeNodeCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        ///     The collection whose elements should be added
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <para><paramref name="collection"/> is <b>null</b>.</para>
        ///     <para>-or-</para>
        ///     <para>An item within the specified <paramref name="collection"/> is <b>null</b>.</para>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     An item within the specified <paramref name="collection"/> already belongs to this or
        ///     another collection.
        /// </exception>
        public void AddRange(IEnumerable<VirtualTreeNode<T>> collection)
        {
            #region Argument Check

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            #endregion

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        #endregion

        #region IList<VirtualTreeNode<T>> Members

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
        public VirtualTreeNode<T> this[int index]
        {
            [DebuggerNonUserCode]
            get
            {
                return _list[index];
            }

            [DebuggerNonUserCode]
            set
            {
                #region Argument Check

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (index < 0 || index >= _list.Count)
                {
                    throw new ArgumentOutOfRangeException("index", index, "The specified index is out of range.");
                }

                if (value != _list[index])
                {
                    CheckItemBeingAdded(value);
                }

                #endregion

                _list[index] = value;
            }
        }

        /// <summary>
        ///     Determines the index of a specific item.
        /// </summary>
        /// <param name="item">
        ///     The object to locate.
        /// </param>
        /// <returns>
        ///     The index of <paramref name="item"/> if found; otherwise, <b>-1</b>.
        /// </returns>
        public int IndexOf(VirtualTreeNode<T> item)
        {
            return item == null ? -1 : _list.IndexOf(item);
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
        ///     <paramref name="item"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="item"/> already belongs to this or another collection.
        /// </exception>
        public void Insert(int index, VirtualTreeNode<T> item)
        {
            #region Argument Check

            CheckItemBeingAdded(item);

            #endregion

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

        #endregion

        #region ICollection<VirtualTreeNode<T>> Members

        /// <summary>
        ///     Gets the number of elements contained in the <see cref="VirtualTreeNodeCollection{T}"/>.
        /// </summary>
        public int Count
        {
            [DebuggerStepThrough]
            get { return _list.Count; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="VirtualTreeNodeCollection{T}"/> is read-only.
        /// </summary>
        /// <returns>
        ///     The implementation of <see cref="VirtualTreeNodeCollection{T}"/> returns <b>false</b>.
        /// </returns>
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get { return false; }
        }

        /// <summary>
        ///     Adds an item to the end of the <see cref="VirtualTreeNodeCollection{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to add to the <see cref="VirtualTreeNodeCollection{T}"/>.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="item"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="item"/> already belongs to this or another collection.
        /// </exception>
        public void Add(VirtualTreeNode<T> item)
        {
            #region Argument Check

            CheckItemBeingAdded(item);

            #endregion

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
        ///     <b>true</b> if <paramref name="item"/> is found in the <see cref="VirtualTreeNodeCollection{T}"/>;
        ///     otherwise, <b>false</b>.
        /// </returns>
        public bool Contains(VirtualTreeNode<T> item)
        {
            return item != null && _list.Contains(item);
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
        ///     <paramref name="array"/> is <b>null</b>.
        /// </exception>
        public void CopyTo(VirtualTreeNode<T>[] array, int arrayIndex)
        {
            #region Argument Check

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            #endregion

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
        ///     <b>true</b> if <paramref name="item"/> was successfully removed from
        ///     the <see cref="VirtualTreeNodeCollection{T}"/>; otherwise, <b>false</b>.
        /// </returns>
        public bool Remove(VirtualTreeNode<T> item)
        {
            if (item == null)
            {
                return false;
            }

            bool result = _list.Remove(item);
            if (result)
            {
                item.Parent = null;
            }

            return result;
        }

        #endregion

        #region IEnumerable<VirtualTreeNode<T>> Members

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<VirtualTreeNode<T>> GetEnumerator()
        {
            return _list.GetEnumerator();
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

        #region Private Methods

        private void CheckItemBeingAdded(VirtualTreeNode<T> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            if (item.Parent == null)
            {
                return;
            }

            if (item.Parent == _owner)
            {
                throw new ArgumentException("The item already belongs to this collection.", "item");
            }

            throw new ArgumentException("The item already belongs to another collection.", "item");
        }

        #endregion
    }
}