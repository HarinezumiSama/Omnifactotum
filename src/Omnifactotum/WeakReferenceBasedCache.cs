using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Represents the cache which is similar to a dictionary and leverages <see cref="WeakReference"/>
///     for the cached values.
/// </summary>
/// <typeparam name="TKey">
///     The type of the keys in the cache.
/// </typeparam>
/// <typeparam name="TValue">
///     The type of the values in the cache.
/// </typeparam>
[DebuggerDisplay("{ToDebuggerString(),nq}")]
public sealed class WeakReferenceBasedCache<TKey, TValue>
    where TKey : notnull
    where TValue : class
{
    private readonly Func<TKey, TValue> _valueFactory;
    private readonly Dictionary<TKey, WeakReference> _dictionary;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WeakReferenceBasedCache{TKey,TValue}"/> class
    ///     using the specified value factory and equality comparer for keys.
    /// </summary>
    /// <param name="valueFactory">
    ///     A reference to a method that creates a value for the specified key once needed.
    /// </param>
    /// <param name="keyEqualityComparer">
    ///     The equality comparer to use when comparing keys, or <see langword="null"/> to use
    ///     the default <see cref="EqualityComparer{T}"/> for the type of the key.
    /// </param>
    public WeakReferenceBasedCache(
        [NotNull] Func<TKey, TValue> valueFactory,
        [CanBeNull] IEqualityComparer<TKey>? keyEqualityComparer = null)
    {
        _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        _dictionary = new Dictionary<TKey, WeakReference>(keyEqualityComparer);
    }

    /// <summary>
    ///     Gets the equality comparer used when comparing keys.
    /// </summary>
    /// <remarks>
    ///     This property is thread-safe.
    /// </remarks>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [NotNull]
    public IEqualityComparer<TKey> KeyEqualityComparer
    {
        [DebuggerNonUserCode]
        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
        get
        {
            lock (_dictionary)
            {
                return _dictionary.Comparer.EnsureNotNull();
            }
        }
    }

    /// <summary>
    ///     Gets the number of the items in the cache.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Count
    {
        [DebuggerNonUserCode]
        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
        get
        {
            lock (_dictionary)
            {
                return UnsafeCount;
            }
        }
    }

    /// <summary>
    ///     Gets a value associated with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The key to get a value for.
    /// </param>
    /// <returns>
    ///     A value associated with the specified key.
    /// </returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [NotNull]
    public TValue this[[NotNull] TKey key]
    {
        get
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            TValue? result;
            lock (_dictionary)
            {
                //// ReSharper disable once InvokeAsExtensionMethod :: Avoiding multi-target issues
                var valueReference = OmnifactotumDictionaryExtensions.GetValueOrDefault(_dictionary, key);
                if (valueReference is null)
                {
                    result = CreateValue(key);
                    valueReference = new WeakReference(result, false);
                    _dictionary[key] = valueReference;
                }
                //// ReSharper disable once ArrangeRedundantParentheses :: For clarity
                else if ((result = valueReference.Target as TValue) is null)
                {
                    result = CreateValue(key);
                    valueReference.Target = result;
                }
            }

            return result;
        }
    }

    private int UnsafeCount
    {
        [DebuggerNonUserCode]
        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
        get => _dictionary.Count;
    }

    /// <summary>
    ///     Clears the cache.
    /// </summary>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public void Clear()
    {
        lock (_dictionary)
        {
            _dictionary.Clear();
        }
    }

    /// <summary>
    ///     Removes the value associated with the specified key.
    /// </summary>
    /// <param name="key">
    ///     The key to remove.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the element is successfully found and removed; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public bool Remove([NotNull] TKey key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        lock (_dictionary)
        {
            return _dictionary.Remove(key);
        }
    }

    [NotNull]
    private string ToDebuggerString() => AsInvariant($@"{{ {GetType().GetQualifiedName()}: {nameof(Count)} = {UnsafeCount} }}");

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    private TValue CreateValue([NotNull] TKey key) => _valueFactory(key).EnsureNotNull();
}