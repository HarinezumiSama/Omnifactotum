using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Represents a generic dictionary which is internally handled by a fixed size array due to limitation for
///     the maximum number of possible keys, and whose keys are enumeration values.
/// </summary>
/// <typeparam name="TKey">
///     The type of the keys in the dictionary.
/// </typeparam>
/// <typeparam name="TValue">
///     The type of the values in the dictionary.
/// </typeparam>
public sealed class EnumFixedSizeDictionary<TKey, TValue> : FixedSizeDictionary<TKey, TValue, EnumFixedSizeDictionaryDeterminant<TKey>>
    where TKey : struct, Enum
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class.
    /// </summary>
    public EnumFixedSizeDictionary()
    {
        // Nothing to do
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class
    ///     by copying the key/values pairs from the specified collection.
    /// </summary>
    /// <param name="pairs">
    ///     The collection of the key/values pairs to copy from.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="pairs"/> is <see langword="null"/>.
    /// </exception>
    public EnumFixedSizeDictionary([NotNull] IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        : base(pairs)
    {
        // Nothing to do
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class
    ///     by copying the key/values pairs from the specified dictionary.
    /// </summary>
    /// <param name="dictionary">
    ///     The dictionary to copy the key/values pairs from.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="dictionary"/> is <see langword="null"/>.
    /// </exception>
    public EnumFixedSizeDictionary(
        [NotNull] FixedSizeDictionary<TKey, TValue, EnumFixedSizeDictionaryDeterminant<TKey>> dictionary)
        : base(dictionary)
    {
        // Nothing to do
    }
}