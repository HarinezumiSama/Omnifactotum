using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the determinant for the <see cref="EnumFixedSizeDictionary{TKey,TValue}"/> class.
    /// </summary>
    public sealed class EnumFixedSizeDictionaryDeterminant<TKey> : FixedSizeDictionaryDeterminant<TKey>
        where TKey : struct
    {
        private readonly Func<TKey, int> _getIndex;
        private readonly Func<int, TKey> _getKey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EnumFixedSizeDictionaryDeterminant{TKey}"/> class.
        /// </summary>
        public EnumFixedSizeDictionaryDeterminant()
        {
            const int MinValue = 0;
            const int MaxValue = int.MaxValue;

            var type = typeof(TKey);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException(
                    AsInvariant($@"The type {type.GetFullName().ToUIString()} is not an enumeration."));
            }

            var values = Enum.GetValues(type).Cast<object>().ToArray();
            var minObject = values.Min();
            var maxObject = values.Max();

            int? upperBound;
            if (Enum.GetUnderlyingType(type) == typeof(ulong))
            {
                var max = Convert.ToUInt64(maxObject);
                upperBound = max <= MaxValue ? Convert.ToInt32(max) : null;
            }
            else
            {
                var min = Convert.ToInt64(minObject);
                var max = Convert.ToInt64(maxObject);
                upperBound = min >= MinValue && max <= MaxValue ? Convert.ToInt32(max) : null;
            }

            if (!upperBound.HasValue)
            {
                throw new InvalidOperationException(
                    AsInvariant($@"The values of the enumeration {type.GetFullName().ToUIString()} must be in the valid range ({
                        MinValue} to {MaxValue})."));
            }

            Size = upperBound.Value + 1;
            _getIndex = CreateConversionMethod<TKey, int>();
            _getKey = CreateConversionMethod<int, TKey>();
        }

        /// <summary>
        ///     Gets the constant size of an internal array used in
        ///     the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
        /// </summary>
        public override int Size
        {
            [DebuggerStepThrough]
            get;
        }

        /// <summary>
        ///     Gets the internal index corresponding to the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to get the index of.
        /// </param>
        /// <returns>
        ///     The internal index corresponding to the specified key.
        /// </returns>
        public override int GetIndex(TKey key) => _getIndex(key);

        /// <summary>
        ///     Gets the key corresponding to the specified internal index.
        /// </summary>
        /// <param name="index">
        ///     The index to get the key for.
        /// </param>
        /// <returns>
        ///     The key corresponding to the specified internal index.
        /// </returns>
        public override TKey GetKey(int index) => _getKey(index);

        private static Func<TSource, TTarget> CreateConversionMethod<TSource, TTarget>()
        {
            var parameter = Expression.Parameter(typeof(TSource));
            var convert = Expression.Convert(parameter, typeof(TTarget));
            return Expression.Lambda<Func<TSource, TTarget>>(convert, parameter).Compile();
        }
    }
}