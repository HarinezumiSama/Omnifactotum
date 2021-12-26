#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Omnifactotum;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;
using NotNullIfNotNull = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System
{
    /// <summary>
    ///     Contains extension methods for any type.
    /// </summary>
    public static class OmnifactotumGenericObjectExtensions
    {
        private const string NullString = @"<null>";

        private const string ItemSeparator = @", ";
        private const string PropertyNameValueSeparator = @" = ";
        private const string CollectionElementsPropertyName = @"Elements";
        private const string CollectionElementsOpeningBrace = @"[";
        private const string CollectionElementsClosingBrace = @"]";
        private const string ComplexObjectOpeningBrace = @"{";
        private const string ComplexObjectClosingBrace = @"}";

        /// <summary>
        ///     The pointer string format.
        /// </summary>
        internal static readonly string PointerStringFormat = AsInvariant($@"0x{{0:X{Marshal.SizeOf(typeof(IntPtr)) * 2}}}");

        private static readonly MethodInfo ToPropertyStringInternalMethodDefinition =
            new ToPropertyStringInternalMethod(ToPropertyStringInternal).Method.GetGenericMethodDefinition();

        private static readonly WeakReferenceBasedCache<Type, FieldInfo[]> ContentFieldsCache = new(GetContentFieldsCacheFields);

        [ThreadStatic]
        private static HashSet<object>? _toPropertyStringObjectsBeingProcessed;

        [ThreadStatic]
        private static StringBuilder? _toPropertyStringResultBuilder;

        [ThreadStatic]
        private static HashSet<PairReferenceHolder>? _assertEqualityByContentsObjectsBeingProcessed;

        private delegate void ToPropertyStringInternalMethod(
            object? obj,
            bool isRoot,
            ToPropertyStringOptions options,
            Func<Type, PropertyInfo[]> getProperties,
            StringBuilder resultBuilder,
            int recursionLevel);

        /// <summary>
        ///     Returns the specified value if it is not <see langword="null"/>;
        ///     otherwise, throws an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The reference type of the value to check.
        /// </typeparam>
        /// <param name="value">
        ///     The value to check.
        /// </param>
        /// <returns>
        ///     The specified value if it is not <see langword="null"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [return: NotNullIfNotNull(@"value")]
        [NotNull]
        [DebuggerStepThrough]
        [ContractAnnotation("null => stop; notnull => notnull", true)]
        public static T EnsureNotNull<T>([CanBeNull] this T? value)
            where T : class
            => value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        ///     Returns the value which underlies the specified nullable value, if it is not <see langword="null"/>
        ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <see langword="true"/>);
        ///     otherwise, throws <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type which underlies the nullable type of the value to check.
        /// </typeparam>
        /// <param name="value">
        ///     The value to check.
        /// </param>
        /// <returns>
        ///     The value which underlies the specified nullable value, if it is not <see langword="null"/>
        ///     (that is, if its <see cref="Nullable{T}.HasValue"/> property is <see langword="true"/>).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="value"/> is <see langword="null"/>, that is, its <see cref="Nullable{T}.HasValue"/> property is
        ///     <see langword="false"/>.
        /// </exception>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [DebuggerStepThrough]
        public static T EnsureNotNull<T>([CanBeNull] this T? value)
            where T : struct
            => value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        ///     <para>
        ///         Safely gets a <see cref="System.String"/> that represents the specified value.
        ///     </para>
        ///     <para>
        ///         Returns the result of the <see cref="object.ToString()"/> method call for the specified value, considering that this
        ///         value may be <see langword="null"/>. If the specified value is <see langword="null"/> or its
        ///         <see cref="object.ToString()"/> method call returns <see langword="null"/>, then
        ///         the <paramref name="fallbackResult"/> value is used. And if the <paramref name="fallbackResult"/> value is
        ///         <see langword="null"/>, then <see cref="string.Empty"/> is returned.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <param name="fallbackResult">
        ///     A <see cref="System.String"/> to return if <paramref name="value"/> is <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        [DebuggerStepThrough]
        public static string ToStringSafely<T>(this T value, [CanBeNull] string? fallbackResult)
            => value?.ToString() ?? fallbackResult ?? string.Empty;

        /// <summary>
        ///     <para>
        ///         Safely gets a <see cref="System.String"/> that represents the specified value.
        ///     </para>
        ///     <para>
        ///         Returns the result of the <see cref="object.ToString()"/> method call for the specified value, considering that this
        ///         value may be <see langword="null"/>. If the specified value is <see langword="null"/> or its
        ///         <see cref="object.ToString()"/> method call returns <see langword="null"/>, then <see cref="string.Empty"/> is
        ///         returned.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        [DebuggerStepThrough]
        public static string ToStringSafely<T>(this T value) => ToStringSafely(value, null);

        /// <summary>
        ///     <para>
        ///         Safely gets a <see cref="System.String"/> that represents the specified value, using
        ///         <see cref="CultureInfo.InvariantCulture"/>.
        ///     </para>
        ///     <para>
        ///         Returns the result of the <see cref="object.ToString()"/> method call for the specified value, considering that this
        ///         value may be <see langword="null"/>. If the specified value is <see langword="null"/> or its
        ///         <see cref="object.ToString()"/> method call returns <see langword="null"/>, then
        ///         the <paramref name="fallbackResult"/> value is used. And if the <paramref name="fallbackResult"/> value is
        ///         <see langword="null"/>, then <see cref="string.Empty"/> is returned.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <param name="fallbackResult">
        ///     A <see cref="System.String"/> to return if <paramref name="value"/> is <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value, or the value of
        ///     the <paramref name="fallbackResult"/> parameter if <paramref name="value"/> is <see langword="null"/>.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        [DebuggerStepThrough]
        public static string ToStringSafelyInvariant<T>(this T value, [CanBeNull] string? fallbackResult)
            => (value is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : value?.ToString())
                ?? fallbackResult
                ?? string.Empty;

        /// <summary>
        ///     <para>
        ///         Safely gets a <see cref="System.String"/> that represents the specified value, using
        ///         <see cref="CultureInfo.InvariantCulture"/>.
        ///     </para>
        ///     <para>
        ///         Returns the result of the <see cref="object.ToString()"/> method call for the specified value, considering that this
        ///         value may be <see langword="null"/>. If the specified value is <see langword="null"/> or its
        ///         <see cref="object.ToString()"/> method call returns <see langword="null"/>, then <see cref="string.Empty"/> is
        ///         returned.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a string representation of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a string representation of.
        /// </param>
        /// <returns>
        ///     A <see cref="System.String"/> that represents the specified value it is not <see langword="null"/>;
        ///     otherwise, the empty string.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static string ToStringSafelyInvariant<T>(this T value) => ToStringSafelyInvariant(value, string.Empty);

        /// <summary>
        ///     Gets a hash code of the specified value safely, that is, <see langword="null"/> does not cause an exception.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a hash code of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a hash code of. Can be <see langword="null"/>.
        /// </param>
        /// <param name="nullValueHashCode">
        ///     The value to return if the <paramref name="value"/> parameter is <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     A hash code of the specified value obtained by calling <see cref="object.GetHashCode"/> for this value
        ///     if it is not <see langword="null"/>; otherwise, the value specified by the <paramref name="nullValueHashCode"/>
        ///     parameter.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static int GetHashCodeSafely<T>(this T value, int nullValueHashCode)
            => value is null ? nullValueHashCode : value.GetHashCode();

        /// <summary>
        ///     Gets a hash code of the specified value safely, that is, <see langword="null"/> does not cause an exception.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to get a hash code of.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get a hash code of. Can be <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     A hash code of the specified value obtained by calling <see cref="object.GetHashCode"/> for this value
        ///     if it is not <see langword="null"/>; otherwise, <c>0</c>.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static int GetHashCodeSafely<T>(this T value) => GetHashCodeSafely(value, 0);

        /// <summary>
        ///     Gets the type of the specified value, considering that this value may be <see langword="null"/>.
        ///     In the latter case, the formally specified type <typeparamref name="T"/> is returned.
        /// </summary>
        /// <typeparam name="T">
        ///     The formal type of the value.
        /// </typeparam>
        /// <param name="value">
        ///     The value to get the type of.
        /// </param>
        /// <returns>
        ///     The actual type of the value if it is not <see langword="null"/>; otherwise, <typeparamref name="T"/>.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static Type GetTypeSafely<T>(this T value) => value is null ? typeof(T) : value.GetType();

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
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static T[] AsArray<T>(this T value) => new[] { value };

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
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static List<T> AsList<T>(this T value) => new() { value };

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
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static IEnumerable<T> AsCollection<T>(this T value)
        {
            yield return value;
        }

        /// <summary>
        ///     Converts the specified value of the value type to a corresponding <see cref="Nullable{T}"/> value.
        /// </summary>
        /// <param name="value">
        ///     The value of the value type to convert to a corresponding <see cref="Nullable{T}"/> value.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the value to convert.
        /// </typeparam>
        /// <returns>
        ///     A <see cref="Nullable{T}"/> value that contains <paramref name="value"/>.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static T? AsNullable<T>(this T value)
            where T : struct
            => value;

        /// <summary>
        ///     Avoids the specified reference type value to be a <see langword="null"/> reference: returns the specified value
        ///     if it is not <see langword="null"/> or a default value which is returned by the specified ad-hoc method.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the value to handle.
        /// </typeparam>
        /// <param name="source">
        ///     The value to secure from a <see langword="null"/> reference.
        /// </param>
        /// <param name="getDefault">
        ///     The method that will return the default value to use instead of <see langword="null"/>.
        /// </param>
        /// <returns>
        ///     The string value if it is not <see langword="null"/>; otherwise, the value returned from call to
        ///     <paramref name="getDefault"/> method.
        /// </returns>
        [NotNull]
        public static T AvoidNull<T>([CanBeNull] this T? source, [NotNull] [InstantHandle] Func<T> getDefault)
            where T : class
        {
            if (getDefault is null)
            {
                throw new ArgumentNullException(nameof(getDefault));
            }

            var result = source ?? getDefault();
            if (result is null)
            {
                throw new InvalidOperationException("The method that had to return non-null value returned null.");
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
        ///             <term><see langword="null"/></term>
        ///             <description>The literal: <b>null</b></description>
        ///         </item>
        ///         <item>
        ///             <term>not <see langword="null"/></term>
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
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static string ToUIString<T>([CanBeNull] this T? value)
            where T : struct
            => (value is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : value?.ToString())
                ?? OmnifactotumRepresentationConstants.NullValueRepresentation;

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
        ///             <term><see langword="null"/></term>
        ///             <description>The literal: <b>null</b></description>
        ///         </item>
        ///         <item>
        ///             <term>not <see langword="null"/></term>
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
        ///     The format to use, or <see langword="null"/> to use the default format defined for the type <typeparamref name="T"/>.
        /// </param>
        /// <param name="formatProvider">
        ///     The provider to use to format the value, or <see langword="null"/> to obtain the format
        ///     information from the current locale setting of the operating system.
        /// </param>
        /// <returns>
        ///     The UI representation of the specified nullable value.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static string ToUIString<T>(
            [CanBeNull] this T? value,
            [CanBeNull] string? format,
            [CanBeNull] IFormatProvider? formatProvider)
            where T : struct, IFormattable
            => value?.ToString(format, formatProvider) ?? OmnifactotumRepresentationConstants.NullValueRepresentation;

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
        ///             <term><see langword="null"/></term>
        ///             <description>The literal: <b>null</b></description>
        ///         </item>
        ///         <item>
        ///             <term>not <see langword="null"/></term>
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
        ///     The provider to use to format the value, or <see langword="null"/> to obtain the format
        ///     information from the current locale setting of the operating system.
        /// </param>
        /// <returns>
        ///     The UI representation of the specified nullable value.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static string ToUIString<T>([CanBeNull] this T? value, [CanBeNull] IFormatProvider? formatProvider)
            where T : struct, IFormattable
            => value.ToUIString(null, formatProvider);

        /// <summary>
        ///     Gets the description of the specified object reference. The resulting description contains
        ///     the full type name of the object and the hash code based on the object reference.
        /// </summary>
        /// <param name="obj">
        ///     The object to get the reference description of.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the object.
        /// </typeparam>
        /// <returns>
        ///     The description of the specified object reference.
        /// </returns>
        /// <seealso cref="OmnifactotumTypeExtensions.GetFullName"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static string GetObjectReferenceDescription<T>(this T? obj)
            where T : class
            => GetObjectReferenceDescriptionInternal(obj, OmnifactotumTypeExtensions.GetFullNameMethod);

        /// <summary>
        ///     Gets the short description of the specified object reference. The resulting short description contains
        ///     the qualified type name of the object and the hash code based on the object reference.
        /// </summary>
        /// <param name="obj">
        ///     The object to get the reference description of.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the object.
        /// </typeparam>
        /// <returns>
        ///     The short description of the specified object reference.
        /// </returns>
        /// <seealso cref="OmnifactotumTypeExtensions.GetQualifiedName"/>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static string GetShortObjectReferenceDescription<T>(this T? obj)
            where T : class
            => GetObjectReferenceDescriptionInternal(obj, OmnifactotumTypeExtensions.GetQualifiedNameMethod);

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
        [NotNull]
        public static string ToPropertyString<T>([CanBeNull] this T? obj, [CanBeNull] ToPropertyStringOptions? options)
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

            if (_toPropertyStringResultBuilder is null)
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
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        [NotNull]
        public static string ToPropertyString<T>([CanBeNull] this T? obj) => ToPropertyString(obj, null);

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
        ///     <see langword="true"/> if the contents of the two specified objects are equal; otherwise, <see langword="false"/>.
        /// </returns>
        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        public static bool IsEqualByContentsTo<T>(this T obj, T other) => AreEqualByContentsInternal(obj, other);

        private static bool IsSimpleTypeInternal(this Type type)
            => type.IsPrimitive
                || type.IsEnum
                || type.IsPointer
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(Pointer)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset);

        private static FieldInfo[] GetContentFieldsCacheFields(Type type)
            => type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        [DebuggerNonUserCode]
        private static void ToPropertyStringInternal<T>(
            [CanBeNull] T? obj,
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
                if (_toPropertyStringObjectsBeingProcessed is null)
                {
                    isToPropertyStringObjectsBeingProcessedCreated = true;

                    _toPropertyStringObjectsBeingProcessed = new HashSet<object>(ByReferenceEqualityComparer<object>.Instance);
                }

                void OpenBrace()
                {
                    if (isBraceOpen || isRoot)
                    {
                        return;
                    }

                    resultBuilder.Append(ComplexObjectOpeningBrace);
                    isBraceOpen = true;
                }

                var nextRecursionLevel = recursionLevel + 1;

                var type = obj.GetTypeSafely();

                string RenderActualType() => AsInvariant($@"{type.GetQualifiedName()} :: ");

                var shouldRenderActualType = isRoot ? options.RenderRootActualType : options.RenderActualType;

                if (shouldRenderActualType)
                {
                    OpenBrace();
                    resultBuilder.Append(RenderActualType());
                }

                if (obj is null)
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
                    resultBuilder.Append(
                        AsInvariant(
                            $@"{ComplexObjectOpeningBrace} {(shouldRenderActualType ? string.Empty : RenderActualType())}<- {
                                ComplexObjectClosingBrace}"));

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

                    ProcessSingleProperty(propertyInfo);
                }

                void ProcessSingleProperty(PropertyInfo propertyInfo)
                {
                    resultBuilder.Append(propertyInfo.Name);
                    if (options.RenderMemberType)
                    {
                        resultBuilder.Append(@":");
                        resultBuilder.Append(propertyInfo.PropertyType.GetQualifiedName());
                    }

                    resultBuilder.Append(PropertyNameValueSeparator);

                    object? propertyValue;
                    try
                    {
                        propertyValue = propertyInfo.GetValue(obj, null);
                    }
                    catch (Exception ex)
                    {
                        //// ReSharper disable once ConstantNullCoalescingCondition
                        var underlyingException = ex.GetBaseException() ?? ex;

                        resultBuilder.Append(
                            AsInvariant(
                                $@"{{ Error getting the property value: [{underlyingException.GetType().Name}] {
                                    underlyingException.Message} }}"));

                        return;
                    }

                    var typeArgument = propertyInfo.PropertyType.IsPointer ? typeof(object) : propertyInfo.PropertyType;
                    var method = ToPropertyStringInternalMethodDefinition.MakeGenericMethod(typeArgument);

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
                    //// ReSharper disable once RedundantSuppressNullableWarningExpression
                    _toPropertyStringObjectsBeingProcessed!.Remove(obj!);
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
                    resultBuilder.AppendFormat(PointerStringFormat, (long)Pointer.Unbox(obj!));
                }
            }
            else if (type == typeof(IntPtr))
            {
                resultBuilder.AppendFormat(PointerStringFormat, ((IntPtr)(object)obj!).ToInt64());
            }
            else if (type == typeof(UIntPtr))
            {
                resultBuilder.AppendFormat(PointerStringFormat, ((UIntPtr)(object)obj!).ToUInt64());
            }
            else if (type == typeof(string))
            {
                resultBuilder.Append(((string)(object)obj!).ToUIString());
            }
            else if (type.IsEnum && type.IsDefined(typeof(FlagsAttribute), false))
            {
                resultBuilder.Append(ComplexObjectOpeningBrace + obj.ToStringSafelyInvariant() + ComplexObjectClosingBrace);
            }
            else if (type == typeof(DateTime))
            {
                resultBuilder.Append(((DateTime)(object)obj!).ToPreciseFixedString());
            }
            else if (type == typeof(DateTimeOffset))
            {
                resultBuilder.Append(((DateTimeOffset)(object)obj!).ToPreciseFixedString());
            }
            else if (typeof(Type).IsAssignableFrom(type))
            {
                resultBuilder.Append(((Type)(object)obj!).AssemblyQualifiedName.ToUIString());
            }
            else if (typeof(Assembly).IsAssignableFrom(type))
            {
                resultBuilder.Append(((Assembly)(object)obj!).Location.ToUIString());
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
            [NotNull] [InstantHandle] Func<Type, PropertyInfo[]> getProperties,
            [NotNull] StringBuilder resultBuilder,
            int nextRecursionLevel)
        {
            var elementType = type.GetCollectionElementType().EnsureNotNull();

            resultBuilder.Append(CollectionElementsOpeningBrace);
            resultBuilder.Append(CollectionElementsPropertyName);
            if (options.RenderMemberType)
            {
                resultBuilder.Append(":");
                resultBuilder.Append(elementType.GetQualifiedName());
            }

            resultBuilder.Append(CollectionElementsClosingBrace);

            resultBuilder.Append(PropertyNameValueSeparator);
            resultBuilder.Append(ComplexObjectOpeningBrace);

            var count = 0;

            using var enumeratorWrapper = SmartDisposable.Create(enumerable.GetEnumerator());
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

                object? currentValue;
                try
                {
                    currentValue = enumeratorWrapper.Instance.Current;
                }
                catch (Exception ex)
                {
                    //// ReSharper disable once ConstantNullCoalescingCondition
                    var underlyingException = ex.GetBaseException() ?? ex;

                    resultBuilder.Append(
                        AsInvariant(
                            $@"{{ Error getting the collection element at index {count - 1}: [{underlyingException.GetType().Name}] {
                                underlyingException.Message} }}"));

                    continue;
                }

                var typeArgument = elementType.IsPointer ? typeof(object) : elementType;
                var method = ToPropertyStringInternalMethodDefinition.MakeGenericMethod(typeArgument);

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

            resultBuilder.Append(ComplexObjectClosingBrace);
        }

        private static bool AreEqualByContentsInternal(object? valueA, object? valueB)
        {
            var isAssertEqualityByContentObjectsBeingProcessedCreated = false;
            var isObjectPairAddedToBeingProcessed = false;
            try
            {
                if (ReferenceEquals(valueA, valueB))
                {
                    return true;
                }

                if (valueA is null || valueB is null)
                {
                    return false;
                }

                var actualType = valueA.GetType();
                if (valueB.GetType() != actualType)
                {
                    return false;
                }

                if (actualType.IsSimpleTypeInternal())
                {
                    return Equals(valueA, valueB);
                }

                if (_assertEqualityByContentsObjectsBeingProcessed is null)
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
                    return true;
                }

                //// ReSharper disable once LoopCanBeConvertedToQuery
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
                    //// ReSharper disable RedundantSuppressNullableWarningExpression
                    _assertEqualityByContentsObjectsBeingProcessed!.Remove(new PairReferenceHolder(valueA!, valueB!));
                    //// ReSharper restore RedundantSuppressNullableWarningExpression
                }

                if (isAssertEqualityByContentObjectsBeingProcessedCreated)
                {
                    _assertEqualityByContentsObjectsBeingProcessed = null;
                }
            }

            return true;
        }

        [MethodImpl(
            MethodImplOptions.AggressiveInlining
#if NET5_0_OR_GREATER
            | MethodImplOptions.AggressiveOptimization
#endif
        )]
        private static string GetObjectReferenceDescriptionInternal<T>(T? obj, [InstantHandle] Func<Type, string> formatType)
            where T : class
            => obj is null
                ? OmnifactotumRepresentationConstants.NullValueRepresentation
                : $@"{formatType(obj.GetType())}:0x{RuntimeHelpers.GetHashCode(obj):X8}";

        private readonly struct PairReferenceHolder : IEquatable<PairReferenceHolder>
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

            public override bool Equals(object? obj) => obj is PairReferenceHolder holder && Equals(holder);

            public override int GetHashCode()
                => EqualityComparer.GetHashCode(_valueA).CombineHashCodeValues(EqualityComparer.GetHashCode(_valueB));

            public bool Equals(PairReferenceHolder other)
                => ReferenceEquals(_valueA, other._valueA) && ReferenceEquals(_valueB, other._valueB);
        }
    }
}