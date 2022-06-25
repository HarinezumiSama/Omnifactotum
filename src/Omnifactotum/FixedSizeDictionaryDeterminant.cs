#nullable enable

using System;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum
{
    /// <summary>
    ///     Specifies the size of a <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/> as well as mapping
    ///     between keys and internal indexes.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the key in the <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
    /// </typeparam>
    public abstract class FixedSizeDictionaryDeterminant<TKey>
    {
        /// <summary>
        ///     Gets the validated value of <see cref="Size"/>.
        /// </summary>
        public int ValidatedSize
        {
            get
            {
                var size = Size;
                if (size <= 0)
                {
                    throw new InvalidOperationException(
                        AsInvariant(
                            $@"The determinant {GetType().GetFullName().ToUIString()} returned invalid size: {size}. The size must be greater than zero."));
                }

                return size;
            }
        }

        /// <summary>
        ///     Gets the constant size of an internal array used in <see cref="FixedSizeDictionary{TKey,TValue,TDeterminant}"/>.
        /// </summary>
        protected abstract int Size { get; }

        /// <summary>
        ///     Gets the internal index corresponding to the specified key.
        /// </summary>
        /// <param name="key">
        ///     The key to get the index of.
        /// </param>
        /// <returns>
        ///     The internal index corresponding to the specified key.
        /// </returns>
        public abstract int GetIndex(TKey key);

        /// <summary>
        ///     Gets the key corresponding to the specified internal index.
        /// </summary>
        /// <param name="index">
        ///     The index to get the key for.
        /// </param>
        /// <returns>
        ///     The key corresponding to the specified internal index.
        /// </returns>
        public abstract TKey GetKey(int index);
    }
}