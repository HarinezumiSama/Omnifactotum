﻿using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using DisallowNullAttribute = System.Diagnostics.CodeAnalysis.DisallowNullAttribute;

#if !NET7_0_OR_GREATER
using System.Collections.ObjectModel;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax
//// ReSharper disable UseNullableAttributesSupportedByCompiler

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Collections.Generic;

/// <summary>
///     Contains extension methods for <see cref="IDictionary{TKey,TValue}"/>.
/// </summary>
public static class OmnifactotumDictionaryExtensions
{
    /// <summary>
    ///     Gets the value associated with the specified key from the specified dictionary.
    ///     If the specified key is not found, then a value is created, using the specified value factory, and
    ///     added to the dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of values in the dictionary.
    /// </typeparam>
    /// <param name="dictionary">
    ///     The dictionary to get the value from.
    /// </param>
    /// <param name="key">
    ///     The key whose value to get.
    /// </param>
    /// <param name="valueFactory">
    ///     A reference to a method used to create a value if the key is not found.
    /// </param>
    /// <returns>
    ///     A value that was associated with the specified key, or has been associated if it was not.
    /// </returns>
    public static TValue GetOrCreateValue<TKey, TValue>(
        [NotNull] this IDictionary<TKey, TValue> dictionary,
        [NotNull] [DisallowNull] TKey key,
        [NotNull] [InstantHandle] Func<TKey, TValue> valueFactory)
    {
        if (dictionary is null)
        {
            throw new ArgumentNullException(nameof(dictionary));
        }

        if (key is null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (valueFactory is null)
        {
            throw new ArgumentNullException(nameof(valueFactory));
        }

        if (dictionary.TryGetValue(key, out var result))
        {
            return result;
        }

        result = valueFactory(key);
        dictionary.Add(key, result);

        return result;
    }

    /// <summary>
    ///     Gets the value associated with the specified key from the specified dictionary.
    ///     If the specified key is not found, then a value is created, using the default constructor for
    ///     the type <typeparamref name="TValue"/>, and added to the dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of values in the dictionary.
    /// </typeparam>
    /// <param name="dictionary">
    ///     The dictionary to get the value from.
    /// </param>
    /// <param name="key">
    ///     The key whose value to get.
    /// </param>
    /// <returns>
    ///     A value that was associated with the specified key, or has been associated if it was not.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static TValue GetOrCreateValue<TKey, TValue>(
        [NotNull] this IDictionary<TKey, TValue> dictionary,
        [NotNull] [DisallowNull] TKey key)
        where TValue : new()
        => GetOrCreateValue(dictionary, key, _ => new TValue());

#if !NET7_0_OR_GREATER
    /// <summary>
    ///     Creates a read-only wrapper for the specified dictionary.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of values in the dictionary.
    /// </typeparam>
    /// <param name="dictionary">
    ///     The dictionary to create a read-only wrapper for.
    /// </param>
    /// <returns>
    ///     A read-only wrapper for the specified dictionary.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
        [NotNull] this IDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        => new(dictionary);
#endif
}