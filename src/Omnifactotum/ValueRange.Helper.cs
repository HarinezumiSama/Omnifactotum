﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Omnifactotum;

/// <summary>
///     Provides helper functionality for creating instances of the <see cref="ValueRange{T}"/> type using type inference in a friendly way.
/// </summary>
public static class ValueRange
{
    /// <summary>
    ///     The default boundary separator. Used in <see cref="ValueRange{T}.ToString()"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "ConvertToConstant.Global")]
    public static readonly string DefaultBoundarySeparator = "\x0020~\x0020";

    /// <summary>
    ///     Creates a new instance of the <see cref="ValueRange{T}"/> structure using the specified values.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value.
    /// </typeparam>
    /// <param name="lower">
    ///     The lower boundary of the range.
    /// </param>
    /// <param name="upper">
    ///     The upper boundary of the range.
    /// </param>
    /// <returns>
    ///     A new instance of the <see cref="ValueRange{T}"/> class.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static ValueRange<T> Create<T>(T lower, T upper)
        where T : IComparable
        => new(lower, upper);
}