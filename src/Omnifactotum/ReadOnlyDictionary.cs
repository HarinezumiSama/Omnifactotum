#if NET40

using System;
using System.Collections;
using System.Collections.Generic;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents a read-only wrapper for the <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of values in the dictionary.
    /// </typeparam>
    [Serializable]
    public sealed class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        [NonSerialized]
        private ReadOnlyItemCollection<TKey> _keys;

        [NonSerialized]
        private ReadOnlyItemCollection<TValue> _values;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">
        ///     The original dictionary to wrap.
        /// </param>
        public ReadOnlyDictionary([NotNull] IDictionary<TKey, TValue> dictionary)
            => _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        /// <summary>
        ///     Gets a collection containing the keys of the current dictionary.
        /// </summary>
        /// <returns>
        ///     A collection containing the keys of the current dictionary.
        /// </returns>
        public ReadOnlyItemCollection<TKey> Keys => _keys ??= new ReadOnlyItemCollection<TKey>(_dictionary.Keys);

        /// <summary>
        ///     Gets a collection containing the keys of the current dictionary.
        /// </summary>
        /// <returns>
        ///     A collection containing the keys of the current dictionary.
        /// </returns>
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        ///     Gets a collection containing the values in the current dictionary.
        /// </summary>
        /// <returns>
        ///     A collection containing the values in the current dictionary.
        /// </returns>
        public ReadOnlyItemCollection<TValue> Values => _values ??= new ReadOnlyItemCollection<TValue>(_dictionary.Values);

        /// <summary>
        ///     Gets a collection containing the values in the current dictionary.
        /// </summary>
        /// <returns>
        ///     A collection containing the values in the current dictionary.
        /// </returns>
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        ///     Gets the element with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key of the element to get.
        /// </param>
        /// <returns>
        ///     The element with the specified key.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        ///     The property is retrieved and key is not found.
        /// </exception>
        public TValue this[TKey key] => _dictionary[key];

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
        ///     <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        ///     The property is retrieved and key is not found.
        /// </exception>
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => _dictionary[key];

            set => throw CreateReadOnlyInstanceException();
        }

        /// <summary>
        ///     Gets the number of elements contained in the current collection.
        /// </summary>
        /// <returns>
        ///     The number of elements contained in the current collection.
        /// </returns>
        public int Count => _dictionary.Count;

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
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw CreateReadOnlyInstanceException();

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
        ///     <see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>.
        /// </returns>
        bool IDictionary<TKey, TValue>.Remove(TKey key) => throw CreateReadOnlyInstanceException();

        /// <summary>
        ///     Determines whether the current dictionary contains an element with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to locate in the current dictionary.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the current dictionary contains an element
        ///     with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

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
        ///     <see langword="true"/> if the current dictionary contains an element with the specified key;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        /// <summary>
        ///     Gets a value indicating whether the current collection is read-only.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the current collection is read-only; otherwise, <see langword="false"/>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

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
            => throw CreateReadOnlyInstanceException();

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
        void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw CreateReadOnlyInstanceException();

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
        ///     <see langword="true"/> if item was successfully removed from the current collection; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///     The current collection is read-only.
        /// </exception>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => throw CreateReadOnlyInstanceException();

        /// <summary>
        ///     Determines whether the current collection contains a specific value.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the current collection.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if item is found in the current collection; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

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
            => _dictionary.CopyTo(array, arrayIndex);

        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Collections.Generic.IEnumerator{T}"/> that can be used to iterate through
        ///     the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.Collections.IEnumerator"/> object that can be used to iterate through
        ///     the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        private NotSupportedException CreateReadOnlyInstanceException()
            => new(AsInvariant($@"The {GetType().GetQualifiedName()} instance cannot be modified because it is read-only."));
    }
}

#endif