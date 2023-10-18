using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

namespace Omnifactotum;

/// <summary>
///     Contains extension methods for the <see cref="ValueRange{T}"/> structure.
/// </summary>
public static class ValueRangeExtensions
{
    /// <summary>
    ///     Enumerates the specified range from <see cref="ValueRange{T}.Lower"/> to <see cref="ValueRange{T}.Upper"/> using the specified delegate to get
    ///     a next value.
    /// </summary>
    /// <param name="range">
    ///     The range to enumerate.
    /// </param>
    /// <param name="getNext">
    ///     A reference to a method that returns a next value in the collection given the current value.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the values in the range.
    /// </typeparam>
    /// <returns>
    ///     A sequence containing values from the range.
    /// </returns>
    public static IEnumerable<T> Enumerate<T>(this ValueRange<T> range, [NotNull] Func<T, T> getNext)
        where T : IComparable
    {
        if (getNext is null)
        {
            throw new ArgumentNullException(nameof(getNext));
        }

        var valueComparer = ValueRange<T>.ValueComparer;

        var current = range.Lower;
        do
        {
            yield return current;

            var previous = current;
            current = getNext(previous);
            if (valueComparer.Compare(current, previous) <= 0)
            {
                throw new ArgumentException(
                    AsInvariant($"The next value ({current}) is less than or equal to the previous value ({previous})."),
                    nameof(getNext));
            }
        }
        while (valueComparer.Compare(current, range.Upper) <= 0);
    }

#if NET7_0_OR_GREATER
    /// <summary>
    ///     Enumerates the specified range from <see cref="ValueRange{T}.Lower"/> to <see cref="ValueRange{T}.Upper"/> for the value type
    ///     that implements increment and comparison operators.
    /// </summary>
    /// <param name="range">
    ///     The range to enumerate.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the values in the range.
    /// </typeparam>
    /// <returns>
    ///     A sequence containing values from the range.
    /// </returns>
    public static IEnumerable<T> Enumerate<T>(this ValueRange<T> range)
        where T : IBinaryInteger<T>
    {
        var current = range.Lower;
        while (current <= range.Upper)
        {
            yield return current++;
        }
    }

    /// <summary>
    ///     Returns an array containing values from the specified range from <see cref="ValueRange{T}.Lower"/> to <see cref="ValueRange{T}.Upper"/>.
    /// </summary>
    /// <param name="range">
    ///     The range to create an array from.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the values in the range.
    /// </typeparam>
    /// <returns>
    ///     An array containing values from the range.
    /// </returns>
    [CLSCompliant(false)]
    public static T[] ToArray<T>(this ValueRange<T> range)
        where T : IBinaryInteger<T>, IConvertible
    {
        var distance = range.Upper - range.Lower;
        var typeCode = distance.GetTypeCode();

        var castDistance = typeCode switch
        {
            TypeCode.Empty
                or TypeCode.Object
                or TypeCode.DBNull
                or TypeCode.Boolean
                or TypeCode.Single
                or TypeCode.Double
                or TypeCode.Decimal
                or TypeCode.DateTime
                or TypeCode.String
                => throw typeCode.CreateEnumValueNotSupportedException(),

            TypeCode.Char
                or TypeCode.SByte
                or TypeCode.Byte
                or TypeCode.Int16
                or TypeCode.UInt16
                or TypeCode.Int32
                or TypeCode.UInt32
                or TypeCode.Int64
                or TypeCode.UInt64
                => Convert.ToInt32(distance),

            _ => throw typeCode.CreateEnumValueNotImplementedException()
        };

        var length = checked(castDistance + 1);
        var result = new T[length];

        var current = range.Lower;
        var arrayIndex = 0;
        while (current <= range.Upper)
        {
            result[arrayIndex++] = current++;
        }

        return result;
    }
#endif
}