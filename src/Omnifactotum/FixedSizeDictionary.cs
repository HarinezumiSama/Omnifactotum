using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents a generic dictionary which is internally handled by a fixed size array due to limitation for
    ///     the maximum number of possible keys.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the values in the dictionary.
    /// </typeparam>
    /// <typeparam name="TDeterminant">
    ///     The type of the determinant. See <see cref="FixedSizeDictionaryDeterminant{TKey}"/>.
    /// </typeparam>
    public class FixedSizeDictionary<TKey, TValue, TDeterminant> : IDictionary<TKey, TValue>
        where TDeterminant : FixedSizeDictionaryDeterminant<TKey>, new()
    {
        #region Constants and Fields

        private static readonly FixedSizeDictionaryDeterminant<TKey> Determinant = new SafeDeterminant();

        private readonly DictionaryValueHolder[] _items;
        private int _count;
        private int _version;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FixedSizeDictionary{TKey, TValue, TDeterminant}"/> class.
        /// </summary>
        public FixedSizeDictionary()
            : this(new DictionaryValueHolder[Determinant.Size])
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FixedSizeDictionary{TKey, TValue, TDeterminant}"/> class
        ///     by copying the key/values pairs from the specified dictionary.
        /// </summary>
        /// <param name="dictionary">
        ///     The dictionary to copy the key/values pairs from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dictionary"/> is <b>null</b>.
        /// </exception>
        //// ReSharper disable once ParameterTypeCanBeEnumerable.Local - By design
        public FixedSizeDictionary([NotNull] IDictionary<TKey, TValue> dictionary)
            : this()
        {
            #region Argument Check

            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            #endregion

            foreach (var pair in dictionary)
            {
                Add(pair.Key, pair.Value);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FixedSizeDictionary{TKey, TValue, TDeterminant}"/> class
        ///     by copying the key/values pairs from the specified dictionary.
        /// </summary>
        /// <param name="dictionary">
        ///     The dictionary to copy the key/values pairs from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dictionary"/> is <b>null</b>.
        /// </exception>
        public FixedSizeDictionary([NotNull] FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            : this(dictionary.EnsureNotNull()._items.Copy())
        {
            if (_items.Length != Determinant.Size)
            {
                throw new InvalidOperationException("Invalid item array length in the source dictionary.");
            }

            _count = dictionary._count;
        }

        private FixedSizeDictionary(DictionaryValueHolder[] items)
        {
            _items = items.EnsureNotNull();

            this.Keys = new KeyCollection(this);
            this.Values = new ValueCollection(this);
        }

        #endregion

        #region IDictionary<TKey, TValue> Members: Properties

        /// <summary>
        ///     Gets an <see cref="ICollection{TKey}" /> containing the keys of
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{TValue}" /> containing the values of
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        public ICollection<TValue> Values
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key of the element to get or set.
        /// </param>
        /// <returns>
        ///     The element with the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="key"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        ///     An element with the specified key was not found.
        /// </exception>
        public TValue this[[NotNull] TKey key]
        {
            get
            {
                TValue result;
                var found = TryGetValue(key, out result);
                if (!found)
                {
                    throw new KeyNotFoundException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The specified key was not present in the dictionary (key: {0}).",
                            key));
                }

                return result;
            }

            set
            {
                SetItemInternal(key, value, true);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> Members: Properties

        /// <summary>
        ///     Gets the number of elements contained in
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        public int Count
        {
            [DebuggerStepThrough]
            get
            {
                return _count;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region IDictionary<TKey, TValue> Members: Methods

        /// <summary>
        ///     Adds an element with the specified key and value to
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
        /// </summary>
        /// <param name="key">
        ///     The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        ///     The object to use as the value of the element to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="key"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     An element with the same key already exists in
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
        /// </exception>
        public void Add([NotNull] TKey key, [CanBeNull] TValue value)
        {
            SetItemInternal(key, value, false);
        }

        /// <summary>
        ///     Determines whether the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> contains
        ///     an element with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to locate in the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> contains an element
        ///     with the specified key; otherwise, <b>false</b>.
        /// </returns>
        public bool ContainsKey([NotNull] TKey key)
        {
            var index = Determinant.GetIndex(key);
            var item = _items[index];
            return item.IsSet;
        }

        /// <summary>
        ///     Removes the element with the specified key from
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        /// <param name="key">
        ///     The key of the element to remove.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the element is successfully removed; otherwise, <b>false</b>.
        /// </returns>
        public bool Remove([NotNull] TKey key)
        {
            var index = Determinant.GetIndex(key);

            var result = _items[index].IsSet;
            _version++;
            _items[index] = new DictionaryValueHolder();

            if (result)
            {
                _count--;
            }

            return result;
        }

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key whose value to get.
        /// </param>
        /// <param name="value">
        ///     When this method returns, the value associated with the specified key, if the key is found;
        ///     otherwise, the default value for the type of the <paramref name="value" /> parameter.
        ///     This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> contains an element
        ///     with the specified key; otherwise, <b>false</b>.
        /// </returns>
        // ReSharper disable once AnnotationRedundanceInHierarchy - To emphasize
        public bool TryGetValue([NotNull] TKey key, [CanBeNull] out TValue value)
        {
            var index = Determinant.GetIndex(key);
            var item = _items[index];

            var result = item.IsSet;
            value = result ? item.Value : default(TValue);
            return result;
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> Members: Methods

        /// <summary>
        ///     Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///     Removes all items from the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        public void Clear()
        {
            _version++;

            for (var index = 0; index < _items.Length; index++)
            {
                _items[index] = new DictionaryValueHolder();
            }

            _count = 0;
        }

        /// <summary>
        ///     Determines whether the <see cref="ICollection{T}"/> contains the specified value.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        /// <returns>
        ///     <b>true</b> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>;
        ///     otherwise, <b>false</b>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            return TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <summary>
        ///     Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="Array"/>,
        ///     starting at a particular <see cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        ///     The one-dimensional <see cref="Array"/> that is the destination of the elements copied
        ///     from <see cref="ICollection{T}"/>. The <see cref="Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        ///     The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="array"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The number of elements in the source <see cref="ICollection{T}"/> is greater than the available
        ///     space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
            KeyValuePair<TKey, TValue>[] array,
            int arrayIndex)
        {
            #region Argument Check

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "arrayIndex",
                    arrayIndex,
                    @"The value cannot be negative.");
            }

            if (arrayIndex + this.Count > array.Length)
            {
                throw new ArgumentException(
                    "The number of elements is greater than the available space from the array index to the end"
                        + " of the destination array.");
            }

            #endregion

            var currentArrayIndex = arrayIndex;
            for (var index = 0; index < _items.Length; index++)
            {
                var item = _items[index];
                if (!item.IsSet)
                {
                    continue;
                }

                var key = Determinant.GetKey(index);
                array[currentArrayIndex] = KeyValuePair.Create(key, item.Value);
                currentArrayIndex++;
            }
        }

        /// <summary>
        ///     Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to remove from the <see cref="ICollection{T}"/>.
        /// </param>
        /// <returns>
        ///     <b>true</b> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>;
        ///     otherwise, <b>false</b>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (!TryGetValue(item.Key, out value) || !EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                return false;
            }

            return Remove(item.Key);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey, TValue>> Members

        /// <summary>
        ///     Returns an enumerator that iterates through
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///     Returns an enumerator that iterates through
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>..
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private Methods

        private void SetItemInternal([NotNull] TKey key, [CanBeNull] TValue value, bool replaceExisting)
        {
            var index = Determinant.GetIndex(key);
            var previousItem = _items[index];

            if (previousItem.IsSet)
            {
                if (!replaceExisting)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "An element with the same key already exists (key: {0}).",
                            key),
                        "key");
                }
            }

            _version++;
            _items[index] = new DictionaryValueHolder { IsSet = true, Value = value };

            if (!previousItem.IsSet)
            {
                _count++;
            }
        }

        #endregion

        #region SafeDeterminant Class

        private sealed class SafeDeterminant : FixedSizeDictionaryDeterminant<TKey>
        {
            #region Constants and Fields

            private readonly TDeterminant _determinant;
            private readonly int _size;

            #endregion

            #region Constructors

            internal SafeDeterminant()
            {
                _determinant = new TDeterminant();
                _size = _determinant.Size;

                if (_size <= 0)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The determinant '{0}' return invalid size {1}. The size must be positive.",
                            typeof(TDeterminant).GetFullName(),
                            _size));
                }
            }

            #endregion

            #region Public Properties

            public override int Size
            {
                [DebuggerStepThrough]
                get
                {
                    return _size;
                }
            }

            #endregion

            #region Public Methods

            public override int GetIndex(TKey key)
            {
                var index = _determinant.GetIndex(key);
                return index;
            }

            public override TKey GetKey(int index)
            {
                return _determinant.GetKey(index);
            }

            #endregion
        }

        #endregion

        #region KeyCollection Class

        private sealed class KeyCollection : ICollection<TKey>
        {
            #region Constants and Fields

            private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;

            #endregion

            #region Constructors

            internal KeyCollection(FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            {
                _dictionary = dictionary.EnsureNotNull();
            }

            #endregion

            #region ICollection<TKey> Members

            public int Count
            {
                get
                {
                    return _dictionary.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TKey item)
            {
                return _dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                #region Constants and Fields

                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "arrayIndex",
                        arrayIndex,
                        @"The value cannot be negative.");
                }

                if (arrayIndex + this.Count > array.Length)
                {
                    throw new ArgumentException(
                        "The number of elements is greater than the available space from the array index to the end"
                            + " of the destination array.");
                }

                #endregion

                var index = arrayIndex;
                foreach (var item in this)
                {
                    array[index] = item;
                    index++;
                }
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IEnumerable<TKey> Members

            public IEnumerator<TKey> GetEnumerator()
            {
                return _dictionary.Select(pair => pair.Key).GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region ValueCollection Class

        private sealed class ValueCollection : ICollection<TValue>
        {
            #region Constants and Fields

            private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;

            #endregion

            #region Constructors

            internal ValueCollection(FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            {
                _dictionary = dictionary.EnsureNotNull();
            }

            #endregion

            #region ICollection<TValue> Members

            public int Count
            {
                get
                {
                    return _dictionary.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TValue item)
            {
                return _dictionary._items.Any(
                    obj => obj.IsSet && EqualityComparer<TValue>.Default.Equals(obj.Value, item));
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                #region Constants and Fields

                if (array == null)
                {
                    throw new ArgumentNullException("array");
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "arrayIndex",
                        arrayIndex,
                        @"The value cannot be negative.");
                }

                if (arrayIndex + this.Count > array.Length)
                {
                    throw new ArgumentException(
                        "The number of elements is greater than the available space from the array index to the end"
                            + " of the destination array.");
                }

                #endregion

                var index = arrayIndex;
                foreach (var item in this)
                {
                    array[index] = item;
                    index++;
                }
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IEnumerable<TValue> Members

            public IEnumerator<TValue> GetEnumerator()
            {
                return _dictionary.Select(pair => pair.Value).GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region Enumerator Class

        private sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            #region Constants and Fields

            private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;
            private readonly int _initialVersion;
            private readonly int _size;
            private readonly int _initialCount;
            private int _index;

            #endregion

            #region Constructors

            internal Enumerator([NotNull] FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            {
                #region Argument Check

                if (dictionary == null)
                {
                    throw new ArgumentNullException("dictionary");
                }

                #endregion

                _dictionary = dictionary;
                _initialVersion = dictionary._version;
                _size = Determinant.Size;
                _initialCount = dictionary._count;
                ResetInternal();
            }

            #endregion

            #region IEnumerator<KeyValuePair<TKey,TValue>> Members

            public KeyValuePair<TKey, TValue> Current
            {
                get
                {
                    EnsureVersionConsistency();

                    if (_index < 0 || _index >= _size)
                    {
                        throw new InvalidOperationException("The enumerator is not positioned properly.");
                    }

                    var item = _dictionary._items[_index];
                    if (!item.IsSet)
                    {
                        throw new InvalidOperationException("Internal logic error.");
                    }

                    return new KeyValuePair<TKey, TValue>(Determinant.GetKey(_index), item.Value);
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                // Nothing to do
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public bool MoveNext()
            {
                EnsureVersionConsistency();

                if (_index >= _size)
                {
                    return false;
                }

                while (++_index < _size)
                {
                    var item = _dictionary._items[_index];
                    if (item.IsSet)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                ResetInternal();
            }

            #endregion

            #region Private Methods

            private void ResetInternal()
            {
                _index = -1;
            }

            private void EnsureVersionConsistency()
            {
                if (_dictionary._version != _initialVersion || _dictionary._count != _initialCount)
                {
                    throw new InvalidOperationException("Cannot enumerate the modified collection.");
                }
            }

            #endregion
        }

        #endregion

        #region DictionaryValueHolder Class

        internal struct DictionaryValueHolder
        {
            #region Public Properties

            public bool IsSet
            {
                get;
                set;
            }

            public TValue Value
            {
                get;
                set;
            }

            #endregion
        }

        #endregion
    }
}