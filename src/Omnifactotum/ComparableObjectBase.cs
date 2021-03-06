﻿using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the abstract container that supports comparison.
    /// </summary>
    public abstract class ComparableObjectBase : EquatableObjectBase, IComparable<ComparableObjectBase>
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
        ///     <c>true</c> if the left <see cref="ComparableObjectBase"/> instance is less than
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <([CanBeNull] ComparableObjectBase left, [CanBeNull] ComparableObjectBase right)
        {
            return Comparer<ComparableObjectBase>.Default.Compare(left, right) < 0;
        }

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
        ///     <c>true</c> if the left <see cref="ComparableObjectBase"/> instance is less than or equal to
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <=([CanBeNull] ComparableObjectBase left, [CanBeNull] ComparableObjectBase right)
        {
            return Comparer<ComparableObjectBase>.Default.Compare(left, right) <= 0;
        }

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
        ///     <c>true</c> if the left <see cref="ComparableObjectBase"/> instance is greater than
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >([CanBeNull] ComparableObjectBase left, [CanBeNull] ComparableObjectBase right)
        {
            return Comparer<ComparableObjectBase>.Default.Compare(left, right) > 0;
        }

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
        ///     <c>true</c> if the left <see cref="ComparableObjectBase"/> instance is greater than or equal to
        ///     the right <see cref="ComparableObjectBase"/> instance; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >=([CanBeNull] ComparableObjectBase left, [CanBeNull] ComparableObjectBase right)
        {
            return Comparer<ComparableObjectBase>.Default.Compare(left, right) >= 0;
        }

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
        /// <exception cref="ArgumentException">
        ///     The <paramref name="other"/> object's type differs from this object's type.
        /// </exception>
        public int CompareTo([CanBeNull] ComparableObjectBase other)
        {
            if (other is null)
            {
                return 1;
            }

            if (other.GetType() != GetType())
            {
                throw new ArgumentException("Incompatible comparand type.", nameof(other));
            }

            return CompareToInternal(other);
        }

        /// <summary>
        ///     <para>
        ///         Determines whether the current object is equal to another object of the same type.
        ///     </para>
        ///     <para>
        ///         <b>IMPORTANT NOTE</b>: Default implementation of this method simply calls
        ///         the <see cref="ComparableObjectBase.CompareToInternal"/> method, which may not always be
        ///         the best option from the performance perspective.
        ///     </para>
        /// </summary>
        /// <param name="other">
        ///     An object to compare with this object. The parameter is checked prior to calling this method, therefore
        ///     it is never <c>null</c> and can also be safely cast to an actual derived type.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current object is equal to the <paramref name="other"/> parameter;
        ///     otherwise, <c>false</c>.
        /// </returns>
        protected override bool EqualsInternal(EquatableObjectBase other)
        {
            return CompareToInternal((ComparableObjectBase)other) == 0;
        }

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
    }
}