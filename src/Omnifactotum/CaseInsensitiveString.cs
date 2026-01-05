using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CanBeNullAttribute = Omnifactotum.Annotations.CanBeNullAttribute;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;
using PublicAPIAttribute = Omnifactotum.Annotations.PublicAPIAttribute;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Represents the case-insensitive string.
/// </summary>
/// <remarks>
///     For un uninitialized <see cref="CaseInsensitiveString"/> instance (as in <c>var s = default(CaseInsensitiveString)</c>),
///     <see cref="Value"/> is <see cref="string.Empty"/>.
/// </remarks>
/// <seealso cref="CaseInsensitiveString.Comparer"/>
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
    ///     Represents the empty <see cref="CaseInsensitiveString"/>, that is, the one that corresponds to <see cref="string.Empty"/>.
    /// </summary>
    public static readonly CaseInsensitiveString Empty = new(string.Empty);

    private readonly string _value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CaseInsensitiveString"/> structure.
    /// </summary>
    /// <param name="value">
    ///     The original string value to create a <see cref="CaseInsensitiveString"/> for.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public CaseInsensitiveString([NotNull] string value) => _value = value ?? throw new ArgumentNullException(nameof(value));

    /// <summary>
    ///     Gets the original string value.
    /// </summary>
    /// <remarks>
    ///     For un uninitialized <see cref="CaseInsensitiveString"/> instance (as in <c>var s = default(CaseInsensitiveString)</c>),
    ///     <see cref="Value"/> is <see cref="string.Empty"/>.
    /// </remarks>
    [NotNull]
    public string Value => _value ?? string.Empty;

    /// <summary>
    ///     Creates a nullable <see cref="CaseInsensitiveString"/> instance from the specified nullable string value.
    /// </summary>
    /// <param name="value">
    ///     A nullable string value to create a nullable <see cref="CaseInsensitiveString"/> instance from.
    /// </param>
    /// <returns>
    ///     A <see cref="CaseInsensitiveString"/> instance that has <see cref="Value"/> equal to the specified string value if it's not <see langword="null"/>;
    ///     or <see langword="null"/> if the specified string value is <see langword="null"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [return: NotNullIfNotNull(nameof(value))]
    public static implicit operator CaseInsensitiveString?([CanBeNull] string? value) => Create(value);

    /// <summary>
    ///     Creates a <see cref="CaseInsensitiveString"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">
    ///     A string value to create a <see cref="CaseInsensitiveString"/> instance from.
    /// </param>
    /// <returns>
    ///     A <see cref="CaseInsensitiveString"/> instance that has <see cref="Value"/> equal to the specified string value.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static implicit operator CaseInsensitiveString([NotNull] string value) => new(value);

    /// <summary>
    ///     Returns the value of the <see cref="Value"/> property.
    /// </summary>
    /// <param name="obj">
    ///     A <see cref="CaseInsensitiveString"/> instance to convert to string.
    /// </param>
    /// <returns>
    ///     The value of the <see cref="Value"/> property.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [NotNull]
    public static implicit operator string(CaseInsensitiveString obj) => obj.Value;

    /// <summary>
    ///     Returns the value of the <see cref="Value"/> property if <paramref name="obj"/> is not <see langword="null"/>;
    ///     otherwise, returns<see langword="null"/>.
    /// </summary>
    /// <param name="obj">
    ///     A nullable <see cref="CaseInsensitiveString"/> instance to convert to string.
    /// </param>
    /// <returns>
    ///     The value of the <see cref="Value"/> property or <see langword="null"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [CanBeNull]
    [return: NotNullIfNotNull(nameof(obj))]
    public static implicit operator string?(CaseInsensitiveString? obj) => obj?.Value;

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

    /// <summary>
    ///     Determines whether the two specified nullable <see cref="CaseInsensitiveString"/> instances are equal.
    /// </summary>
    /// <param name="left">
    ///     The first nullable <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second nullable <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified nullable <see cref="CaseInsensitiveString"/> instances are equal;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static bool operator ==(CaseInsensitiveString? left, CaseInsensitiveString? right) => Equals(left, right);

    /// <summary>
    ///     Determines whether the two specified nullable <see cref="CaseInsensitiveString"/> instances are not equal.
    /// </summary>
    /// <param name="left">
    ///     The first nullable <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second nullable <see cref="CaseInsensitiveString"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified nullable <see cref="CaseInsensitiveString"/> instances are not equal;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public static bool operator !=(CaseInsensitiveString? left, CaseInsensitiveString? right) => !Equals(left, right);

    /// <summary>
    ///     Creates a nullable <see cref="CaseInsensitiveString"/> instance from the specified nullable string value.
    /// </summary>
    /// <param name="value">
    ///     A nullable string value to create a nullable <see cref="CaseInsensitiveString"/> instance from.
    /// </param>
    /// <returns>
    ///     A <see cref="CaseInsensitiveString"/> instance that has <see cref="Value"/> equal to the specified string value if it's not <see langword="null"/>;
    ///     or <see langword="null"/> if the specified string value is <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(value))]
    public static CaseInsensitiveString? Create([CanBeNull] string? value) => value is null ? (CaseInsensitiveString?)null : new CaseInsensitiveString(value);

    /// <inheritdoc />
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public override bool Equals([CanBeNull] object? obj) => obj is CaseInsensitiveString castObj && Equals(castObj);

    /// <inheritdoc />
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public override int GetHashCode() => Comparer.GetHashCode(Value);

    /// <inheritdoc />
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    public bool Equals(CaseInsensitiveString other) => Equals(this, other);

    /// <summary>
    ///     Returns the value of the <see cref="Value"/> property.
    /// </summary>
    /// <returns>
    ///     The value of the <see cref="Value"/> property.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [NotNull]
    public override string ToString() => Value;

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    private static bool Equals(CaseInsensitiveString left, CaseInsensitiveString right) => Comparer.Equals(left.Value, right.Value);

    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    private static bool Equals(CaseInsensitiveString? left, CaseInsensitiveString? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return Equals(left.Value, right.Value);
    }
}