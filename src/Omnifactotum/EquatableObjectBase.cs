using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the abstract container that supports comparison for equality.
    /// </summary>
    public abstract class EquatableObjectBase : IEquatable<EquatableObjectBase>
    {
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
        ///     <see langword="true"/> if the two specified <see cref="EquatableObjectBase"/> instances are equal;
        ///     otherwise, <see langword="false"/>.
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
        ///     <see langword="true"/> if the two specified <see cref="EquatableObjectBase"/> instances are not equal;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=([CanBeNull] EquatableObjectBase left, [CanBeNull] EquatableObjectBase right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Object"/> is equal to
        ///     this <see cref="EquatableObjectBase"/>.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the specified <see cref="System.Object"/> is equal to
        ///     this <see cref="EquatableObjectBase"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public sealed override bool Equals(object obj)
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

        /// <summary>
        ///     Determines whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(EquatableObjectBase other)
            => !(other is null) && other.GetType() == GetType()
                && (ReferenceEquals(other, this) || EqualsInternal(other));

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
        ///     it is never <see langword="null"/> in this method and also can be safely cast to an actual derived type.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter;
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        protected abstract bool EqualsInternal([NotNull] EquatableObjectBase other);
    }
}