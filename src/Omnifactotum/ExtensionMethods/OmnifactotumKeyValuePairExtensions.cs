using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Omnifactotum.ExtensionMethods;

/// <summary>
///     Contains extension methods for the <see cref="KeyValuePair{TKey,TValue}"/> structure.
/// </summary>
public static class OmnifactotumKeyValuePairExtensions
{
    /// <summary>
    ///     Converts the specified <see cref="KeyValuePair{TKey,TValue}"/> to <see cref="ValueTuple{TKey,TValue}"/>.
    /// </summary>
    /// <param name="pair">
    ///     The key/value pair to convert.
    /// </param>
    /// <typeparam name="TKey">
    ///     The type of the key in <see cref="KeyValuePair{TKey,TValue}"/> and the value tuple's first element.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the  value in <see cref="KeyValuePair{TKey,TValue}"/> and the value tuple's second element.
    /// </typeparam>
    /// <returns>
    ///     A <see cref="ValueTuple{TKey,TValue}"/> created from <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ValueTuple<TKey, TValue> ToValueTuple<TKey, TValue>(this KeyValuePair<TKey, TValue> pair) => new(pair.Key, pair.Value);
}