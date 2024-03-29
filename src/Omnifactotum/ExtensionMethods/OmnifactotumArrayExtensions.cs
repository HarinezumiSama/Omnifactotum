﻿using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for array types.
/// </summary>
public static class OmnifactotumArrayExtensions
{
    /// <summary>
    ///     Creates a shallow copy of the specified array.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the array.
    /// </typeparam>
    /// <param name="array">
    ///     The array to copy. Can be <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     A shallow copy of the specified array, or <see langword="null"/> if this array is <see langword="null"/>.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [CanBeNull]
    [return: NotNullIfNotNull(nameof(array))]
    public static T[]? Copy<T>([CanBeNull] this T[]? array) => (T[]?)array?.Clone();

    /// <summary>
    ///     Initializes all the elements of the specified array using the specified method to initialize
    ///     each particular element.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the array.
    /// </typeparam>
    /// <param name="array">
    ///     The array whose elements to initialize.
    /// </param>
    /// <param name="getElementValue">
    ///     A reference to a method that returns a new value for each array's element;
    ///     the first parameter represents the previous value of the element;
    ///     the second parameter represents the index of the element in the array.
    /// </param>
    [NotNull]
    public static T[] Initialize<T>([NotNull] this T[] array, [NotNull] [InstantHandle] Func<T, int, T> getElementValue)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (getElementValue is null)
        {
            throw new ArgumentNullException(nameof(getElementValue));
        }

        for (var index = 0; index < array.Length; index++)
        {
            array[index] = getElementValue(array[index], index);
        }

        return array;
    }

    /// <summary>
    ///     Initializes all the elements of the specified array using the specified method to initialize
    ///     each particular element.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the array.
    /// </typeparam>
    /// <param name="array">
    ///     The array whose elements to initialize.
    /// </param>
    /// <param name="getElementValue">
    ///     A reference to a method that returns a new value for each array's element;
    ///     the parameter represents the index of the element in the array.
    /// </param>
    [NotNull]
    public static T[] Initialize<T>([NotNull] this T[] array, [NotNull] [InstantHandle] Func<int, T> getElementValue)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (getElementValue is null)
        {
            throw new ArgumentNullException(nameof(getElementValue));
        }

        for (var index = 0; index < array.Length; index++)
        {
            array[index] = getElementValue(index);
        }

        return array;
    }

    /// <summary>
    ///     Initializes all the elements of the specified array with the specified value.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the elements in the array.
    /// </typeparam>
    /// <param name="array">
    ///     The array whose elements to initialize.
    /// </param>
    /// <param name="value">
    ///     The value to set to all the elements of the specified array.
    /// </param>
    [NotNull]
    public static T[] Initialize<T>([NotNull] this T[] array, T value)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        for (var index = 0; index < array.Length; index++)
        {
            array[index] = value;
        }

        return array;
    }

    /// <summary>
    ///     Avoids the specified array being a <see langword="null"/> reference.
    ///     Returns the specified array if it is not <see langword="null"/>; otherwise, returns an empty array.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the array.
    /// </typeparam>
    /// <param name="source">
    ///     The array to secure from a <see langword="null"/> reference.
    /// </param>
    /// <returns>
    ///     The source array if it is not <see langword="null"/>; otherwise, an empty array.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static T[] AvoidNull<T>([CanBeNull] this T[]? source) => source ?? Array.Empty<T>();

    /// <summary>
    ///     Converts the specified array of bytes to its equivalent string representation that is encoded with hexadecimal characters.
    /// </summary>
    /// <param name="bytes">
    ///     The byte array to convert.
    /// </param>
    /// <param name="separator">
    ///     An optional separator used to split hexadecimal representation of each byte.
    /// </param>
    /// <param name="upperCase">
    ///     <see langword="true"/> to use upper case letters (<c>A..F</c>) in the resulting hexadecimal string;
    ///     <see langword="false"/> to use lower case letters (<c>a..f</c>) in the resulting hexadecimal string.
    /// </param>
    /// <returns>
    ///     A hexadecimal string representation of the specified array of bytes.
    /// </returns>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static string ToHexString([NotNull] this byte[] bytes, [CanBeNull] string? separator = null, bool upperCase = false)
    {
        if (bytes is null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        return new ReadOnlySpan<byte>(bytes).ToHexString(separator, upperCase);
    }
}