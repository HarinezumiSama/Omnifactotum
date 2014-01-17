using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     <para>
    ///         Represents the abstract immutable container that encapsulates a strongly-typed value and supports
    ///         comparison for equality.
    ///     </para>
    ///     <para>
    ///         <b>IMPORTANT</b> note for inheritors: a derived class MUST NOT add any new fields or properties
    ///         influencing equality comparison.
    ///     </para>
    /// </summary>
    /// <typeparam name="T">
    ///     The type of an encapsulated value.
    /// </typeparam>
    public abstract class EquatableValueCapsule<T> : ValueCapsule<T>, IEquatable<EquatableValueCapsule<T>>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EquatableValueCapsule{T}"/> class
        ///     using the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize this instance with.
        /// </param>
        protected EquatableValueCapsule([CanBeNull] T value)
            : base(value)
        {
            // Nothing to do
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the <see cref="IEqualityComparer{T}"/> used for comparing contained values for equality.
        /// </summary>
        public IEqualityComparer<T> ValueEqualityComparer
        {
            [DebuggerNonUserCode]
            get
            {
                return GetValueEqualityComparer().EnsureNotNull();
            }
        }

        #endregion

        #region Operators

        /// <summary>
        ///     Determines whether the two specified <see cref="EquatableValueCapsule{T}"/> instances are equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="EquatableValueCapsule{T}"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="EquatableValueCapsule{T}"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the two specified <see cref="EquatableValueCapsule{T}"/> instances are equal;
        ///     otherwise, <b>false</b>.
        /// </returns>
        public static bool operator ==([CanBeNull] EquatableValueCapsule<T> left, [CanBeNull] EquatableValueCapsule<T> right)
        {
            return EqualityComparer<EquatableValueCapsule<T>>.Default.Equals(left, right);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="EquatableValueCapsule{T}"/> instances are not equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="EquatableValueCapsule{T}"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="EquatableValueCapsule{T}"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the two specified <see cref="EquatableValueCapsule{T}"/> instances are not equal;
        ///     otherwise, <b>false</b>.
        /// </returns>
        public static bool operator !=([CanBeNull] EquatableValueCapsule<T> left, [CanBeNull] EquatableValueCapsule<T> right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="Object"/> is equal to
        ///     this <see cref="EquatableValueCapsule{T}"/>.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the specified <see cref="System.Object"/> is equal to
        ///     this <see cref="EquatableValueCapsule{T}"/>; otherwise, <b>false</b>.
        /// </returns>
        public sealed override bool Equals([CanBeNull] object obj)
        {
            return Equals(obj as EquatableValueCapsule<T>);
        }

        /// <summary>
        ///     Returns a hash code for this <see cref="EquatableValueCapsule{T}"/>.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like
        ///     a hash table.
        /// </returns>
        public sealed override int GetHashCode()
        {
            return ReferenceEquals(this.Value, null) ? 0 : this.ValueEqualityComparer.GetHashCode(this.Value);
        }

        #endregion

        #region IEquatable<EquatableValueCapsule<T>> Members

        /// <summary>
        ///     Determines whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the current object is equal to the <paramref name="other"/> parameter;
        ///     otherwise, <b>false</b>.
        /// </returns>
        public bool Equals([CanBeNull] EquatableValueCapsule<T> other)
        {
            if (ReferenceEquals(other, null) || other.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return this.ValueEqualityComparer.Equals(other.Value, this.Value);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Gets the <see cref="IEqualityComparer{T}"/> used for comparing contained values for equality.
        ///     Default implementation returns the default equality comparer for the type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEqualityComparer{T}"/> used for comparing contained values for equality.
        /// </returns>
        [DebuggerNonUserCode]
        protected virtual IEqualityComparer<T> GetValueEqualityComparer()
        {
            return EqualityComparer<T>.Default;
        }

        #endregion
    }
}