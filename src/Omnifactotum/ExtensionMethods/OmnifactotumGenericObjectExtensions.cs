using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Omnifactotum;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace

namespace System
{
    /// <summary>
    ///     Contains extension methods for any type.
    /// </summary>
    public static class OmnifactotumGenericObjectExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Returns the specified value if is not <c>null</c>;
        ///     otherwise, throws <see cref="System.ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The reference type of the value to check.
        /// </typeparam>
        /// <param name="value">
        ///     The value to check.
        /// </param>
        /// <returns>
        ///     The specified value if is not <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="value"/> is <c>null</c>.
        /// </exception>
        [NotNull]
        public static T EnsureNotNull<T>([CanBeNull] this T value)
            where T : class
        {
            #region Argument Check

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            #endregion

            return value;
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents the specified value, considering that this value
        ///     may be <c>null</c>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <param name="nullValueString">
        ///     A <see cref="System.String"/> to return if <paramref name="value"/> is <c>null</c>.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value, or the value of
        ///     the <paramref name="nullValueString"/> parameter if <paramref name="value"/> is <c>null</c>.
        /// </returns>
        public static string ToStringSafely<T>([CanBeNull] this T value, [CanBeNull] string nullValueString)
        {
            return ReferenceEquals(value, null) ? nullValueString : value.ToString();
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents the specified value, considering that this value
        ///     may be <c>null</c>. In the latter case, the empty string is returned.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value it is not <c>null</c>;
        ///     otherwise, the empty string.
        /// </returns>
        public static string ToStringSafely<T>([CanBeNull] this T value)
        {
            return ToStringSafely(value, string.Empty);
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents the specified value, using invariant culture and
        ///     considering that this value may be <c>null</c>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <param name="nullValueString">
        ///     A <see cref="System.String"/> to return if <paramref name="value"/> is <c>null</c>.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value, or the value of
        ///     the <paramref name="nullValueString"/> parameter if <paramref name="value"/> is <c>null</c>.
        /// </returns>
        public static string ToStringSafelyInvariant<T>([CanBeNull] this T value, [CanBeNull] string nullValueString)
        {
            if (ReferenceEquals(value, null))
            {
                return nullValueString;
            }

            var formattable = value as IFormattable;
            return formattable == null ? value.ToString() : formattable.ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Returns a <see cref="System.String"/> that represents the specified value, using invariant culture and
        ///     considering that this value may be <c>null</c>.
        ///     If the value is <c>null</c>, the empty string is returned.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value it is not <c>null</c>;
        ///     otherwise, the empty string.
        /// </returns>
        public static string ToStringSafelyInvariant<T>([CanBeNull] this T value)
        {
            return ToStringSafelyInvariant(value, string.Empty);
        }

        /// <summary>
        ///     Gets a hash code of the specified value safely, that is, <n>null</n> does not cause an exception.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a hash code of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a hash code of. Can be <c>null</c>.
        /// </param>
        /// <param name="nullValueHashCode">
        ///     The value to return if the <paramref name="value"/> parameter is <c>null</c>.
        /// </param>
        /// <returns>
        ///     A hash code of the specified value obtained by calling <see cref="object.GetHashCode"/> for this value
        ///     if it is not <c>null</c>; otherwise, the value specified in the <paramref name="nullValueHashCode"/>
        ///     parameter.
        /// </returns>
        public static int GetHashCodeSafely<T>([CanBeNull] this T value, int nullValueHashCode)
        {
            return ReferenceEquals(value, null) ? nullValueHashCode : value.GetHashCode();
        }

        /// <summary>
        ///     Gets a hash code of the specified value safely, that is, <n>null</n> does not cause an exception.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a hash code of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a hash code of. Can be <c>null</c>.
        /// </param>
        /// <returns>
        ///     A hash code of the specified value obtained by calling <see cref="object.GetHashCode"/> for this value
        ///     if it is not <c>null</c>; otherwise, <c>0</c>.
        /// </returns>
        public static int GetHashCodeSafely<T>([CanBeNull] this T value)
        {
            return GetHashCodeSafely(value, 0);
        }

        /// <summary>
        ///     Gets the type of the specified value, considering that this value may be <c>null</c>.
        ///     In the latter case, the formally specified type <typeparamref name="T"/> is returned.
        /// </summary>
        /// <typeparam name="T">
        ///     The formal type of the value.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get the type of.
        /// </param>
        /// <returns>
        ///     The actual type of the value if it is not <c>null</c>; otherwise, <typeparamref name="T"/>.
        /// </returns>
        public static Type GetTypeSafely<T>([CanBeNull] this T value)
        {
            return ReferenceEquals(value, null) ? typeof(T) : value.GetType();
        }

        /// <summary>
        ///     Creates an array containing the specified value as its sole element.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the input value and elements of the resulting array.
        /// </typeparam>
        /// <param name="value">
        ///     The value to create an array from.
        /// </param>
        /// <returns>
        ///     An array containing the specified value as its sole element.
        /// </returns>
        [NotNull]
        public static T[] AsArray<T>([CanBeNull] this T value)
        {
            return new[] { value };
        }

        /// <summary>
        ///     Creates a strongly-typed list containing the specified value as its sole element.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the input value and elements of the resulting list.
        /// </typeparam>
        /// <param name="value">
        ///     The value to create a list from.
        /// </param>
        /// <returns>
        ///     A strongly-typed list containing the specified value as its sole element.
        /// </returns>
        [NotNull]
        public static List<T> AsList<T>([CanBeNull] this T value)
        {
            return new List<T> { value };
        }

        /// <summary>
        ///     Gets a strongly-typed collection containing the specified value as its sole element.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the input value and elements of the resulting collection.
        /// </typeparam>
        /// <param name="value">
        ///     The value to create a collection from.
        /// </param>
        /// <returns>
        ///     A strongly-typed collection containing the specified value as its sole element.
        /// </returns>
        [NotNull]
        public static IEnumerable<T> AsCollection<T>([CanBeNull] this T value)
        {
            yield return value;
        }

        /// <summary>
        ///     Avoids the specified reference type value to be a <c>null</c> reference: returns the specified value
        ///     if it is not <c>null</c> or a default value which is returned by the specified ad-hoc method.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to handle.
        /// </typeparam>
        /// <param name="source">
        ///     The value to secure from a <c>null</c> reference.
        /// </param>
        /// <param name="getDefault">
        ///     The method that will return the default value to use instead of <c>null</c>.
        /// </param>
        /// <returns>
        ///     The string value if it is not <c>null</c>; otherwise, the value returned from call to
        ///     <paramref name="getDefault"/> method.
        /// </returns>
        [NotNull]
        public static T AvoidNull<T>([CanBeNull] this T source, Func<T> getDefault)
            where T : class
        {
            #region Argument Check

            if (getDefault == null)
            {
                throw new ArgumentNullException("getDefault");
            }

            #endregion

            var result = source ?? getDefault();
            if (result == null)
            {
                throw new InvalidOperationException(
                    "The method that had to return non-null value returned null.");
            }

            return result;
        }

        /// <summary>
        ///     Converts the specified nullable value its UI representation.
        /// </summary>
        /// <typeparam name="T">
        ///     The underlying value type of the nullable value.
        /// </typeparam>
        /// <param name="value">
        ///     The nullable value to convert.
        /// </param>
        /// <returns>
        ///     The UI representation of the specified nullable value.
        /// </returns>
        public static string ToUIString<T>([CanBeNull] this T? value)
            where T : struct
        {
            return value.HasValue ? value.Value.ToString() : OmnifactotumStringExtensions.NullString;
        }

        /// <summary>
        ///     Converts the specified nullable value its UI representation using the specified format provider.
        /// </summary>
        /// <typeparam name="T">
        ///     The underlying value type of the nullable value.
        /// </typeparam>
        /// <param name="value">
        ///     The nullable value to convert.
        /// </param>
        /// <param name="formatProvider">
        ///     The provider to use to format the value, or <c>null</c> to obtain the format information
        ///     from the current locale setting of the operating system.
        /// </param>
        /// <returns>
        ///     The UI representation of the specified nullable value.
        /// </returns>
        public static string ToUIString<T>([CanBeNull] this T? value, IFormatProvider formatProvider)
            where T : struct, IFormattable
        {
            return value.HasValue
                ? value.Value.ToString(null, formatProvider)
                : OmnifactotumStringExtensions.NullString;
        }

        /// <summary>
        ///     Gets a string representing the properties of the specified object.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the object to convert.
        /// </typeparam>
        /// <param name="obj">
        ///     The object to convert.
        /// </param>
        /// <param name="options">
        ///     The options specifying how to build the string representation.
        /// </param>
        /// <returns>
        ///     A string representing the properties of the specified object.
        /// </returns>
        public static string ToPropertyString<T>([CanBeNull] this T obj, [CanBeNull] ToPropertyStringOptions options)
        {
            return Factotum.ToPropertyString(obj, options);
        }

        /// <summary>
        ///     Gets a string representing the properties of the specified object.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the object to convert.
        /// </typeparam>
        /// <param name="obj">
        ///     The object to convert.
        /// </param>
        /// <returns>
        ///     A string representing the properties of the specified object.
        /// </returns>
        public static string ToPropertyString<T>([CanBeNull] this T obj)
        {
            return Factotum.ToPropertyString(obj, null);
        }

        /// <summary>
        ///     Determines if the contents of the specified object are equal to the contents of another specified
        ///     object, that is, if these objects are of the same type and the values of all their corresponding
        ///     instance fields are equal.
        /// </summary>
        /// <remarks>
        ///     This method uses reflection to obtain the list of fields for comparison.
        ///     This method recursively processes the composite objects, if any.
        /// </remarks>
        /// <typeparam name="T">
        ///     The type of the objects to compare.
        /// </typeparam>
        /// <param name="obj">
        ///     The first object to compare.
        /// </param>
        /// <param name="other">
        ///     The second object to compare.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the contents of the two specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEqualByContentsTo<T>([CanBeNull] this T obj, [CanBeNull] T other)
        {
            return Factotum.AreEqualByContents(obj, other);
        }

        /// <summary>
        ///     Computes the specified predicate against the specified reference type value and
        ///     returns this value if the predicate evaluates to <c>true</c>; otherwise, returns <c>null</c>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value.
        /// </typeparam>
        /// <param name="value">
        ///     The value to compute the predicate against.
        /// </param>
        /// <param name="predicate">
        ///     The predicate to compute.
        /// </param>
        /// <returns>
        ///     <paramref name="value"/> if the predicate evaluates to <c>true</c>; otherwise, <c>null</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="predicate"/> is <c>null</c>.
        /// </exception>
        public static T Affirm<T>([CanBeNull] this T value, [NotNull] Func<T, bool> predicate)
            where T : class
        {
            #region Argument Check

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            #endregion

            return value == null || !predicate(value) ? null : value;
        }

        /// <summary>
        ///     Computes the specified predicate against the specified reference type value considering that
        ///     this value can be <c>null</c>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value.
        /// </typeparam>
        /// <param name="value">
        ///     The value to compute the predicate against.
        /// </param>
        /// <param name="predicate">
        ///     The predicate to compute.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="value"/> is NOT <c>null</c> and the predicate evaluates to
        ///     <c>true</c>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="predicate"/> is <c>null</c>.
        /// </exception>
        public static bool ComputePredicate<T>([CanBeNull] this T value, [NotNull] Func<T, bool> predicate)
            where T : class
        {
            #region Argument Check

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            #endregion

            return value != null && predicate(value);
        }

        /// <summary>
        ///     Metamorphoses the specified reference type input value into an output value using the specified
        ///     transformation method. If the input value is <c>null</c>, the specified default output value is
        ///     returned.
        /// </summary>
        /// <typeparam name="TInput">
        ///     The type of the input.
        /// </typeparam>
        /// <typeparam name="TOutput">
        ///     The type of the output.
        /// </typeparam>
        /// <param name="input">
        ///     The input value.
        /// </param>
        /// <param name="transform">
        ///     A reference to the transformation method.
        /// </param>
        /// <param name="defaultOutput">
        ///     The default output value.
        /// </param>
        /// <returns>
        ///     An output value obtained by using the <paramref name="transform"/> method if the
        ///     <paramref name="input"/> value is NOT <c>null</c>; otherwise, the specified default output value.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="transform"/> is <c>null</c>.
        /// </exception>
        public static TOutput Morph<TInput, TOutput>(
            [CanBeNull] this TInput input,
            [NotNull] Func<TInput, TOutput> transform,
            [CanBeNull] TOutput defaultOutput)
            where TInput : class
        {
            #region Argument Check

            if (transform == null)
            {
                throw new ArgumentNullException("transform");
            }

            #endregion

            return input == null ? defaultOutput : transform(input);
        }

        /// <summary>
        ///     Metamorphoses the specified reference type input value into an output value using the specified
        ///     transformation method. If the input value is <c>null</c>, the default value for the output type
        ///     is returned.
        /// </summary>
        /// <typeparam name="TInput">
        ///     The type of the input.
        /// </typeparam>
        /// <typeparam name="TOutput">
        ///     The type of the output.
        /// </typeparam>
        /// <param name="input">
        ///     The input value.
        /// </param>
        /// <param name="transform">
        ///     A reference to the transformation method.
        /// </param>
        /// <returns>
        ///     An output value obtained by using the <paramref name="transform"/> method if the
        ///     <paramref name="input"/> value is NOT <c>null</c>; otherwise, the default value for the output type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="transform"/> is <c>null</c>.
        /// </exception>
        public static TOutput Morph<TInput, TOutput>(
            [CanBeNull] this TInput input,
            [NotNull] Func<TInput, TOutput> transform)
            where TInput : class
        {
            return Morph(input, transform, default(TOutput));
        }

        #endregion
    }
}