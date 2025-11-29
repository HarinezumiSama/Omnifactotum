using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Omnifactotum.ExtensionMethods;

/// <summary>
///     Contains extension methods for the value tuple types.
/// </summary>
public static class OmnifactotumValueTupleExtensions
{
    /// <summary>
    ///     Converts the specified <see cref="ValueTuple{TKey,TValue}"/> to <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="tuple">
    ///     The value tuple to convert.
    /// </param>
    /// <typeparam name="TKey">
    ///     The type of the value tuple's first element and the key in <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the value tuple's second element and the value in <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="KeyValuePair{TKey,TValue}"/> created from <see cref="ValueTuple{TKey,TValue}"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static KeyValuePair<TKey, TValue> ToKeyValuePair<TKey, TValue>(this ValueTuple<TKey, TValue> tuple) => new(tuple.Item1, tuple.Item2);

    /// <summary>
    ///     Converts the specified <see cref="ValueTuple{TKey,TValue}"/> to <see cref="DictionaryEntry"/>.
    /// </summary>
    /// <param name="tuple">
    ///     The value tuple to convert.
    /// </param>
    /// <typeparam name="TKey">
    ///     The type of the value tuple's first element and the key in <see cref="DictionaryEntry"/>.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the value tuple's second element and the value in <see cref="DictionaryEntry"/>.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="DictionaryEntry"/> created from <see cref="ValueTuple{TKey,TValue}"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static DictionaryEntry ToDictionaryEntry<TKey, TValue>(this ValueTuple<TKey, TValue> tuple)
        where TKey : notnull
        => new(tuple.Item1, tuple.Item2);

    /// <summary>
    ///     Converts the specified <see cref="ValueTuple{T,T}"/> to <see cref="ValueRange{T}"/>.
    /// </summary>
    /// <param name="tuple">
    ///     The value tuple to convert.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the values in the value tuple and in the resulting value range.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="ValueRange{T}"/> created from <see cref="ValueTuple{T,T}"/>,
    ///     where <see cref="ValueRange{T}.Lower"/> is the first tuple component and <see cref="ValueRange{T}.Upper"/> is the second tuple component.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ValueRange<T> ToValueRange<T>(this ValueTuple<T, T> tuple)
        where T : IComparable
        => new(tuple.Item1, tuple.Item2);
}