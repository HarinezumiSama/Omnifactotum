using System;
using System.Diagnostics;
using System.Linq;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System.Collections.Generic

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Contains the extension methods for helping to compute the hash codes for collections of objects.
    /// </summary>
    public static class OmnifactotumCollectionHashCodeHelper
    {
        #region Public Methods

        /// <summary>
        ///     Computes the hash code of the specified collection by combining hash codes of the elements
        ///     in the collection into a new hash code.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the collection.
        /// </typeparam>
        /// <param name="collection">
        ///     The collection to compute a hash code of.
        /// </param>
        /// <returns>
        ///     A hash code of the specified collection.
        /// </returns>
        public static int ComputeCollectionHashCode<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return 0;
            }

            return collection.Aggregate(
                0,
                (current, item) => current.CombineHashCodeValues(item.GetHashCodeSafely()));
        }

        #endregion
    }
}