﻿using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Omnifactotum;

/// <summary>
///     Provides helper functionality for creating instances of the <see cref="ValueContainer{T}"/> type
///     using the type inference in a friendly way.
/// </summary>
public static class ValueContainer
{
    /// <summary>
    ///     Creates and initializes a new instance of the <see cref="ValueContainer{T}"/> class
    ///     using the specified value.
    /// </summary>
    /// <param name="value">
    ///     The value to initialize this instance with.
    /// </param>
    /// <returns>
    ///     A created and initialized instance of the <see cref="ValueContainer{T}" /> class.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static ValueContainer<T> Create<T>(T value) => new(value);
}