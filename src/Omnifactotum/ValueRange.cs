using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CanBeNullAttribute = Omnifactotum.Annotations.CanBeNullAttribute;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
using static Omnifactotum.FormattableStringFactotum;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum;

/// <summary>
///     Represents an inclusive range of values, given lower and upper boundaries.
/// </summary>
/// <typeparam name="T">
///     The type of the values in the range.
/// </typeparam>
[Serializable]
public readonly struct ValueRange<T>
    : IEquatable<ValueRange<T>>
#if NET7_0_OR_GREATER
        ,
        IEqualityOperators<ValueRange<T>, ValueRange<T>, bool>
#endif
    where T : IComparable
{
    private static readonly IComparer<T> ValueComparer = Comparer<T>.Default;
    private static readonly IEqualityComparer<T> ValueEqualityComparer = EqualityComparer<T>.Default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ValueRange{T}"/> structure.
    /// </summary>
    /// <param name="lower">
    ///     The lower boundary of the range.
    /// </param>
    /// <param name="upper">
    ///     The upper boundary of the range.
    /// </param>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public ValueRange(T lower, T upper)
    {
        if (ValueComparer.Compare(lower, upper) > 0)
        {
            throw new ArgumentException(AsInvariant($@"The lower boundary ({lower}) cannot be greater than the upper boundary ({upper})."));
        }

        Lower = lower;
        Upper = upper;
    }

    /// <summary>
    ///     Gets the lower boundary of the range.
    /// </summary>
    public T Lower
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get;
    }

    /// <summary>
    ///     Gets the upper boundary of the range.
    /// </summary>
    public T Upper
    {
        [Pure]
        [Omnifactotum.Annotations.Pure]
        get;
    }

    /// <summary>
    ///     Determines if the two specified <see cref="ValueRange{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ValueRange{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ValueRange{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="ValueRange{T}"/> instances are equal;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool operator ==(ValueRange<T> left, ValueRange<T> right) => Equals(left, right);

    /// <summary>
    ///     Determines if the two specified <see cref="ValueRange{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">
    ///     The first <see cref="ValueRange{T}"/> instance to compare.
    /// </param>
    /// <param name="right">
    ///     The second <see cref="ValueRange{T}"/> instance to compare.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the two specified <see cref="ValueRange{T}"/> instances are not equal;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public static bool operator !=(ValueRange<T> left, ValueRange<T> right) => !Equals(left, right);

#if NET7_0_OR_GREATER
    /// <inheritdoc/>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    static bool IEqualityOperators<ValueRange<T>, ValueRange<T>, bool>.operator ==(ValueRange<T> left, ValueRange<T> right) => left == right;

    /// <inheritdoc/>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    static bool IEqualityOperators<ValueRange<T>, ValueRange<T>, bool>.operator !=(ValueRange<T> left, ValueRange<T> right) => left != right;
#endif

    /// <summary>
    ///     Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// </summary>
    /// <param name="obj">
    ///     The <see cref="System.Object"/> to compare with this instance.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to this instance;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public override bool Equals([CanBeNull] object? obj) => obj is ValueRange<T> castObj && Equals(castObj);

    /// <summary>
    ///     Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    ///     A 32-bit signed integer that is the hash code for this instance.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public override int GetHashCode() => Lower.CombineHashCodes(Upper);

    /// <summary>
    ///     Returns a <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
    /// </returns>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         Console.WriteLine("Range: {0}", new ValueRange<int>(-2, 3).ToString()); // Output: Range: [-2 ~ 3]
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public override string ToString() => ToString(ValueRange.DefaultBoundarySeparator);

    /// <summary>
    ///     Returns a <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>, formatted using the specified boundary separator.
    /// </summary>
    /// <param name="boundarySeparator">
    ///     The separator to insert in the resulting string between <see cref="Lower"/> and <see cref="Upper"/>.
    ///     Cannot be <see langword="null"/> or <see cref="string.Empty"/>.
    /// </param>
    /// <returns>
    ///     A <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>, formatted using the specified boundary separator.
    /// </returns>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         Console.WriteLine("Range: {0}", new ValueRange<int>(-2, 3).ToString("/")); // Output: Range: [-2/3]
    ///         Console.WriteLine("Range: {0}", new ValueRange<int>(-2, 3).ToString(" ... ")); // Output: Range: [-2 ... 3]
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public string ToString([NotNull] string boundarySeparator)
    {
        if (string.IsNullOrEmpty(boundarySeparator))
        {
            throw new ArgumentException(@"The value can be neither empty string nor null.", nameof(boundarySeparator));
        }

        return AsInvariant($"[{Lower}{boundarySeparator}{Upper}]");
    }

    /// <summary>
    ///     Determines whether the current range contains the specified value.
    /// </summary>
    /// <param name="value">
    ///     The value to check.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the current range contains the specified value; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public bool Contains(T value) => ValueComparer.Compare(value, Lower) >= 0 && ValueComparer.Compare(value, Upper) <= 0;

    /// <summary>
    ///     Determines whether the current range contains the whole specified range, that is, whether
    ///     the values within the current range are a superset of the values within the specified range.
    /// </summary>
    /// <param name="other">
    ///     The range to check if it is contained in the current range.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the current range contains the whole specified range; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public bool Contains(ValueRange<T> other) => Contains(in other);

    /// <summary>
    ///     Determines whether the current range contains the whole specified range passed by reference, that is, whether
    ///     the values within the current range are a superset of the values within the specified range.
    /// </summary>
    /// <param name="other">
    ///     The range, passed by reference, to check if it is contained in the current range.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the current range contains the whole specified range; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [CLSCompliant(false)]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public bool Contains(in ValueRange<T> other) => Contains(other.Lower) && Contains(other.Upper);

    /// <summary>
    ///     Determines whether the current range intersects with the specified range.
    /// </summary>
    /// <param name="other">
    ///     The range to check for intersection with the current range.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the current range intersects with the specified other range; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public bool IntersectsWith(ValueRange<T> other) => IntersectsWith(in other);

    /// <summary>
    ///     Determines whether the current range intersects with the specified range passed by reference.
    /// </summary>
    /// <param name="other">
    ///     The range, passed by reference, to check for intersection with the current range.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the current range intersects with the specified other range; otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [CLSCompliant(false)]
    public bool IntersectsWith(in ValueRange<T> other) => Contains(other.Lower) || Contains(other.Upper) || other.Contains(Lower) || other.Contains(Upper);

    /// <summary>
    ///     Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">
    ///     An object to compare with this object.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    public bool Equals(ValueRange<T> other) => Equals(this, other);

    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    private static bool Equals(ValueRange<T> left, ValueRange<T> right)
        => ValueEqualityComparer.Equals(left.Lower, right.Lower) && ValueEqualityComparer.Equals(left.Upper, right.Upper);
}