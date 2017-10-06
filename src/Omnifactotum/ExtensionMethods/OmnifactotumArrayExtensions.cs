using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
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
        ///     The array to copy. Can be <c>null</c>.
        /// </param>
        /// <returns>
        ///     A shallow copy of the specified array, or <c>null</c> if this array is <c>null</c>.
        /// </returns>
        [CanBeNull]
        public static T[] Copy<T>([CanBeNull] this T[] array)
        {
            return array == null ? null : (T[])array.Clone();
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
        ///     the first parameter represents the previous value of the element;
        ///     the second parameter represents the index of the element in the array.
        /// </param>
        public static void Initialize<T>([NotNull] this T[] array, [NotNull] Func<T, int, T> getElementValue)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (getElementValue == null)
            {
                throw new ArgumentNullException("getElementValue");
            }

            for (var index = 0; index < array.Length; index++)
            {
                array[index] = getElementValue(array[index], index);
            }
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
        public static void Initialize<T>([NotNull] this T[] array, [NotNull] Func<int, T> getElementValue)
        {
            if (getElementValue == null)
            {
                throw new ArgumentNullException("getElementValue");
            }

            Initialize(array, (element, index) => getElementValue(index));
        }

        /// <summary>
        ///     Avoids the specified array to be a <c>null</c> reference: returns the specified array
        ///     if it is not <c>null</c> or an empty array otherwise.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        ///     The array to secure from a <c>null</c> reference.
        /// </param>
        /// <returns>
        ///     The source array if it is not <c>null</c>; otherwise, empty array.
        /// </returns>
        [NotNull]
        public static T[] AvoidNull<T>([CanBeNull] this T[] source)
        {
            return source ?? StrongTypeHelper<T>.EmptyArray;
        }

        /// <summary>
        ///     Creates a read-only wrapper for the specified array.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of values in the array.
        /// </typeparam>
        /// <param name="array">
        ///     The array to create a read-only wrapper for.
        /// </param>
        /// <returns>
        ///     A read-only wrapper for the specified array.
        /// </returns>
        [NotNull]
        public static ReadOnlyCollection<T> AsReadOnly<T>([NotNull] this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            return new ReadOnlyCollection<T>(array);
        }

        /// <summary>
        ///     Converts the specified array of bytes to the hexadecimal string.
        /// </summary>
        /// <param name="byteArray">
        ///     The byte array to convert.
        /// </param>
        /// <param name="useUpperCase">
        ///     <c>true</c> to use upper case letter in the resulting hexadecimal string;
        ///     <c>false</c> to use lower case letter in the resulting hexadecimal string.
        /// </param>
        /// <returns>
        ///     A hexadecimal string.
        /// </returns>
        [NotNull]
        public static string ToHexString([NotNull] this byte[] byteArray, bool useUpperCase)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
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
        [NotNull]
        public static string ToHexString([NotNull] this byte[] byteArray)
        {
            return ToHexString(byteArray, false);
        }

        /// <summary>
        ///     The strong type helper.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of a value or values.
        /// </typeparam>
        private static class StrongTypeHelper<T>
        {
            /// <summary>
            ///     The empty array.
            /// </summary>
            private static volatile T[] _emptyArray;

            /// <summary>
            ///     Gets the empty array.
            /// </summary>
            [NotNull]
            public static T[] EmptyArray
            {
                [DebuggerNonUserCode]
                get
                {
                    //// Thread-safe lock is not needed here

                    // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                    if (_emptyArray == null)
                    {
                        _emptyArray = new T[0];
                    }

                    return _emptyArray;
                }
            }
        }
    }
}