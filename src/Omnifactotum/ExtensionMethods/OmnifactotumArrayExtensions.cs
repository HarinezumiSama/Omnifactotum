using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    ///     Contains extension methods for array types.
    /// </summary>
    public static class OmnifactotumArrayExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Creates a shallow copy of the specified array.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the elements in the array.
        /// </typeparam>
        /// <param name="array">
        ///     The array to copy. Can be <b>null</b>.
        /// </param>
        /// <returns>
        ///     A shallow copy of the specified array, or <b>null</b> if this array is <b>null</b>.
        /// </returns>
        public static T[] Copy<T>(this T[] array)
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
        public static void Initialize<T>(this T[] array, Func<T, int, T> getElementValue)
        {
            #region Argument Check

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (getElementValue == null)
            {
                throw new ArgumentNullException("getElementValue");
            }

            #endregion

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
        public static void Initialize<T>(this T[] array, Func<int, T> getElementValue)
        {
            #region Argument Check

            if (getElementValue == null)
            {
                throw new ArgumentNullException("getElementValue");
            }

            #endregion

            Initialize(array, (element, index) => getElementValue(index));
        }

        /// <summary>
        ///     Avoids the specified array to be a <b>null</b> reference: returns the specified array
        ///     if it is not <b>null</b> or an empty array otherwise.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of elements in the array.
        /// </typeparam>
        /// <param name="source">
        ///     The array to secure from a <b>null</b> reference.
        /// </param>
        /// <returns>
        ///     The source array if it is not <b>null</b>; otherwise, empty array.
        /// </returns>
        public static T[] AvoidNull<T>(this T[] source)
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
        public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            #region Argument Check

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            #endregion

            return new ReadOnlyCollection<T>(array);
        }

        /// <summary>
        ///     Converts the specified array of bytes to the hexadecimal string.
        /// </summary>
        /// <param name="byteArray">
        ///     The byte array to convert.
        /// </param>
        /// <param name="useUpperCase">
        ///     <b>true</b> to use upper case letter in the resulting hexadecimal string;
        ///     <b>false</b> to use lower case letter in the resulting hexadecimal string.
        /// </param>
        /// <returns>
        ///     A hexadecimal string.
        /// </returns>
        public static string ToHexString(this byte[] byteArray, bool useUpperCase)
        {
            #region Argument Check

            if (byteArray == null)
            {
                throw new ArgumentNullException("byteArray");
            }

            #endregion

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
        public static string ToHexString(this byte[] byteArray)
        {
            return ToHexString(byteArray, false);
        }

        #endregion

        #region StrongTypeHelper<T> Class

        /// <summary>
        ///     The strong type helper.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of a value or values.
        /// </typeparam>
        private static class StrongTypeHelper<T>
        {
            #region Constants and Fields

            /// <summary>
            ///     The empty array.
            /// </summary>
            private static T[] _emptyArray;

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the empty array.
            /// </summary>
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

            #endregion
        }

        #endregion
    }
}