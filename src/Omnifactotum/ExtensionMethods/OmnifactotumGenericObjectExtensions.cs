using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Omnifactotum;
using Omnifactotum.Annotations;

//// ReSharper disable once CheckNamespace - Namespace is intentionally named so in order to simplify usage of extension methods

namespace System
{
    /// <summary>
    ///     Contains extension methods for any type.
    /// </summary>
    public static class OmnifactotumGenericObjectExtensions
    {
        private const string NullString = "<null>";

        private const string ItemSeparator = ", ";
        private const string PropertyNameValueSeparator = " = ";
        private const string CollectionElementsPropertyName = "Elements";
        private const string CollectionElementsOpeningBrace = "[";
        private const string CollectionElementsClosingBrace = "]";
        private const string ComplexObjectOpeningBrace = "{";
        private const string ComplexObjectClosingBrace = "}";

        /// <summary>
        ///     The pointer string format.
        /// </summary>
        internal static readonly string PointerStringFormat = $@"0x{{0:X{Marshal.SizeOf(typeof(IntPtr)) * 2}}}";

        private static readonly MethodInfo ToPropertyStringInternalMethodDefinition =
            new ToPropertyStringInternalMethod(ToPropertyStringInternal).Method.GetGenericMethodDefinition();

        private static readonly WeakReferenceBasedCache<Type, FieldInfo[]> ContentFieldsCache =
            new WeakReferenceBasedCache<Type, FieldInfo[]>(
                t => t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

        [ThreadStatic]
        private static HashSet<object> _toPropertyStringObjectsBeingProcessed;

        [ThreadStatic]
        private static StringBuilder _toPropertyStringResultBuilder;

        [ThreadStatic]
        private static HashSet<PairReferenceHolder> _assertEqualityByContentsObjectsBeingProcessed;

        private delegate void ToPropertyStringInternalMethod(
            object obj,
            bool isRoot,
            ToPropertyStringOptions options,
            Func<Type, PropertyInfo[]> getProperties,
            StringBuilder resultBuilder,
            int recursionLevel);

        /// <summary>
        ///     Returns the specified value if is not <c>null</c>;
        ///     otherwise, throws <see cref="ArgumentNullException"/>.
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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <c>null</c>.
        /// </exception>
        [NotNull]
        public static T EnsureNotNull<T>([CanBeNull] this T value)
            where T : class
            => value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        ///     Returns the value which underlies the specified nullable value, if it is not <c>null</c>
        ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <c>true</c>);
        ///     otherwise, throws <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type which underlies the nullable type of the value to check.
        /// </typeparam>
        /// <param name="value">
        ///     The value to check.
        /// </param>
        /// <returns>
        ///     The value which underlies the specified nullable value, if it is not <c>null</c>
        ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <c>true</c>).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <c>null</c>, that is, its <see cref="Nullable{T}.HasValue"/> property is
        ///     <c>false</c>.
        /// </exception>
        [DebuggerStepThrough]
        public static T EnsureNotNull<T>([CanBeNull] this T? value)
            where T : struct
            => value ?? throw new ArgumentNullException(nameof(value));

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
            => ReferenceEquals(value, null) ? nullValueString : value.ToString();

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
        public static string ToStringSafely<T>([CanBeNull] this T value) => ToStringSafely(value, string.Empty);

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

            return value is IFormattable formattable
                ? formattable.ToString(null, CultureInfo.InvariantCulture)
                : value.ToString();
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
            => ToStringSafelyInvariant(value, string.Empty);

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
            => ReferenceEquals(value, null) ? nullValueHashCode : value.GetHashCode();

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
        public static int GetHashCodeSafely<T>([CanBeNull] this T value) => GetHashCodeSafely(value, 0);

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
            => ReferenceEquals(value, null) ? typeof(T) : value.GetType();

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
        public static T[] AsArray<T>([CanBeNull] this T value) => new[] { value };

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
        public static List<T> AsList<T>([CanBeNull] this T value) => new List<T> { value };

        /// <summary>
        ///     Creates a strongly-typed collection containing the specified value as its sole element.
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
        public static T AvoidNull<T>([CanBeNull] this T source, [NotNull] Func<T> getDefault)
            where T : class
        {
            if (getDefault == null)
            {
                throw new ArgumentNullException(nameof(getDefault));
            }

            var result = source ?? getDefault();
            if (result == null)
            {
                throw new InvalidOperationException(
                    "The method that had to return non-null value returned null.");
            }

            return result;
        }

        /// <summary>
        ///     <para>
        ///         Converts the specified nullable value to its UI representation.
        ///     </para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>The input value</term>
        ///             <description>The result of the method</description>
        ///         </listheader>
        ///         <item>
        ///             <term><c>null</c></term>
        ///             <description>The literal: <b>null</b></description>
        ///         </item>
        ///         <item>
        ///             <term>not <c>null</c></term>
        ///             <description>
        ///                 A result of the <see cref="object.ToString()"/> method called for the
        ///                 input value.
        ///             </description>
        ///         </item>
        ///     </list>
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
        /// <example>
        ///     <code>
        /// <![CDATA[
        ///         int? value;
        ///
        ///         value = null;
        ///         Console.WriteLine("Value is {0}.", value.ToUIString()); // Output: Value is null.
        ///
        ///         value = 42;
        ///         Console.WriteLine("Value is {0}.", value.ToUIString()); // Output: Value is 42.
        /// ]]>
        ///     </code>
        /// </example>
        public static string ToUIString<T>([CanBeNull] this T? value)
            where T : struct
            => value?.ToString() ?? OmnifactotumConstants.NullValueRepresentation;

        /// <summary>
        ///     <para>
        ///         Converts the specified nullable value to its UI representation using the
        ///         specified format and format provider.
        ///     </para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>The input value</term>
        ///             <description>The result of the method</description>
        ///         </listheader>
        ///         <item>
        ///             <term><c>null</c></term>
        ///             <description>The literal: <b>null</b></description>
        ///         </item>
        ///         <item>
        ///             <term>not <c>null</c></term>
        ///             <description>
        ///                 A result of the
        ///                 <see cref="IFormattable.ToString(string,System.IFormatProvider)"/> method
        ///                 called for the input value.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <typeparam name="T">
        ///     The underlying value type of the nullable value.
        /// </typeparam>
        /// <param name="value">
        ///     The nullable value to convert.
        /// </param>
        /// <param name="format">
        ///     The format to use, or <c>null</c> to use the default format defined for the type <typeparamref name="T"/>.
        /// </param>
        /// <param name="formatProvider">
        ///     The provider to use to format the value, or <c>null</c> to obtain the format
        ///     information from the current locale setting of the operating system.
        /// </param>
        /// <returns>
        ///     The UI representation of the specified nullable value.
        /// </returns>
        public static string ToUIString<T>([CanBeNull] this T? value, string format, IFormatProvider formatProvider)
            where T : struct, IFormattable
            => value?.ToString(format, formatProvider) ?? OmnifactotumConstants.NullValueRepresentation;

        /// <summary>
        ///     <para>
        ///         Converts the specified nullable value to its UI representation using the
        ///         specified format provider.
        ///     </para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>The input value</term>
        ///             <description>The result of the method</description>
        ///         </listheader>
        ///         <item>
        ///             <term><c>null</c></term>
        ///             <description>The literal: <b>null</b></description>
        ///         </item>
        ///         <item>
        ///             <term>not <c>null</c></term>
        ///             <description>
        ///                 A result of the
        ///                 <see cref="IFormattable.ToString(string,System.IFormatProvider)"/> method
        ///                 called for the input value.
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <typeparam name="T">
        ///     The underlying value type of the nullable value.
        /// </typeparam>
        /// <param name="value">
        ///     The nullable value to convert.
        /// </param>
        /// <param name="formatProvider">
        ///     The provider to use to format the value, or <c>null</c> to obtain the format
        ///     information from the current locale setting of the operating system.
        /// </param>
        /// <returns>
        ///     The UI representation of the specified nullable value.
        /// </returns>
        public static string ToUIString<T>([CanBeNull] this T? value, IFormatProvider formatProvider)
            where T : struct, IFormattable
            => value.ToUIString(null, formatProvider);

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
            var actualOptions = options ?? new ToPropertyStringOptions();

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (actualOptions.IncludeNonPublicMembers)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }

            PropertyInfo[] GetProperties(Type type)
            {
                var getPropertiesQuery = type
                    .GetProperties(bindingFlags)
                    .Where(item => item.CanRead && !item.GetIndexParameters().Any());
                if (actualOptions.SortMembersAlphabetically)
                {
                    getPropertiesQuery = getPropertiesQuery.OrderBy(item => item.Name);
                }

                return getPropertiesQuery.ToArray();
            }

            if (_toPropertyStringResultBuilder == null)
            {
                _toPropertyStringResultBuilder = new StringBuilder(128);
            }
            else
            {
                _toPropertyStringResultBuilder.Clear();
            }

            ToPropertyStringInternal(obj, true, actualOptions, GetProperties, _toPropertyStringResultBuilder, 0);

            var result = _toPropertyStringResultBuilder.ToString();
            _toPropertyStringResultBuilder.Clear();
            return result;
        }

        /// <summary>
        ///     Gets a string representing the properties of the specified object using the default options.
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
        public static string ToPropertyString<T>([CanBeNull] this T obj) => ToPropertyString(obj, null);

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
            => AreEqualByContentsInternal(obj, other);

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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="predicate"/> is <c>null</c>.
        /// </exception>
        public static T Affirm<T>([CanBeNull] this T value, [NotNull] Func<T, bool> predicate)
            where T : class
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="predicate"/> is <c>null</c>.
        /// </exception>
        public static bool ComputePredicate<T>([CanBeNull] this T value, [NotNull] Func<T, bool> predicate)
            where T : class
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="transform"/> is <c>null</c>.
        /// </exception>
        public static TOutput Morph<TInput, TOutput>(
            [CanBeNull] this TInput input,
            [NotNull] Func<TInput, TOutput> transform,
            [CanBeNull] TOutput defaultOutput)
            where TInput : class
        {
            if (transform == null)
            {
                throw new ArgumentNullException(nameof(transform));
            }

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
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="transform"/> is <c>null</c>.
        /// </exception>
        public static TOutput Morph<TInput, TOutput>(
            [CanBeNull] this TInput input,
            [NotNull] Func<TInput, TOutput> transform)
            where TInput : class
            => Morph(input, transform, default(TOutput));

        private static bool IsSimpleTypeInternal([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsPrimitive
                || type.IsEnum
                || type.IsPointer
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(Pointer)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset);
        }

        [DebuggerNonUserCode]
        private static void ToPropertyStringInternal<T>(
            T obj,
            bool isRoot,
            ToPropertyStringOptions options,
            Func<Type, PropertyInfo[]> getProperties,
            StringBuilder resultBuilder,
            int recursionLevel)
        {
            var isToPropertyStringObjectsBeingProcessedCreated = false;
            var isObjectAddedToBeingProcessed = false;
            var isBraceOpen = false;
            try
            {
                if (_toPropertyStringObjectsBeingProcessed == null)
                {
                    isToPropertyStringObjectsBeingProcessedCreated = true;
                    _toPropertyStringObjectsBeingProcessed = new HashSet<object>(
                        ByReferenceEqualityComparer<object>.Instance);
                }

                void OpenBrace()
                {
                    if (!isBraceOpen && !isRoot)
                    {
                        resultBuilder.Append(ComplexObjectOpeningBrace);
                        isBraceOpen = true;
                    }
                }

                var nextRecursionLevel = recursionLevel + 1;

                var type = obj.GetTypeSafely();

                string RenderActualType() => $@"{type.GetQualifiedName()} :: ";

                var shouldRenderActualType = false;
                if (isRoot)
                {
                    if (options.RenderRootActualType)
                    {
                        shouldRenderActualType = true;
                    }
                }
                else
                {
                    if (options.RenderActualType)
                    {
                        shouldRenderActualType = true;
                    }
                }

                if (shouldRenderActualType)
                {
                    OpenBrace();
                    resultBuilder.Append(RenderActualType());
                }

                if (ReferenceEquals(obj, null))
                {
                    resultBuilder.Append(NullString);
                    return;
                }

                var isSimpleType = type.IsSimpleTypeInternal()
                    || typeof(Type).IsAssignableFrom(type)
                    || typeof(Assembly).IsAssignableFrom(type)
                    || typeof(Delegate).IsAssignableFrom(type);

                if (!isSimpleType && _toPropertyStringObjectsBeingProcessed.Contains(obj))
                {
                    resultBuilder.AppendFormat(
                        "{0} {1}<- {2}",
                        ComplexObjectOpeningBrace,
                        shouldRenderActualType ? string.Empty : RenderActualType(),
                        ComplexObjectClosingBrace);
                    return;
                }

                if (!type.IsValueType || !typeof(T).IsValueType)
                {
                    _toPropertyStringObjectsBeingProcessed.Add(obj);
                    isObjectAddedToBeingProcessed = true;
                }

                if (isSimpleType)
                {
                    ToPropertyStringInternalForSimpleType(obj, type, resultBuilder);
                    return;
                }

                OpenBrace();

                if (!isRoot && !options.RenderComplexProperties)
                {
                    resultBuilder.Append(obj.ToStringSafelyInvariant());
                    return;
                }

                if (recursionLevel > options.MaxRecursionLevel)
                {
                    resultBuilder.Append("<Max recursion level reached>");
                    return;
                }

                var propertySeparatorNeeded = false;

                if (obj is IEnumerable enumerable)
                {
                    propertySeparatorNeeded = true;

                    ToPropertyStringInternalForEnumerableCollection(
                        enumerable,
                        type,
                        options,
                        getProperties,
                        resultBuilder,
                        nextRecursionLevel);
                }

                var propertyInfos = getProperties(type);

                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertySeparatorNeeded)
                    {
                        resultBuilder.Append(ItemSeparator);
                    }

                    propertySeparatorNeeded = true;
                    resultBuilder.Append(propertyInfo.Name);
                    if (options.RenderMemberType)
                    {
                        resultBuilder.AppendFormat(":{0}", propertyInfo.PropertyType.GetQualifiedName());
                    }

                    resultBuilder.Append(PropertyNameValueSeparator);

                    object propertyValue;
                    try
                    {
                        propertyValue = propertyInfo.GetValue(obj, null);
                    }
                    catch (Exception ex)
                    {
                        resultBuilder.AppendFormat(
                            "{{ Error getting property value: [{0}] {1}) }}",
                            ex.GetType().Name,
                            (ex.GetBaseException() ?? ex).Message);

                        continue;
                    }

                    var method = ToPropertyStringInternalMethodDefinition.MakeGenericMethod(
                        propertyInfo.PropertyType.IsPointer ? typeof(object) : propertyInfo.PropertyType);

                    var parameters =
                        new[]
                        {
                            propertyValue,
                            false,
                            options,
                            getProperties,
                            resultBuilder,
                            nextRecursionLevel
                        };

                    method.Invoke(null, parameters);
                }
            }
            finally
            {
                if (isBraceOpen)
                {
                    resultBuilder.Append(ComplexObjectClosingBrace);
                }

                if (isObjectAddedToBeingProcessed)
                {
                    _toPropertyStringObjectsBeingProcessed.Remove(obj);
                }

                if (isToPropertyStringObjectsBeingProcessedCreated)
                {
                    _toPropertyStringObjectsBeingProcessed = null;
                }
            }
        }

        private static void ToPropertyStringInternalForSimpleType<T>(T obj, Type type, StringBuilder resultBuilder)
        {
            if (type == typeof(Pointer))
            {
                unsafe
                {
                    resultBuilder.AppendFormat(PointerStringFormat, (long)Pointer.Unbox(obj));
                }
            }
            else if (type == typeof(IntPtr))
            {
                resultBuilder.AppendFormat(PointerStringFormat, ((IntPtr)(object)obj).ToInt64());
            }
            else if (type == typeof(UIntPtr))
            {
                resultBuilder.AppendFormat(PointerStringFormat, ((UIntPtr)(object)obj).ToUInt64());
            }
            else if (type == typeof(string))
            {
                resultBuilder.Append(((string)(object)obj).ToUIString());
            }
            else if (type.IsEnum && type.IsDefined(typeof(FlagsAttribute), false))
            {
                resultBuilder.Append(
                    ComplexObjectOpeningBrace + obj.ToStringSafelyInvariant() + ComplexObjectClosingBrace);
            }
            else if (type == typeof(DateTime))
            {
                resultBuilder.Append(((DateTime)(object)obj).ToPreciseFixedString());
            }
            else if (type == typeof(DateTimeOffset))
            {
                resultBuilder.Append(((DateTimeOffset)(object)obj).ToPreciseFixedString());
            }
            else if (typeof(Type).IsAssignableFrom(type))
            {
                resultBuilder.AppendFormat(((Type)(object)obj).AssemblyQualifiedName.ToUIString());
            }
            else if (typeof(Assembly).IsAssignableFrom(type))
            {
                resultBuilder.AppendFormat(((Assembly)(object)obj).CodeBase.ToUIString());
            }
            else
            {
                resultBuilder.Append(obj.ToStringSafelyInvariant());
            }
        }

        private static void ToPropertyStringInternalForEnumerableCollection(
            [NotNull] IEnumerable enumerable,
            [NotNull] Type type,
            [NotNull] ToPropertyStringOptions options,
            [NotNull] Func<Type, PropertyInfo[]> getProperties,
            [NotNull] StringBuilder resultBuilder,
            int nextRecursionLevel)
        {
            var elementType = type.GetCollectionElementType().EnsureNotNull();

            resultBuilder.Append(CollectionElementsOpeningBrace);
            resultBuilder.Append(CollectionElementsPropertyName);
            if (options.RenderMemberType)
            {
                resultBuilder.AppendFormat(":{0}", elementType.GetQualifiedName());
            }

            resultBuilder.Append(CollectionElementsClosingBrace);

            resultBuilder.Append(PropertyNameValueSeparator);
            resultBuilder.Append(ComplexObjectOpeningBrace);

            var count = 0;
            using (var enumeratorWrapper = SmartDisposable.Create(enumerable.GetEnumerator()))
            {
                while (enumeratorWrapper.Instance.MoveNext())
                {
                    if (count >= options.MaxCollectionItemCount)
                    {
                        if (count > 0)
                        {
                            resultBuilder.Append(ItemSeparator);
                        }

                        resultBuilder.Append("...");

                        break;
                    }

                    if (count > 0)
                    {
                        resultBuilder.Append(ItemSeparator);
                    }

                    count++;

                    object currentValue;
                    try
                    {
                        currentValue = enumeratorWrapper.Instance.Current;
                    }
                    catch (Exception ex)
                    {
                        resultBuilder.AppendFormat(
                            "{{ Error getting the collection element at index {0} ({1}: {2}) }}",
                            count - 1,
                            ex.GetType().Name,
                            ex.Message);
                        continue;
                    }

                    var method = ToPropertyStringInternalMethodDefinition.MakeGenericMethod(
                        elementType.IsPointer ? typeof(object) : elementType);

                    var parameters = new[]
                    {
                        currentValue,
                        false,
                        options,
                        getProperties,
                        resultBuilder,
                        nextRecursionLevel
                    };

                    method.Invoke(null, parameters);
                }
            }

            resultBuilder.Append(ComplexObjectClosingBrace);
        }

        private static bool AreEqualByContentsInternal(object valueA, object valueB)
        {
            var isAssertEqualityByContentObjectsBeingProcessedCreated = false;
            var isObjectPairAddedToBeingProcessed = false;
            try
            {
                if (ReferenceEquals(valueA, valueB))
                {
                    return true;
                }

                if (ReferenceEquals(valueA, null) || ReferenceEquals(valueB, null))
                {
                    return false;
                }

                var actualType = valueA.GetType();
                if (actualType != valueB.GetType())
                {
                    return false;
                }

                if (actualType.IsSimpleTypeInternal())
                {
                    return Equals(valueA, valueB);
                }

                if (_assertEqualityByContentsObjectsBeingProcessed == null)
                {
                    isAssertEqualityByContentObjectsBeingProcessedCreated = true;
                    _assertEqualityByContentsObjectsBeingProcessed = new HashSet<PairReferenceHolder>();
                }

                if (!actualType.IsValueType)
                {
                    isObjectPairAddedToBeingProcessed = true;
                    _assertEqualityByContentsObjectsBeingProcessed.Add(new PairReferenceHolder(valueA, valueB));
                }

                var fields = ContentFieldsCache[actualType];
                if (fields.Length == 0)
                {
                    return actualType.IsValueType || ReferenceEquals(valueA, valueB);
                }

                //// ReSharper disable once LoopCanBeConvertedToQuery - More readable in 'foreach' style
                foreach (var field in fields)
                {
                    var fieldValueA = field.GetValue(valueA);
                    var fieldValueB = field.GetValue(valueB);

                    var fieldEqual = AreEqualByContentsInternal(fieldValueA, fieldValueB);
                    if (!fieldEqual)
                    {
                        return false;
                    }
                }
            }
            finally
            {
                if (isObjectPairAddedToBeingProcessed)
                {
                    _assertEqualityByContentsObjectsBeingProcessed.Remove(new PairReferenceHolder(valueA, valueB));
                }

                if (isAssertEqualityByContentObjectsBeingProcessedCreated)
                {
                    _assertEqualityByContentsObjectsBeingProcessed = null;
                }
            }

            return true;
        }

        private struct PairReferenceHolder : IEquatable<PairReferenceHolder>
        {
            private static readonly ByReferenceEqualityComparer<object> EqualityComparer =
                ByReferenceEqualityComparer<object>.Instance;

            private readonly object _valueA;
            private readonly object _valueB;

            internal PairReferenceHolder(object valueA, object valueB)
            {
                _valueA = valueA;
                _valueB = valueB;
            }

            public override bool Equals(object obj) => obj is PairReferenceHolder holder && Equals(holder);

            public override int GetHashCode()
                => EqualityComparer.GetHashCode(_valueA).CombineHashCodeValues(EqualityComparer.GetHashCode(_valueB));

            public bool Equals(PairReferenceHolder other)
                => ReferenceEquals(_valueA, other._valueA) && ReferenceEquals(_valueB, other._valueB);
        }
    }
}