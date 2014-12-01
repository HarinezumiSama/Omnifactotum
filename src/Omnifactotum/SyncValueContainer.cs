using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the mutable thread-safe container that encapsulates a strongly-typed value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    [Serializable]
    [DebuggerDisplay(@"\{Value = {_value}\}")]
    public sealed class SyncValueContainer<T> : IEquatable<SyncValueContainer<T>>
    {
        #region Constants and Fields

        private T _value;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}" /> class
        ///     using the specified value and synchronization object.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        /// <param name="syncObject">
        ///     The synchronization object used for thread-safe access.
        ///     Cannot be <c>null</c> and must be of a reference type.
        /// </param>
        public SyncValueContainer([CanBeNull] T value, [NotNull] object syncObject)
        {
            #region Argument Check

            if (syncObject == null)
            {
                throw new ArgumentNullException("syncObject");
            }

            if (syncObject.GetType().IsValueType)
            {
                throw new ArgumentException("The synchronization object cannot be a value type object.");
            }

            #endregion

            this.SyncObject = syncObject;
            _value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        public SyncValueContainer([CanBeNull] T value)
            : this(value, new object())
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncValueContainer{T}"/> class
        ///     using the default value for the type <typeparamref name="T"/>.
        /// </summary>
        public SyncValueContainer()
            : this(default(T))
        {
            // Nothing to do
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the synchronization object used for thread-safe access.
        /// </summary>
        [NotNull]
        public object SyncObject
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets the contained value.
        /// </summary>
        [CanBeNull]
        public T Value
        {
            [DebuggerNonUserCode]
            get
            {
                lock (this.SyncObject)
                {
                    return _value;
                }
            }

            [DebuggerNonUserCode]
            set
            {
                lock (this.SyncObject)
                {
                    _value = value;
                }
            }
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Determines whether the two specified <see cref="SyncValueContainer{T}"/> instances are equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the two specified <see cref="SyncValueContainer{T}"/> instances are equal;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(SyncValueContainer<T> left, SyncValueContainer<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="SyncValueContainer{T}"/> instances are not equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="SyncValueContainer{T}"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the two specified <see cref="SyncValueContainer{T}"/> instances are not equal;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(SyncValueContainer<T> left, SyncValueContainer<T> right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="String" /> that represents this <see cref="SyncValueContainer{T}"/> instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="String" /> that represents this <see cref="SyncValueContainer{T}"/> instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{{ Value = {0} }}", this.Value.ToStringSafely());
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Object" /> is equal to
        ///     this <see cref="SyncValueContainer{T}"/> instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this <see cref="SyncValueContainer{T}"/> instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Object" /> is equal to
        ///     this <see cref="SyncValueContainer{T}"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as SyncValueContainer<T>);
        }

        /// <summary>
        ///     Returns a hash code for this <see cref="SyncValueContainer{T}"/> instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this <see cref="SyncValueContainer{T}"/> instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCodeSafely();
        }

        #endregion

        #region IEquatable<SyncValueContainer<T>> Members

        /// <summary>
        ///     Determines whether the current <see cref="SyncValueContainer{T}"/> instance is equal to another instance
        ///     of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this <see cref="SyncValueContainer{T}"/> instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="SyncValueContainer{T}"/> instance is equal to
        ///     the <paramref name="other" /> parameter; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(SyncValueContainer<T> other)
        {
            return Equals(this, other);
        }

        #endregion

        #region Private Methods

        private static bool Equals(SyncValueContainer<T> left, SyncValueContainer<T> right)
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

        #endregion
    }
}