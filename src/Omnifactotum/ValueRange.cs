#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using CanBeNullAttribute = Omnifactotum.Annotations.CanBeNullAttribute;
using NotNullAttribute = Omnifactotum.Annotations.NotNullAttribute;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum
{
    /// <summary>
    ///     Represents an inclusive range of values, given lower and upper boundaries.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the values in the range.
    /// </typeparam>
    [Serializable]
    public readonly struct ValueRange<T> : IEquatable<ValueRange<T>>
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
        public T Lower { get; }

        /// <summary>
        ///     Gets the upper boundary of the range.
        /// </summary>
        public T Upper { get; }

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
        public override bool Equals([CanBeNull] object? obj) => obj is ValueRange<T> castObj && Equals(castObj);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
        public override int GetHashCode() => Lower.CombineHashCodes(Upper);

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [NotNull]
        public override string ToString() => AsInvariant($"[{Lower}; {Upper}]");

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
        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
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
        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
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
        public bool Equals(ValueRange<T> other) => Equals(this, other);

        [Pure]
        [Omnifactotum.Annotations.Pure]
        [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
        private static bool Equals(ValueRange<T> left, ValueRange<T> right)
            => ValueEqualityComparer.Equals(left.Lower, right.Lower) && ValueEqualityComparer.Equals(left.Upper, right.Upper);
    }
}