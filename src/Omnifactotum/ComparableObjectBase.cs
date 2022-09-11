using System;
using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the abstract container that supports comparison.
    /// </summary>
    public abstract class ComparableObjectBase : EquatableObjectBase, IComparable<ComparableObjectBase>, IComparable
    {
        /// <summary>
        ///     Determines whether the left <see cref="ComparableObjectBase"/> instance is less than
        ///     the right <see cref="ComparableObjectBase"/> instance.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the left <see cref="ComparableObjectBase"/> instance is less than
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <([CanBeNull] ComparableObjectBase? left, [CanBeNull] ComparableObjectBase? right) => CompareObjects(left, right) < 0;

        /// <summary>
        ///     Determines whether the left <see cref="ComparableObjectBase"/> instance is less than or equal
        ///     to the right <see cref="ComparableObjectBase"/> instance.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the left <see cref="ComparableObjectBase"/> instance is less than or equal to
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=([CanBeNull] ComparableObjectBase? left, [CanBeNull] ComparableObjectBase? right) => CompareObjects(left, right) <= 0;

        /// <summary>
        ///     Determines whether the left <see cref="ComparableObjectBase"/> instance is greater than
        ///     the right <see cref="ComparableObjectBase"/> instance.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the left <see cref="ComparableObjectBase"/> instance is greater than
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >([CanBeNull] ComparableObjectBase? left, [CanBeNull] ComparableObjectBase? right) => CompareObjects(left, right) > 0;

        /// <summary>
        ///     Determines whether the left <see cref="ComparableObjectBase"/> instance is greater than or equal
        ///     to the right <see cref="ComparableObjectBase"/> instance.
        /// </summary>
        /// <param name="left">
        ///     The first <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <param name="right">
        ///     The second <see cref="ComparableObjectBase"/> instance to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the left <see cref="ComparableObjectBase"/> instance is greater than or equal to
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=([CanBeNull] ComparableObjectBase? left, [CanBeNull] ComparableObjectBase? right) => CompareObjects(left, right) >= 0;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo([CanBeNull] ComparableObjectBase? other) => CompareObjects(this, other);

        /// <inheritdoc />
        int IComparable.CompareTo(object? obj)
        {
            if (obj is null)
            {
                return CompareObjects(this, null);
            }

            if (obj is not ComparableObjectBase castObj)
            {
                throw new ArgumentException($@"Incompatible comparand type: {obj.GetType().GetFullName().ToUIString()}.", nameof(obj));
            }

            return CompareObjects(this, castObj);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected sealed override bool EqualsInternal(EquatableObjectBase other) => CompareToInternal((ComparableObjectBase)other) == 0;

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared.
        ///     See <see cref="IComparable{T}.CompareTo"/>.
        /// </returns>
        protected abstract int CompareToInternal([NotNull] ComparableObjectBase other);

        private static int CompareObjects([CanBeNull] ComparableObjectBase? left, [CanBeNull] ComparableObjectBase? right)
        {
            if (ReferenceEquals(left, right))
            {
                return 0;
            }

            if (left is null)
            {
                return -1;
            }

            if (right is null)
            {
                return 1;
            }

            var leftType = left.GetType();
            var rightType = right.GetType();

            if (leftType != rightType)
            {
                throw new ArgumentException(
                    $@"Incompatible comparand types: {leftType.GetFullName().ToUIString()} and {rightType.GetFullName().ToUIString()}.");
            }

            return left.CompareToInternal(right);
        }
    }
}