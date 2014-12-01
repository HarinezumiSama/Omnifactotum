using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        #region Constants and Fields

        private static readonly IComparer<T> ValueComparer = Comparer<T>.Default;
        private static readonly IEqualityComparer<T> ValueEqualityComparer = EqualityComparer<T>.Default;

        private readonly T _lower;
        private readonly T _upper;

        #endregion

        #region Constructors

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
            #region Argument Check

            if (ValueComparer.Compare(lower, upper) > 0)
            {
                throw new ArgumentException("The lower boundary cannot be greater than the upper one.");
            }

            #endregion

            _lower = lower;
            _upper = upper;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the lower boundary of the range.
        /// </summary>
        public T Lower
        {
            [DebuggerStepThrough]
            get
            {
                return _lower;
            }
        }

        /// <summary>
        ///     Gets the upper boundary of the range.
        /// </summary>
        public T Upper
        {
            [DebuggerStepThrough]
            get
            {
                return _upper;
            }
        }

        #endregion

        #region Operators

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
        public static bool operator ==(ValueRange<T> left, ValueRange<T> right)
        {
            return left.Equals(right);
        }

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
        public static bool operator !=(ValueRange<T> left, ValueRange<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Public Methods

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
        public override bool Equals(object obj)
        {
            return obj is ValueRange<T> && Equals((ValueRange<T>)obj);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        [Pure]
        public override int GetHashCode()
        {
            return _lower.CombineHashCodes(_upper);
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String"/> that represents this <see cref="ValueRange{T}"/>.
        /// </returns>
        [Pure]
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[{0}; {1}]", _lower, _upper);
        }

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
        {
            return ValueComparer.Compare(value, _lower) >= 0 && ValueComparer.Compare(value, _upper) <= 0;
        }

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
        public bool Contains(ValueRange<T> other)
        {
            return Contains(other._lower) && Contains(other._upper);
        }

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
        {
            return Contains(other._lower) || Contains(other._upper)
                || other.Contains(_lower) || other.Contains(_upper);
        }

        #endregion

        #region IEquatable<ValueRange<T>> Members

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
        {
            return ValueEqualityComparer.Equals(other._lower, _lower)
                && ValueEqualityComparer.Equals(other._upper, _upper);
        }

        #endregion
    }
}