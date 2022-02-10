#nullable enable

using System.Runtime.CompilerServices;
using System.Text;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System
{
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
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [CanBeNull]
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
        ///     Avoids the specified array to be a <see langword="null"/> reference: returns the specified array
        ///     if it is not <see langword="null"/> or an empty array otherwise.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        ///     The array to secure from a <see langword="null"/> reference.
        /// </param>
        /// <returns>
        ///     The source array if it is not <see langword="null"/>; otherwise, empty array.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static T[] AvoidNull<T>([CanBeNull] this T[]? source) => source ?? Array.Empty<T>();

        /// <summary>
        ///     Converts the specified array of bytes to the hexadecimal string.
        /// </summary>
        /// <param name="byteArray">
        ///     The byte array to convert.
        /// </param>
        /// <param name="useUpperCase">
        ///     <see langword="true"/> to use upper case letter in the resulting hexadecimal string;
        ///     <see langword="false"/> to use lower case letter in the resulting hexadecimal string.
        /// </param>
        /// <returns>
        ///     A hexadecimal string.
        /// </returns>
        [NotNull]
        public static string ToHexString([NotNull] this byte[] byteArray, bool useUpperCase)
        {
            if (byteArray is null)
            {
                throw new ArgumentNullException(nameof(byteArray));
            }

            const string UpperCaseFormat = "X2";
            const string LowerCaseFormat = "x2";

            var format = useUpperCase ? UpperCaseFormat : LowerCaseFormat;
            var resultBuilder = new StringBuilder(byteArray.Length * 2);

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < byteArray.Length; index++)
            {
                var item = byteArray[index].ToString(format);
                resultBuilder.Append(item);
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        ///     Converts the specified array of bytes to the hexadecimal string (in lower case).
        /// </summary>
        /// <param name="byteArray">
        ///     The byte array to convert.
        /// </param>
        /// <returns>
        ///     A hexadecimal string (in lower case).
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static string ToHexString([NotNull] this byte[] byteArray) => ToHexString(byteArray, false);
    }
}