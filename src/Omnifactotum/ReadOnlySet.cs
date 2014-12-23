using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents a generic read-only set.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the set.
    /// </typeparam>
    [Serializable]
    public sealed class ReadOnlySet<T> : ISet<T>
    {
        #region Constants and Fields

        private const string ReadOnlyMessage = "The set is read-only and cannot be modified.";

        private readonly ISet<T> _set;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlySet{T}"/> class that is a
        ///     read-only wrapper of the specified set.
        /// </summary>
        /// <param name="set">
        ///     The set to wrap.
        /// </param>
        public ReadOnlySet([NotNull] ISet<T> set)
        {
            #region Argument Check

            if (set == null)
            {
                throw new ArgumentNullException("set");
            }

            #endregion

            _set = set;
        }

        #endregion

        #region ISet<T> Members

        /// <summary>
        ///     Adds an element to the current set and returns a value to indicate if the
        ///     element was successfully added.
        /// </summary>
        /// <param name="item">
        ///     The element to add to the set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the element is added to the set; <c>false</c> if the element is already in the set.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        bool ISet<T>.Add([CanBeNull] T item)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Modifies the current set so that it contains only elements that are present
        ///     either in the current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Modifies the current set so that it contains all elements that are present
        ///     in both the current set and in the specified collection.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Determines whether the current set is a property (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current set is a correct subset of <paramref name="other"/>;
        ///     otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="other"/> is <c>null</c>.
        /// </exception>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return _set.IsProperSubsetOf(other);
        }

        /// <summary>
        ///     Determines whether the current set is a correct superset of a specified collection.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current set is a correct superset of <paramref name="other"/>;
        ///     otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="other"/> is <c>null</c>.
        /// </exception>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return _set.IsProperSupersetOf(other);
        }

        /// <summary>
        ///     Determines whether the current set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current set is a subset of <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="other"/> is <c>null</c>.
        /// </exception>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return _set.IsSubsetOf(other);
        }

        /// <summary>
        ///     Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current set is a superset of <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="other"/> is <c>null</c>.
        /// </exception>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return _set.IsSupersetOf(other);
        }

        /// <summary>
        ///     Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current set and other share at least one common element; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="other"/> is <c>null</c>.
        /// </exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            return _set.Overlaps(other);
        }

        /// <summary>
        ///     Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">
        ///     The collection to compare to the current set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current set is equal to <paramref name="other"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="other"/> is <c>null</c>.
        /// </exception>
        public bool SetEquals(IEnumerable<T> other)
        {
            return _set.SetEquals(other);
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        ///     Gets the number of elements contained in the current set.
        /// </summary>
        public int Count
        {
            get
            {
                return _set.Count;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the current set is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        void ICollection<T>.Add([CanBeNull] T item)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Removes all items from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the current set.
        /// </summary>
        /// <param name="item">
        ///     The object to remove from the current set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item"/> was successfully removed from the current set;
        ///     otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="item"/> is not found
        ///     in the current set.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///     The set is read-only.
        /// </exception>
        bool ICollection<T>.Remove([CanBeNull] T item)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Determines whether the current set contains a specific value.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the set.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item"/> is found in the set; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains([CanBeNull] T item)
        {
            return _set.Contains(item);
        }

        /// <summary>
        ///     Copies the elements of the current set to an
        ///     <see cref="System.Array"/>, starting at a particular <see cref="System.Array"/> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="System.Array"/> that is the destination of the elements
        ///     copied from the current ser. The <see cref="System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in array at which copying begins.
        /// </param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
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
            return _set.GetEnumerator();
        }

        #endregion
    }
}