using System;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

namespace Omnifactotum;

/// <summary>
///     Represents the case-insensitive string key.
/// </summary>
[Obsolete($"Use '{nameof(CaseInsensitiveString)}' instead of '{nameof(CaseInsensitiveStringKey)}'.")]
public readonly struct CaseInsensitiveStringKey : IEquatable<CaseInsensitiveStringKey>
{
    /// <summary>
    ///     <see cref="StringComparer"/> used to compare the underlying string value.
    /// </summary>
    [PublicAPI]
    public static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CaseInsensitiveStringKey"/> structure.
    /// </summary>
    /// <param name="value">
    ///     The original string value to create a <see cref="CaseInsensitiveStringKey"/> for.
    /// </param>
    public CaseInsensitiveStringKey(string? value) => Value = value;

    /// <summary>
    ///     Gets the original string value.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    ///     Determines whether the two specified <see cref="CaseInsensitiveStringKey"/> instances are equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="CaseInsensitiveStringKey"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="CaseInsensitiveStringKey"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="CaseInsensitiveStringKey"/> instances are equal; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static bool operator ==(CaseInsensitiveStringKey left, CaseInsensitiveStringKey right) => Equals(left, right);

    /// <summary>
    ///     Determines whether the two specified <see cref="CaseInsensitiveStringKey"/> instances are not equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="CaseInsensitiveStringKey"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="CaseInsensitiveStringKey"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="CaseInsensitiveStringKey"/> instances are not equal; otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static bool operator !=(CaseInsensitiveStringKey left, CaseInsensitiveStringKey right) => !Equals(left, right);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CaseInsensitiveStringKey castObj && Equals(castObj);

    /// <inheritdoc />
    public override int GetHashCode() => Value is null ? 0 : Comparer.GetHashCode(Value);

    /// <inheritdoc />
    public bool Equals(CaseInsensitiveStringKey other) => Equals(this, other);

    /// <inheritdoc />
    public override string ToString() => $@"{nameof(CaseInsensitiveStringKey)} {{ {nameof(Value)} = {Value.ToUIString()} }}";

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    private static bool Equals(CaseInsensitiveStringKey left, CaseInsensitiveStringKey right) => Comparer.Equals(left.Value, right.Value);
}