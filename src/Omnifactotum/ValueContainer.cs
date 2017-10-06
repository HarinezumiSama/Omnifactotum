using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the mutable container that encapsulates a strongly-typed value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    [Serializable]
    [DebuggerDisplay(@"\{Value = {Value}\}")]
    public sealed class ValueContainer<T> : IValueContainer<T>, IEquatable<ValueContainer<T>>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueContainer{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        public ValueContainer([CanBeNull] T value)
        {
            Value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueContainer{T}"/> class
        ///     using the default value for the type <typeparamref name="T"/>.
        /// </summary>
        public ValueContainer()
            : this(default(T))
        {
            // Nothing to do
        }

        /// <summary>
        ///     Gets or sets the encapsulated value.
        /// </summary>
        public T Value
        {
            get;
            set;
        }

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
        ///     <c>true</c> if the two specified <see cref="ValueContainer{T}"/> instances are equal;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(ValueContainer<T> left, ValueContainer<T> right)
        {
            return Equals(left, right);
        }

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
        ///     <c>true</c> if the two specified <see cref="ValueContainer{T}"/> instances are not equal;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(ValueContainer<T> left, ValueContainer<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Returns a <see cref="String" /> that represents this <see cref="ValueContainer{T}"/> instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="String" /> that represents this <see cref="ValueContainer{T}"/> instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{ Value = {0} }}", Value.ToStringSafely());
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Object" /> is equal to
        ///     this <see cref="ValueContainer{T}"/> instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this <see cref="ValueContainer{T}"/> instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Object" /> is equal to
        ///     this <see cref="ValueContainer{T}"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as ValueContainer<T>);
        }

        /// <summary>
        ///     Returns a hash code for this <see cref="ValueContainer{T}"/> instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this <see cref="ValueContainer{T}"/> instance.
        /// </returns>
        public override int GetHashCode()
        {
            return Value.GetHashCodeSafely();
        }

        /// <summary>
        ///     Determines whether the current <see cref="ValueContainer{T}"/> instance is equal to another instance
        ///     of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this <see cref="ValueContainer{T}"/> instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="ValueContainer{T}"/> instance is equal to
        ///     the <paramref name="other" /> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ValueContainer<T> other)
        {
            return Equals(this, other);
        }

        private static bool Equals(ValueContainer<T> left, ValueContainer<T> right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return EqualityComparer<T>.Default.Equals(left.Value, right.Value);
        }
    }
}