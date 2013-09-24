using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Contains the extension methods for helping to compute the hash codes for objects.
    /// </summary>
    public static class OmnifactotumHashCodeHelper
    {
        #region Constants and Fields

        private const int HashCodeMagicMultiplier = 397;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Combines two specified hash code values into a new hash code.
        /// </summary>
        /// <param name="previousHashCode">
        ///     The hash code of a value which is considered as previous.
        /// </param>
        /// <param name="nextHashCode">
        ///     The hash code of a certain value which is considered as next.
        /// </param>
        /// <returns>
        ///     A new hash code produced from the specified hash codes.
        /// </returns>
        public static int CombineHashCodeValues(this int previousHashCode, int nextHashCode)
        {
            return nextHashCode == 0
                ? previousHashCode
                : unchecked(previousHashCode * HashCodeMagicMultiplier) ^ nextHashCode;
        }

        /// <summary>
        ///     Combines hash code obtained from two specified instances into a new hash code.
        /// </summary>
        /// <typeparam name="TPreviuos">
        ///     The type of an instance which is considered as previous.
        /// </typeparam>
        /// <typeparam name="TNext">
        ///     The type of an instance which is considered as next.
        /// </typeparam>
        /// <param name="previous">
        ///     A certain previous instance to get the hash code from.
        /// </param>
        /// <param name="next">
        ///     A certain next instance to get the hash code from.
        /// </param>
        /// <returns>
        ///     A new hash code produced from the hash codes obtained from the specified instances.
        /// </returns>
        public static int CombineHashCodes<TPreviuos, TNext>(this TPreviuos previous, TNext next)
        {
            return CombineHashCodeValues(previous.GetHashCodeSafely(), next.GetHashCodeSafely());
        }

        /// <summary>
        ///     Combines a hash code obtained at a previous step and a hash code of the specified instance
        ///     into a new hash code.
        /// </summary>
        /// <typeparam name="TNext">
        ///     The type of an instance which is considered as next.
        /// </typeparam>
        /// <param name="previousHashCode">
        ///     The hash code of a value which is considered as previous.
        /// </param>
        /// <param name="next">
        ///     A certain next instance to get the hash code from.
        /// </param>
        /// <returns>
        ///     A new hash code produced from a hash code obtained at a previous step and a hash code of
        ///     the specified instance.
        /// </returns>
        public static int CombineHashCodes<TNext>(this int previousHashCode, TNext next)
        {
            return CombineHashCodeValues(previousHashCode, next.GetHashCodeSafely());
        }

        #endregion
    }
}