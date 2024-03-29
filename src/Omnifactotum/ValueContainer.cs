﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Omnifactotum.Abstractions;
using ExcludeFromCodeCoverageAttribute = System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum;

/// <summary>
///     Represents the mutable container that encapsulates a strongly-typed value.
/// </summary>
/// <typeparam name="T">
///     The type of an encapsulated value.
/// </typeparam>
[Serializable]
[DebuggerDisplay(@"{ToDebuggerString(),nq}")]
public sealed class ValueContainer<T> : IValueContainer<T>, IEquatable<ValueContainer<T>>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValueContainer{T}"/> class
    ///     using the specified value.
    /// </summary>
    /// <param name="value">
    ///     The value to initialize this instance with.
    /// </param>
    public ValueContainer(T value) => Value = value;

    /// <inheritdoc />
    public T Value { get; set; }

    /// <summary>
    ///     Determines whether the two specified <see cref="ValueContainer{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ValueContainer{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ValueContainer{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="ValueContainer{T}"/> instances are equal;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(ValueContainer<T>? left, ValueContainer<T>? right) => Equals(left, right);

    /// <summary>
    ///     Determines whether the two specified <see cref="ValueContainer{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ValueContainer{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ValueContainer{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="ValueContainer{T}"/> instances are not equal;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(ValueContainer<T>? left, ValueContainer<T>? right) => !Equals(left, right);

    /// <summary>
    ///     Returns a <see cref="String" /> that represents this <see cref="ValueContainer{T}"/> instance.
    /// </summary>
    /// <returns>
    ///     A <see cref="String" /> that represents this <see cref="ValueContainer{T}"/> instance.
    /// </returns>
    public override string ToString() => AsInvariant($@"{{ {nameof(Value)} = {Value.ToStringSafelyInvariant()} }}");

    /// <summary>
    ///     Determines whether the specified <see cref="object" /> is equal to
    ///     this <see cref="ValueContainer{T}"/> instance.
    /// </summary>
    /// <param name="obj">
    ///     The <see cref="object" /> to compare with this <see cref="ValueContainer{T}"/> instance.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="object" /> is equal to
    ///     this <see cref="ValueContainer{T}"/> instance; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object? obj) => Equals(obj as ValueContainer<T>);

    /// <summary>
    ///     Returns a hash code for this <see cref="ValueContainer{T}"/> instance.
    /// </summary>
    /// <returns>
    ///     A hash code for this <see cref="ValueContainer{T}"/> instance.
    /// </returns>
    public override int GetHashCode()
        //// ReSharper disable once NonReadonlyMemberInGetHashCode :: Specific use-case
        => Value.GetHashCodeSafely();

    /// <summary>
    ///     Determines whether the current <see cref="ValueContainer{T}"/> instance is equal to another instance
    ///     of the same type.
    /// </summary>
    /// <param name="other">
    ///     An object to compare with this <see cref="ValueContainer{T}"/> instance.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the current <see cref="ValueContainer{T}"/> instance is equal to
    ///     the <paramref name="other" /> parameter; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(ValueContainer<T>? other) => Equals(this, other);

    //// ReSharper disable SuggestBaseTypeForParameter :: Performance
    private static bool Equals(ValueContainer<T>? left, ValueContainer<T>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return EqualityComparer<T>.Default.Equals(left.Value, right.Value);
    }
    //// ReSharper restore SuggestBaseTypeForParameter

    [ExcludeFromCodeCoverage]
    private string ToDebuggerString() => ToString();
}