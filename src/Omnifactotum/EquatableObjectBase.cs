using System;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==([CanBeNull] EquatableObjectBase? left, [CanBeNull] EquatableObjectBase? right) => Equals(left, right);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=([CanBeNull] EquatableObjectBase? left, [CanBeNull] EquatableObjectBase? right) => !Equals(left, right);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override bool Equals(object? obj) => Equals(obj as EquatableObjectBase);

        /// <summary>
        ///     Returns a hash code for this <see cref="EquatableObjectBase"/>.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like
        ///     a hash table.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override int GetHashCode() => GetHashCodeInternal();

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals([CanBeNull] EquatableObjectBase? other) => Equals(this, other);

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

        private static bool Equals([CanBeNull] EquatableObjectBase? left, [CanBeNull] EquatableObjectBase? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null || left.GetType() != right.GetType())
            {
                return false;
            }

            return left.EqualsInternal(right);
        }
    }
}