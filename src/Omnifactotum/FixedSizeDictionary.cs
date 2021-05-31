using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

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
        private static readonly FixedSizeDictionaryDeterminant<TKey> Determinant = new SafeDeterminant();

        private readonly DictionaryValueHolder[] _items;
        private int _version;

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
        ///     <paramref name="dictionary"/> is <c>null</c>.
        /// </exception>
        public FixedSizeDictionary(
            //// ReSharper disable once ParameterTypeCanBeEnumerable.Local :: By design
            [NotNull] IDictionary<TKey, TValue> dictionary)
            : this()
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

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
        ///     <paramref name="dictionary"/> is <c>null</c>.
        /// </exception>
        public FixedSizeDictionary([NotNull] FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            : this(dictionary.EnsureNotNull()._items.Copy())
        {
            if (_items.Length != Determinant.Size)
            {
                throw new InvalidOperationException("Invalid item array length in the source dictionary.");
            }

            Count = dictionary.Count;
        }

        private FixedSizeDictionary(DictionaryValueHolder[] items)
        {
            _items = items.EnsureNotNull();

            Keys = new KeyCollection(this);
            Values = new ValueCollection(this);
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{TKey}" /> containing the keys of
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        public ICollection<TKey> Keys { get; }

        /// <summary>
        ///     Gets an <see cref="ICollection{TValue}" /> containing the values of
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        public ICollection<TValue> Values { get; }

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
        ///     <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        ///     An element with the specified key was not found.
        /// </exception>
        public TValue this[TKey key]
        {
            get
            {
                var found = TryGetValue(key, out var result);
                if (!found)
                {
                    throw new KeyNotFoundException(
                        AsInvariant($@"The specified key was not present in the dictionary (key: {key})."));
                }

                return result;
            }

            set => SetItemInternal(key, value, true);
        }

        /// <summary>
        ///     Gets the number of elements contained in
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
        /// </summary>
        public int Count
        {
            [DebuggerStepThrough]
            get;

            [DebuggerStepThrough]
            private set;
        }

        /// <summary>
        ///     Gets a value indicating whether
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> is read-only.
        /// </summary>
        public bool IsReadOnly => false;

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
        ///     <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     An element with the same key already exists in
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
        /// </exception>
        public void Add(TKey key, [CanBeNull] TValue value)
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
        ///     <c>true</c> if the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> contains an element
        ///     with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(TKey key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

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
        ///     <c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(TKey key)
        {
            var index = Determinant.GetIndex(key);

            var result = _items[index].IsSet;
            _version++;
            _items[index] = new DictionaryValueHolder();

            if (result)
            {
                Count--;
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
        ///     <c>true</c> if the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> contains an element
        ///     with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetValue(
            //// ReSharper disable once CommentTypo :: ReSharper term :)
            //// ReSharper disable once AnnotationRedundanceInHierarchy :: To emphasize
            [NotNull] TKey key,
            [CanBeNull] out TValue value)
        {
            var index = Determinant.GetIndex(key);
            var item = _items[index];

            var result = item.IsSet;
            value = result ? item.Value : default;
            return result;
        }

        /// <summary>
        ///     Adds an item to the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">
        ///     The object to add to the <see cref="ICollection{T}"/>.
        /// </param>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

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

            Count = 0;
        }

        /// <summary>
        ///     Determines whether the <see cref="ICollection{T}"/> contains the specified value.
        /// </summary>
        /// <param name="item">
        ///     The object to locate in the <see cref="ICollection{T}"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="item"/> is found in the <see cref="ICollection{T}"/>;
        ///     otherwise, <c>false</c>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            => TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);

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
        ///     <paramref name="array"/> is <c>null</c>.
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
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(arrayIndex),
                    arrayIndex,
                    @"The value cannot be negative.");
            }

            if (arrayIndex + Count > array.Length)
            {
                throw new ArgumentException(
                    "The number of elements is greater than the available space from the array index to the end"
                        + " of the destination array.");
            }

            var currentArrayIndex = arrayIndex;
            for (var index = 0; index < _items.Length; index++)
            {
                var item = _items[index];
                if (!item.IsSet)
                {
                    continue;
                }

                var key = Determinant.GetKey(index);
                array[currentArrayIndex] = OmnifactotumKeyValuePair.Create(key, item.Value);
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
        ///     <c>true</c> if <paramref name="item"/> was successfully removed from the <see cref="ICollection{T}"/>;
        ///     otherwise, <c>false</c>.
        /// </returns>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => TryGetValue(item.Key, out var value)
                && EqualityComparer<TValue>.Default.Equals(value, item.Value)
                && Remove(item.Key);

        /// <summary>
        ///     Returns an enumerator that iterates through
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator{T}" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

        /// <summary>
        ///     Returns an enumerator that iterates through
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>..
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void SetItemInternal([NotNull] TKey key, [CanBeNull] TValue value, bool replaceExisting)
        {
            var index = Determinant.GetIndex(key);
            var previousItem = _items[index];

            if (previousItem.IsSet && !replaceExisting)
            {
                throw new ArgumentException(
                    AsInvariant($@"An element with the same key already exists (key: {key})."),
                    nameof(key));
            }

            _version++;
            _items[index] = new DictionaryValueHolder { IsSet = true, Value = value };

            if (!previousItem.IsSet)
            {
                Count++;
            }
        }

        private sealed class SafeDeterminant : FixedSizeDictionaryDeterminant<TKey>
        {
            private readonly TDeterminant _determinant;

            internal SafeDeterminant()
            {
                _determinant = new TDeterminant();

                var determinantSize = _determinant.Size;
                if (determinantSize <= 0)
                {
                    throw new InvalidOperationException(
                        AsInvariant(
                            $@"The determinant {typeof(TDeterminant).GetFullName().ToUIString()} returned invalid size {
                                determinantSize}. The size must be positive."));
                }

                Size = determinantSize;
            }

            public override int Size
            {
                [DebuggerStepThrough]
                get;
            }

            public override int GetIndex(TKey key)
            {
                var index = _determinant.GetIndex(key);
                return index;
            }

            public override TKey GetKey(int index)
            {
                return _determinant.GetKey(index);
            }
        }

        private sealed class KeyCollection : ICollection<TKey>
        {
            private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;

            internal KeyCollection(FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            {
                _dictionary = dictionary.EnsureNotNull();
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly => true;

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
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                return _dictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (array is null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(arrayIndex),
                        arrayIndex,
                        @"The value cannot be negative.");
                }

                if (arrayIndex + Count > array.Length)
                {
                    throw new ArgumentException(
                        "The number of elements is greater than the available space from the array index to the end"
                            + " of the destination array.");
                }

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

            public IEnumerator<TKey> GetEnumerator()
            {
                return _dictionary.Select(pair => pair.Key).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class ValueCollection : ICollection<TValue>
        {
            private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;

            internal ValueCollection(FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            {
                _dictionary = dictionary.EnsureNotNull();
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly => true;

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
                if (array is null)
                {
                    throw new ArgumentNullException(nameof(array));
                }

                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(arrayIndex),
                        arrayIndex,
                        @"The value cannot be negative.");
                }

                if (arrayIndex + Count > array.Length)
                {
                    throw new ArgumentException(
                        "The number of elements is greater than the available space from the array index to the end"
                            + " of the destination array.");
                }

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

            public IEnumerator<TValue> GetEnumerator()
            {
                return _dictionary.Select(pair => pair.Value).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;
            private readonly int _initialVersion;
            private readonly int _size;
            private readonly int _initialCount;
            private int _index;

            internal Enumerator([NotNull] FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
            {
                _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

                _initialVersion = dictionary._version;
                _size = Determinant.Size;
                _initialCount = dictionary.Count;

                ResetInternal();
            }

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

            public void Dispose()
            {
                // Nothing to do
            }

            object IEnumerator.Current => Current;

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

            private void ResetInternal()
            {
                _index = -1;
            }

            private void EnsureVersionConsistency()
            {
                if (_dictionary._version != _initialVersion || _dictionary.Count != _initialCount)
                {
                    throw new InvalidOperationException("Cannot enumerate the modified collection.");
                }
            }
        }

        private struct DictionaryValueHolder
        {
            public bool IsSet { get; set; }

            public TValue Value { get; set; }
        }
    }
}