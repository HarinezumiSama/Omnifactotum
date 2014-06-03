using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
        : FixedSizeDictionary<TKey, TValue, EnumFixedSizeDictionary<TKey, TValue>.EnumDeterminant>
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
        ///     <paramref name="dictionary"/> is <b>null</b>.
        /// </exception>
        //// ReSharper disable once ParameterTypeCanBeEnumerable.Local - By design
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
        ///     <paramref name="dictionary"/> is <b>null</b>.
        /// </exception>
        public EnumFixedSizeDictionary([NotNull] FixedSizeDictionary<TKey, TValue, EnumDeterminant> dictionary)
            : base(dictionary)
        {
            // Nothing to do
        }

        #endregion

        #region EnumDeterminant Class

        /// <summary>
        ///     Represents the determinant for the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class.
        /// </summary>
        public sealed class EnumDeterminant : FixedSizeDictionaryDeterminant<TKey>
        {
            #region Constants and Fields

            private readonly int _size;
            private readonly Func<TKey, int> _getIndex;
            private readonly Func<int, TKey> _getKey;

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="EnumDeterminant"/> class.
            /// </summary>
            public EnumDeterminant()
            {
                const int MinValue = 0;
                const int MaxValue = int.MaxValue;

                var type = typeof(TKey);
                if (!type.IsEnum)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The type '{0}' is not an enumeration.",
                            type.GetFullName()));
                }

                var values = Enum.GetValues(type).Cast<object>().ToArray();
                var minObject = values.Min();
                var maxObject = values.Max();

                int? size;
                if (Enum.GetUnderlyingType(type) == typeof(ulong))
                {
                    var max = Convert.ToUInt64(maxObject);
                    size = max <= MaxValue ? Convert.ToInt32(max) : (int?)null;
                }
                else
                {
                    var min = Convert.ToInt64(minObject);
                    var max = Convert.ToInt64(maxObject);
                    size = min >= MinValue && max <= MaxValue ? Convert.ToInt32(max) : (int?)null;
                }

                if (!size.HasValue)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The values of the enumeration '{0}' must be in the valid range ({1} to {2}).",
                            type.GetFullName(),
                            MinValue,
                            MaxValue));
                }

                _size = size.Value;
                _getIndex = CreateConversionMethod<TKey, int>();
                _getKey = CreateConversionMethod<int, TKey>();
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the constant size of an internal array used in
            ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
            /// </summary>
            public override int Size
            {
                [DebuggerStepThrough]
                get
                {
                    return _size;
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///     Gets the internal index corresponding to the specified key.
            /// </summary>
            /// <param name="key">
            ///     The key to get the index of.
            /// </param>
            /// <returns>
            ///     The internal index corresponding to the specified key.
            /// </returns>
            public override int GetIndex(TKey key)
            {
                return _getIndex(key);
            }

            /// <summary>
            ///     Gets the key corresponding to the specified internal index.
            /// </summary>
            /// <param name="index">
            ///     The index to get the key for.
            /// </param>
            /// <returns>
            ///     The key corresponding to the specified internal index.
            /// </returns>
            public override TKey GetKey(int index)
            {
                return _getKey(index);
            }

            #endregion

            #region Private Methods

            private static Func<TSource, TTarget> CreateConversionMethod<TSource, TTarget>()
            {
                var parameter = Expression.Parameter(typeof(TSource));
                var convert = Expression.Convert(parameter, typeof(TTarget));
                return Expression.Lambda<Func<TSource, TTarget>>(convert, parameter).Compile();
            }

            #endregion
        }

        #endregion
    }
}