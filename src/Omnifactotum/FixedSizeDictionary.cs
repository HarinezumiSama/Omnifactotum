using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
using static Omnifactotum.FormattableStringFactotum;

#if NET5_0_OR_GREATER
using MaybeNullWhenAttribute = System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

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
[DebuggerDisplay("{ToDebuggerString(),nq}")]
public class FixedSizeDictionary<TKey, TValue, TDeterminant> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    where TDeterminant : FixedSizeDictionaryDeterminant<TKey>, new()
{
    private static readonly TDeterminant Determinant = new();

    private readonly DictionaryValueHolder[] _items;
    private int _version;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FixedSizeDictionary{TKey, TValue, TDeterminant}"/> class.
    /// </summary>
    public FixedSizeDictionary()
        : this(new DictionaryValueHolder[Determinant.ValidatedSize])
    {
        // Nothing to do
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FixedSizeDictionary{TKey, TValue, TDeterminant}"/> class
    ///     by copying the key/values pairs from the specified collection.
    /// </summary>
    /// <param name="pairs">
    ///     The collection of the key/values pairs to copy from.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="pairs"/> is <see langword="null"/>.
    /// </exception>
    public FixedSizeDictionary([NotNull] IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        : this()
    {
        if (pairs is null)
        {
            throw new ArgumentNullException(nameof(pairs));
        }

        foreach (var pair in pairs)
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
    ///     <paramref name="dictionary"/> is <see langword="null"/>.
    /// </exception>
    public FixedSizeDictionary([NotNull] FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary)
        : this(dictionary.EnsureNotNull()._items.Copy())
    {
        var validatedSize = Determinant.ValidatedSize;
        if (_items.Length != validatedSize)
        {
            throw new InvalidOperationException($@"Invalid item array length in the source dictionary: {_items.Length}. Expected: {validatedSize}.");
        }

        Count = dictionary.Count;
    }

    private FixedSizeDictionary([NotNull] DictionaryValueHolder[] items)
    {
        _items = items.EnsureNotNull();

        Keys = new KeyCollection(this);
        Values = new ValueCollection(this);
    }

    /// <inheritdoc />
    [Pure]
    public ICollection<TKey> Keys
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get;
    }

    /// <inheritdoc />
    [Pure]
    public ICollection<TValue> Values
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get;
    }

    /// <inheritdoc cref="IDictionary{TKey,TValue}.this" />
    public TValue this[TKey key]
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get
        {
            var found = TryGetValue(key, out var result);
            if (!found)
            {
                throw new KeyNotFoundException(AsInvariant($@"The specified key is not found in the dictionary (key: {key})."));
            }

            return result!;
        }

        set => SetItemInternal(key, value, true);
    }

    /// <summary>
    ///     Gets the number of elements contained in
    ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" />.
    /// </summary>
    public int Count
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [DebuggerStepThrough]
        get;
        [DebuggerStepThrough]
        private set;
    }

    /// <summary>
    ///     Gets a value indicating whether
    ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}" /> is read-only.
    /// </summary>
    [Pure]
    public bool IsReadOnly
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get => false;
    }

    /// <inheritdoc />
    [Pure]
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get => Keys;
    }

    /// <inheritdoc />
    [Pure]
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get => Values;
    }

    /// <inheritdoc />
    public void Add(TKey key, TValue value)
    {
        SetItemInternal(key, value, false);
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.ContainsKey" />
    [Pure]
    [Omnifactotum.Annotations.Pure]
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

    /// <inheritdoc />
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

    /// <inheritdoc cref="IReadOnlyDictionary{TKey,TValue}.TryGetValue" />
#if NET5_0_OR_GREATER
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#else
    public bool TryGetValue(TKey key, out TValue value)
#endif
    {
        var index = Determinant.GetIndex(key);
        var item = _items[index];

        var result = item.IsSet;
        value = result ? item.Value : default!;
        return result;
    }

    /// <inheritdoc />
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    /// <inheritdoc />
    public void Clear()
    {
        _version++;

        for (var index = 0; index < _items.Length; index++)
        {
            _items[index] = new DictionaryValueHolder();
        }

        Count = 0;
    }

    /// <inheritdoc />
    [Pure]
    [Omnifactotum.Annotations.Pure]
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        => TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);

    /// <inheritdoc />
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
            array[currentArrayIndex] = KeyValuePair.Create(key, item.Value);
            currentArrayIndex++;
        }
    }

    /// <inheritdoc />
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        => TryGetValue(item.Key, out var value)
            && EqualityComparer<TValue>.Default.Equals(value, item.Value)
            && Remove(item.Key);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this);

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal string ToDebuggerString() => $"{nameof(Count)} = {Count}";

    //// ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private void SetItemInternal(TKey key, TValue value, bool replaceExisting)
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

    private sealed class KeyCollection : ICollection<TKey>
    {
        private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;

        internal KeyCollection(FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary) => _dictionary = dictionary.EnsureNotNull();

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        void ICollection<TKey>.Add(TKey item) => throw new NotSupportedException();

        void ICollection<TKey>.Clear() => throw new NotSupportedException();

        public bool Contains(TKey item) => _dictionary.ContainsKey(item);

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
                    @"The number of elements is greater than the available space from the array index to the end of the destination array.",
                    nameof(arrayIndex));
            }

            var index = arrayIndex;
            foreach (var item in this)
            {
                array[index] = item;
                index++;
            }
        }

        bool ICollection<TKey>.Remove(TKey item) => throw new NotSupportedException();

        public IEnumerator<TKey> GetEnumerator() => _dictionary.Select(pair => pair.Key).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class ValueCollection : ICollection<TValue>
    {
        private readonly FixedSizeDictionary<TKey, TValue, TDeterminant> _dictionary;

        internal ValueCollection(FixedSizeDictionary<TKey, TValue, TDeterminant> dictionary) => _dictionary = dictionary.EnsureNotNull();

        public int Count => _dictionary.Count;

        public bool IsReadOnly => true;

        void ICollection<TValue>.Add(TValue item) => throw new NotSupportedException();

        void ICollection<TValue>.Clear() => throw new NotSupportedException();

        public bool Contains(TValue item) => _dictionary._items.Any(obj => obj.IsSet && EqualityComparer<TValue>.Default.Equals(obj.Value, item));

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
                    @"The number of elements is greater than the available space from the array index to the end of the destination array.");
            }

            var index = arrayIndex;
            foreach (var item in this)
            {
                array[index] = item;
                index++;
            }
        }

        bool ICollection<TValue>.Remove(TValue item) => throw new NotSupportedException();

        public IEnumerator<TValue> GetEnumerator() => _dictionary.Select(pair => pair.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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
            _size = Determinant.ValidatedSize;
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

        public void Reset() => ResetInternal();

        private void ResetInternal() => _index = -1;

        private void EnsureVersionConsistency()
        {
            if (_dictionary._version != _initialVersion || _dictionary.Count != _initialCount)
            {
                throw new InvalidOperationException("Cannot enumerate the modified collection.");
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Local")]
    private struct DictionaryValueHolder
    {
        public bool IsSet { get; set; }

        public TValue Value { get; set; }
    }
}