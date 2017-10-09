using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents an inclusive range of values, given lower and upper boundaries.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the values in the range.
    /// </typeparam>
    [Serializable]
    public struct ValueRange<T> : IEquatable<ValueRange<T>>
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
                throw new ArgumentException("The lower boundary cannot be greater than the upper one.");
            }

            Lower = lower;
            Upper = upper;
        }

        /// <summary>
        ///     Gets the lower boundary of the range.
        /// </summary>
        public T Lower
        {
            [DebuggerStepThrough]
            get;
        }

        /// <summary>
        ///     Gets the upper boundary of the range.
        /// </summary>
        public T Upper
        {
            [DebuggerStepThrough]
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
        ///     <c>true</c> if the two specified <see cref="ValueRange{T}"/> instances are equal;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ValueRange<T> left, ValueRange<T> right) => left.Equals(right);

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
        ///     <c>true</c> if the two specified <see cref="ValueRange{T}"/> instances are not equal;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ValueRange<T> left, ValueRange<T> right) => !left.Equals(right);

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        ///     otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public override bool Equals(object obj) => obj is ValueRange<T> castObj && Equals(castObj);

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        [Pure]
        public override int GetHashCode() => Lower.CombineHashCodes(Upper);

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
        /// </returns>
        [Pure]
        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "[{0}; {1}]", Lower, Upper);

        /// <summary>
        ///     Determines whether the current range contains the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current range contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public bool Contains(T value)
            => ValueComparer.Compare(value, Lower) >= 0 && ValueComparer.Compare(value, Upper) <= 0;

        /// <summary>
        ///     Determines whether the current range contains the whole specified range, that is, whether
        ///     the values within the current range are a superset of the values within the specified range.
        /// </summary>
        /// <param name="other">
        ///     The range to check if it is contained in the current range.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current range contains the whole specified range; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public bool Contains(ValueRange<T> other) => Contains(other.Lower) && Contains(other.Upper);

        /// <summary>
        ///     Determines whether the current range intersects with the specified range.
        /// </summary>
        /// <param name="other">
        ///     The range to check for intersection with the current range.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current range intersects with the specified other range; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public bool IntersectsWith(ValueRange<T> other)
            => Contains(other.Lower) || Contains(other.Upper) || other.Contains(Lower) || other.Contains(Upper);

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current object is equal to the <paramref name="other"/> parameter;
        ///     otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public bool Equals(ValueRange<T> other)
            => ValueEqualityComparer.Equals(other.Lower, Lower) && ValueEqualityComparer.Equals(other.Upper, Upper);
    }
}