using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper functionality for creating instances of
    ///     the <see cref="ValueRange{T}"/> type using type inference in a friendly way.
    /// </summary>
    public static class ValueRange
    {
        #region Public Methods

        /// <summary>
        ///     Creates a new instance of the <see cref="ValueRange{T}"/> structure using the specified values.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value.
        /// </typeparam>
        /// <param name="lower">
        ///     The lower boundary of the range.
        /// </param>
        /// <param name="upper">
        ///     The upper boundary of the range.
        /// </param>
        /// <returns>
        ///     A new instance of the <see cref="ValueRange{T}"/> class.
        /// </returns>
        public static ValueRange<T> Create<T>(T lower, T upper)
            where T : IComparable
        {
            return new ValueRange<T>(lower, upper);
        }

        #endregion
    }
}