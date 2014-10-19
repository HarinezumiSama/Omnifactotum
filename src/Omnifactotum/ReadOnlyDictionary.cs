using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    //// TODO [vmcl] This custom ReadOnlyDictionary implementation is not needed in FW 4.5+

    /// <summary>
    ///     Represents a read-only wrapper for the <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of values in the dictionary.
    /// </typeparam>
    public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        #region Constants and Fields

        /// <summary>
        ///     The read-only message.
        /// </summary>
        private const string ReadOnlyMessage = "The instance cannot be modified as it is read-only.";

        /// <summary>
        ///     The wrapped dictionary.
        /// </summary>
        private readonly IDictionary<TKey, TValue> _dictionary;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">
        ///     The original dictionary to wrap.
        /// </param>
        public ReadOnlyDictionary([NotNull] IDictionary<TKey, TValue> dictionary)
        {
            #region Argument Check

            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            #endregion

            _dictionary = dictionary;
        }

        #endregion

        #region IDictionary<TKey, TValue> Members

        /// <summary>
        ///     Gets a collection containing the keys of the current dictionary.
        /// </summary>
        /// <returns>
        ///     A collection containing the keys of the current dictionary.
        /// </returns>
        public ICollection<TKey> Keys
        {
            [DebuggerStepThrough]
            get
            {
                return _dictionary.Keys;
            }
        }

        /// <summary>
        ///     Gets a collection containing the values in the current dictionary.
        /// </summary>
        /// <returns>
        ///     A collection containing the values in the current dictionary.
        /// </returns>
        public ICollection<TValue> Values
        {
            [DebuggerStepThrough]
            get
            {
                return _dictionary.Values;
            }
        }

        /// <summary>
        ///     Gets or sets the element with the specified key.
        ///     <para>
        ///         <b>NOTE.</b>
        ///         Property setter is not supported by <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
        ///     </para>
        /// </summary>
        /// <param name="key">
        ///     The key of the element to get or set.
        /// </param>
        /// <returns>
        ///     The element with the specified key.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///     The property is set and the current dictionary is read-only.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="key"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        ///     The property is retrieved and key is not found.
        /// </exception>
        public TValue this[[NotNull] TKey key]
        {
            [DebuggerNonUserCode]
            get
            {
                return _dictionary[key];
            }

            [DebuggerNonUserCode]
            set
            {
                throw new NotSupportedException(ReadOnlyMessage);
            }
        }

        /// <summary>
        ///     Adds an element with the provided key and value to the current dictionary.
        ///     <para>
        ///         <b>NOTE.</b>
        ///         This method is not supported by <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
        ///     </para>
        /// </summary>
        /// <param name="key">
        ///     The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        ///     The object to use as the value of the element to add.
        /// </param>
        [DebuggerNonUserCode]
        void IDictionary<TKey, TValue>.Add([NotNull] TKey key, TValue value)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Removes the element with the specified key from the current dictionary.
        ///     <para>
        ///         <b>NOTE.</b>
        ///         This method is not supported by <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
        ///     </para>
        /// </summary>
        /// <param name="key">
        ///     The key of the element to remove.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the element is successfully removed; otherwise, <b>false</b>.
        /// </returns>
        [DebuggerNonUserCode]
        bool IDictionary<TKey, TValue>.Remove([NotNull] TKey key)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Determines whether the current dictionary contains an element with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to locate in the current dictionary.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the current dictionary contains an element
        ///     with the specified key; otherwise, <b>false</b>.
        /// </returns>
        [DebuggerNonUserCode]
        public bool ContainsKey([NotNull] TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key whose value to get.
        /// </param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if
        ///     the key is found; otherwise, the default value for the type of the value
        ///     parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the current dictionary contains an element with the specified key;
        ///     otherwise, <b>false</b>.
        /// </returns>
        [DebuggerNonUserCode]
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        ///     Gets the number of elements contained in the current collection.
        /// </summary>
        /// <returns>
        ///     The number of elements contained in the current collection.
        /// </returns>
        public int Count
        {
            [DebuggerNonUserCode]
            get
            {
                return _dictionary.Count;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the current collection is read-only.
        /// </summary>
        /// <returns>
        ///     <b>true</b> if the current collection is read-only; otherwise, <b>false</b>.
        /// </returns>
        public bool IsReadOnly
        {
            [DebuggerStepThrough]
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Adds an item to the current collection.
        ///     <para>
        ///         <b>NOTE.</b>
        ///         This method is not supported by <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
        ///     </para>
        /// </summary>
        /// <param name="item">
        ///     The object to add to the current collection.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        ///     The current collection is read-only.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Removes all items from the current collection.
        ///     <para>
        ///         <b>NOTE.</b>
        ///         This method is not supported by <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
        ///     </para>
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        ///     The current collection is read-only.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the current collection.
        ///     <para>
        ///         <b>NOTE.</b>
        ///         This method is not supported by <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
        ///     </para>
        /// </summary>
        /// <param name="item">
        ///     The object to remove from the current collection.
        /// </param>
        /// <returns>
        ///     <b>true</b> if item was successfully removed from the current collection; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///     The current collection is read-only.
        /// </exception>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException(ReadOnlyMessage);
        }

        /// <summary>
        ///     Determines whether the current collection contains a specific value.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the current collection.
        /// </param>
        /// <returns>
        ///     <b>true</b> if item is found in the current collection; otherwise, <b>false</b>.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        /// <summary>
        ///     Copies the elements of the current collection to an <see cref="System.Array"/>,
        ///     starting at a particular array index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="System.Array"/> that is the destination of the elements
        ///     copied from the current collection. The array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in array at which copying begins.
        /// </param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Collections.Generic.IEnumerator{T}"/> that can be used to iterate through
        ///     the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through
        ///     the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion
    }
}