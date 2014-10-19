using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the abstract container that supports comparison for equality.
    /// </summary>
    public abstract class EquatableObjectBase : IEquatable<EquatableObjectBase>
    {
        #region Operators

        /// <summary>
        ///     Determines whether the two specified <see cref="EquatableObjectBase"/> instances are equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="EquatableObjectBase"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="EquatableObjectBase"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the two specified <see cref="EquatableObjectBase"/> instances are equal;
        ///     otherwise, <b>false</b>.
        /// </returns>
        public static bool operator ==([CanBeNull] EquatableObjectBase left, [CanBeNull] EquatableObjectBase right)
        {
            return EqualityComparer<EquatableObjectBase>.Default.Equals(left, right);
        }

        /// <summary>
        ///     Determines whether the two specified <see cref="EquatableObjectBase"/> instances are not equal.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="EquatableObjectBase"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="EquatableObjectBase"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the two specified <see cref="EquatableObjectBase"/> instances are not equal;
        ///     otherwise, <b>false</b>.
        /// </returns>
        public static bool operator !=([CanBeNull] EquatableObjectBase left, [CanBeNull] EquatableObjectBase right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Determines whether the specified <see cref="Object"/> is equal to
        ///     this <see cref="EquatableObjectBase"/>.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the specified <see cref="System.Object"/> is equal to
        ///     this <see cref="EquatableObjectBase"/>; otherwise, <b>false</b>.
        /// </returns>
        public sealed override bool Equals([CanBeNull] object obj)
        {
            return Equals(obj as EquatableObjectBase);
        }

        /// <summary>
        ///     Returns a hash code for this <see cref="EquatableObjectBase"/>.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like
        ///     a hash table.
        /// </returns>
        public sealed override int GetHashCode()
        {
            return GetHashCodeInternal();
        }

        #endregion

        #region IEquatable<EquatableObjectBase> Members

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
        public bool Equals([CanBeNull] EquatableObjectBase other)
        {
            if (ReferenceEquals(other, null) || other.GetType() != GetType())
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return EqualsInternal(other);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Returns a hash code for this <see cref="EquatableObjectBase"/>.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like
        ///     a hash table.
        /// </returns>
        protected abstract int GetHashCodeInternal();

        /// <summary>
        ///     Determines whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this object. The parameter is checked prior to calling this method, therefore
        ///     it is never <b>null</b> in this method and also can be safely cast to an actual derived type.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the current object is equal to the <paramref name="other"/> parameter;
        ///     otherwise, <b>false</b>.
        /// </returns>
        protected abstract bool EqualsInternal([NotNull] EquatableObjectBase other);

        #endregion
    }
}