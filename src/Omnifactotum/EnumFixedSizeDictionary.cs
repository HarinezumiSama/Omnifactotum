using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents a generic dictionary which is internally handled by a fixed size array due to limitation for
    ///     the maximum number of possible keys, and whose keys are enumeration values.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the keys in the dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the values in the dictionary.
    /// </typeparam>
    public sealed class EnumFixedSizeDictionary<TKey, TValue>
        : FixedSizeDictionary<TKey, TValue, EnumFixedSizeDictionaryDeterminant<TKey>>
        where TKey : struct
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class.
        /// </summary>
        public EnumFixedSizeDictionary()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class
        ///     by copying the key/values pairs from the specified dictionary.
        /// </summary>
        /// <param name="dictionary">
        ///     The dictionary to copy the key/values pairs from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dictionary"/> is <c>null</c>.
        /// </exception>
        public EnumFixedSizeDictionary([NotNull] IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
            // Nothing to do
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class
        ///     by copying the key/values pairs from the specified dictionary.
        /// </summary>
        /// <param name="dictionary">
        ///     The dictionary to copy the key/values pairs from.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="dictionary"/> is <c>null</c>.
        /// </exception>
        public EnumFixedSizeDictionary(
            [NotNull] FixedSizeDictionary<TKey, TValue, EnumFixedSizeDictionaryDeterminant<TKey>> dictionary)
            : base(dictionary)
        {
            // Nothing to do
        }

        #endregion
    }
}