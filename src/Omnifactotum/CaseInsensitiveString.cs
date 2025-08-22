using System;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Omnifactotum;

/// <summary>
///     Represents the case-insensitive string.
/// </summary>
public readonly struct CaseInsensitiveString
    :
#if NET7_0_OR_GREATER
        IEqualityOperators<CaseInsensitiveString, CaseInsensitiveString, bool>,
#endif
        IEquatable<CaseInsensitiveString>
{
    /// <summary>
    ///     <see cref="StringComparer"/> used to compare the underlying string value.
    /// </summary>
    [PublicAPI]
    public static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CaseInsensitiveString"/> structure.
    /// </summary>
    /// <param name="value">
    ///     The original string value to create a <see cref="CaseInsensitiveString"/> for.
    /// </param>
    public CaseInsensitiveString(string? value) => Value = value;

    /// <summary>
    ///     Gets the original string value.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    ///     Creates a <see cref="CaseInsensitiveString"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">
    ///     A string value to create a <see cref="CaseInsensitiveString"/> instance from.
    /// </param>
    /// <returns>
    ///     A <see cref="CaseInsensitiveString"/> instance that has <see cref="Value"/> equal to the specified string value.
    /// </returns>
    public static implicit operator CaseInsensitiveString(string? value) => new(value);

    /// <summary>
    ///     Returns the value of the <see cref="Value"/> property.
    /// </summary>
    /// <param name="obj">
    ///     A <see cref="CaseInsensitiveString"/> instance to convert to string.
    /// </param>
    /// <returns>
    ///     The value of the <see cref="Value"/> property.
    /// </returns>
    public static implicit operator string?(CaseInsensitiveString obj) => obj.Value;

    /// <summary>
    ///     Determines whether the two specified <see cref="CaseInsensitiveString"/> instances are equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="CaseInsensitiveString"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static bool operator ==(CaseInsensitiveString left, CaseInsensitiveString right) => Equals(left, right);

    /// <summary>
    ///     Determines whether the two specified <see cref="CaseInsensitiveString"/> instances are not equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="CaseInsensitiveString"/> instances are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static bool operator !=(CaseInsensitiveString left, CaseInsensitiveString right) => !Equals(left, right);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CaseInsensitiveString castObj && Equals(castObj);

    /// <inheritdoc />
    public override int GetHashCode() => Value is null ? 0 : Comparer.GetHashCode(Value);

    /// <inheritdoc />
    public bool Equals(CaseInsensitiveString other) => Equals(this, other);

    /// <summary>
    ///     Returns the value of the <see cref="Value"/> property.
    /// </summary>
    /// <returns>
    ///     The value of the <see cref="Value"/> property.
    /// </returns>
    public override string? ToString() => Value;

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    private static bool Equals(CaseInsensitiveString left, CaseInsensitiveString right) => Comparer.Equals(left.Value, right.Value);
}