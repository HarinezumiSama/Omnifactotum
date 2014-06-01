using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Provides helper methods and properties for common use.
    /// </summary>
    public static class Factotum
    {
        #region Constants and Fields

        /// <summary>
        ///     The minimum size of a generated identifier part (see <see cref="Factotum.GenerateId"/> and
        ///     <see cref="IdGenerationModes"/>).
        /// </summary>
        public static readonly int MinimumGeneratedIdPartSize = Marshal.SizeOf(typeof(Guid));

        /// <summary>
        ///     The pointer string format.
        /// </summary>
        internal static readonly string PointerStringFormat = string.Format(
            "0x{{0:X{0}}}",
            Marshal.SizeOf(typeof(IntPtr)) * 2);

        private const string NullString = "<null>";

        private const string InvalidExpressionMessageFormat =
            "Invalid expression (must be a getter of a property of the type '{0}'): {{ {1} }}.";

        private const string InvalidExpressionMessageAutoFormat =
            "Invalid expression (must be a getter of a property of some type): {{ {0} }}.";

        private static readonly MethodInfo ToPropertyStringInternalMethodDefinition =
            new ToPropertyStringInternalMethodStub(ToPropertyStringInternal).Method.GetGenericMethodDefinition();

        private static readonly WeakReferenceBasedCache<Type, FieldInfo[]> ContentFieldsCache =
            new WeakReferenceBasedCache<Type, FieldInfo[]>(
                t => t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));

        private static readonly int GuidSize = Marshal.SizeOf(typeof(Guid));

        private static readonly RNGCryptoServiceProvider IdGenerator = new RNGCryptoServiceProvider();

        [ThreadStatic]
        private static HashSet<object> _toPropertyStringObjectsBeingProcessed;

        [ThreadStatic]
        private static StringBuilder _toPropertyStringResultBuilder;

        [ThreadStatic]
        private static HashSet<PairReferenceHolder> _assertEqualityByContentsObjectsBeingProcessed;

        #endregion

        #region Delegates

        private delegate void ToPropertyStringInternalMethodStub(
            object obj,
            bool isRoot,
            ToPropertyStringOptions options,
            Func<Type, PropertyInfo[]> getProperties,
            StringBuilder resultBuilder,
            int recursionLevel);

        #endregion

        #region Public Methods: General

        /// <summary>
        ///     Calls the <see cref="IDisposable.Dispose"/> method of the instance, passed by reference, implementing
        ///     the <see cref="IDisposable"/> interface and sets the reference to this object to <b>null</b>.
        ///     If the <see cref="IDisposable.Dispose"/> method implementation throws an exception,
        ///     the reference to the instance remains unchanged.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the disposable instance.
        /// </typeparam>
        /// <param name="disposable">
        ///     A reference to an object to dispose and set to <b>null</b>.
        /// </param>
        public static void DisposeAndNull<T>([CanBeNull] ref T disposable)
            where T : class, IDisposable
        {
            if (ReferenceEquals(disposable, null))
            {
                return;
            }

            disposable.Dispose();
            disposable = null;
        }

        /// <summary>
        ///     Exchanges two specified values, passed by reference. This method is not thread-safe.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the values to exchange.
        /// </typeparam>
        /// <param name="value1">
        ///     The first value to exchange with the second value.
        /// </param>
        /// <param name="value2">
        ///     The second value to exchange with the first value.
        /// </param>
        [DebuggerNonUserCode]
        public static void Exchange<T>([CanBeNull] ref T value1, [CanBeNull] ref T value2)
        {
            var temporary = value1;
            value1 = value2;
            value2 = temporary;
        }

        /// <summary>
        ///     Represents an identity function which returns the instance passed as an argument.
        ///     The intended usage is instead of identity lambda expression similar to <c>obj => obj</c>.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instance to receive and return.
        /// </typeparam>
        /// <param name="obj">
        ///     The instance to return.
        /// </param>
        /// <returns>
        ///     The instance passed as an argument.
        /// </returns>
        public static T Identity<T>([CanBeNull] T obj)
        {
            return obj;
        }

        /// <summary>
        ///     Sets the values of the public instance properties of the specified object to the values indicated
        ///     in the <see cref="DefaultValueAttribute">DefaultValue</see> attribute which a property is marked with.
        ///     If a property is not marked with the <see cref="DefaultValueAttribute">DefaultValue</see> attribute,
        ///     its value remains unchanged.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the object whose properties to change.
        /// </typeparam>
        /// <param name="obj">
        ///     The object whose properties to change.
        /// </param>
        /// <returns>
        ///     The input object.
        /// </returns>
        [DebuggerNonUserCode]
        public static T SetDefaultValues<T>([NotNull] T obj)
            where T : class
        {
            #region Argument Check

            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            #endregion

            var propertyRecords = obj.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(item => item.CanWrite && item.GetIndexParameters().Length == 0)
                .Select(
                    item => new
                    {
                        Property = item,
                        Attribute = item.GetSingleOrDefaultCustomAttribute<DefaultValueAttribute>(false)
                    })
                .Where(item => item.Attribute != null)
                .Select(
                    item => new
                    {
                        item.Property,
                        Default = item.Attribute.Value
                    })
                .ToArray();

            foreach (var propertyRecord in propertyRecords)
            {
                propertyRecord.Property.SetValue(obj, propertyRecord.Default, null);
            }

            return obj;
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
        public static string ToPropertyString<T>([CanBeNull] T obj, [CanBeNull] ToPropertyStringOptions options)
        {
            var actualOptions = options ?? new ToPropertyStringOptions();

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
            if (actualOptions.IncludeNonPublicMembers)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }

            Func<Type, PropertyInfo[]> getProperties =
                type =>
                {
                    var getPropertiesQuery = type
                        .GetProperties(bindingFlags)
                        .Where(item => item.CanRead && !item.GetIndexParameters().Any());
                    if (actualOptions.SortMembersAlphabetically)
                    {
                        getPropertiesQuery = getPropertiesQuery.OrderBy(item => item.Name);
                    }

                    return getPropertiesQuery.ToArray();
                };

            if (_toPropertyStringResultBuilder == null)
            {
                _toPropertyStringResultBuilder = new StringBuilder(128);
            }
            else
            {
                _toPropertyStringResultBuilder.Clear();
            }

            ToPropertyStringInternal(obj, true, actualOptions, getProperties, _toPropertyStringResultBuilder, 0);

            var result = _toPropertyStringResultBuilder.ToString();
            _toPropertyStringResultBuilder.Clear();
            return result;
        }

        /// <summary>
        ///     Determines if the contents of the two specified objects are equal, that is, if the objects are of
        ///     the same type and the values of all their corresponding instance fields are equal.
        /// </summary>
        /// <remarks>
        ///     This method uses reflection to obtain the list of fields for comparison.
        ///     This method recursively processes the composite objects, if any.
        /// </remarks>
        /// <typeparam name="T">
        ///     The type of the objects to compare.
        /// </typeparam>
        /// <param name="valueA">
        ///     The first object to compare.
        /// </param>
        /// <param name="valueB">
        ///     The second object to compare.
        /// </param>
        /// <returns>
        ///     <b>true</b> if the contents of the two specified objects are equal; otherwise, <b>false</b>.
        /// </returns>
        public static bool AreEqualByContents<T>([CanBeNull] T valueA, [CanBeNull] T valueB)
        {
            return AreEqualByContentsInternal(valueA, valueB);
        }

        /// <summary>
        ///     Compares the two specified values and returns the larger of those.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of values to compare.
        /// </typeparam>
        /// <param name="x">
        ///     The first value to compare.
        /// </param>
        /// <param name="y">
        ///     The second value to compare.
        /// </param>
        /// <returns>
        ///     The larger of the two specified values.
        /// </returns>
        public static T Max<T>(T x, T y)
            where T : IComparable
        {
            return Comparer<T>.Default.Compare(x, y) > 0 ? x : y;
        }

        /// <summary>
        ///     Compares the two specified values and returns the smaller of those.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of values to compare.
        /// </typeparam>
        /// <param name="x">
        ///     The first value to compare.
        /// </param>
        /// <param name="y">
        ///     The second value to compare.
        /// </param>
        /// <returns>
        ///     The smaller of the two specified values.
        /// </returns>
        public static T Min<T>(T x, T y)
            where T : IComparable
        {
            return Comparer<T>.Default.Compare(x, y) < 0 ? x : y;
        }

        /// <summary>
        ///     Generates an identifier which is unique, cryptographically random, or both.
        /// </summary>
        /// <param name="size">
        ///     <para>The size, in bytes, of the resulting identifier.</para>
        ///     <para>
        ///         This value must be at least <see cref="Factotum.MinimumGeneratedIdPartSize"/> when either
        ///         <see cref="IdGenerationModes.Unique"/> or <see cref="IdGenerationModes.Random"/> is solely
        ///         specified, and it must be at least twice as <see cref="Factotum.MinimumGeneratedIdPartSize"/> if
        ///         both modes are specified.
        ///     </para>
        /// </param>
        /// <param name="modes">
        ///     Specifies the modes of identifier generation.
        /// </param>
        /// <returns>
        ///     An array of bytes representing the generated identifier.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="size"/> is less than required by the specified <paramref name="modes"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="modes"/> does not specify anything to generate.
        /// </exception>
        public static byte[] GenerateId(int size, IdGenerationModes modes)
        {
            var minimumSize = 0;
            var generateUniquePart = false;
            var generateRandomPart = false;

            if (modes.IsAnySet(IdGenerationModes.Unique))
            {
                generateUniquePart = true;
                minimumSize += MinimumGeneratedIdPartSize;
            }

            if (modes.IsAnySet(IdGenerationModes.Random))
            {
                generateRandomPart = true;
                minimumSize += MinimumGeneratedIdPartSize;
            }

            #region Argument Check

            if (!generateUniquePart && !generateRandomPart)
            {
                throw new ArgumentException("There is nothing to generate.", "modes");
            }

            if (size < minimumSize)
            {
                var errorMessage = string.Format(
                    CultureInfo.InvariantCulture,
                    "For the specified mode(s), the size must be at least {0}.",
                    minimumSize);
                throw new ArgumentOutOfRangeException("size", size, errorMessage);
            }

            #endregion

            var result = new byte[size];

            var offset = 0;
            if (generateUniquePart)
            {
                var remainingUniquePartSize = generateRandomPart ? GuidSize : size;
                while (remainingUniquePartSize > 0)
                {
                    var uniquePart = Guid.NewGuid().ToByteArray();

                    var blockSize = Math.Min(remainingUniquePartSize, GuidSize);
                    Array.Copy(uniquePart, 0, result, offset, blockSize);
                    offset += blockSize;
                    remainingUniquePartSize -= blockSize;
                }
            }

            if (generateRandomPart)
            {
                var randomPartSize = size - offset;

                // According to the documentation, GetBytes is thread-safe
                // However, using a lock here to be on the safe side
                var randomPart = new byte[randomPartSize];
                lock (IdGenerator)
                {
                    IdGenerator.GetBytes(randomPart);
                }

                Array.Copy(randomPart, 0, result, offset, randomPart.Length);
            }

            return result;
        }

        /// <summary>
        ///     Generates an identifier, which is unique, cryptographically random, or both, and returns its
        ///     hexadecimal representation.
        /// </summary>
        /// <param name="size">
        ///     <para>The size, in bytes, of the resulting identifier.</para>
        ///     <para>
        ///         This value must be at least <see cref="Factotum.MinimumGeneratedIdPartSize"/> when either
        ///         <see cref="IdGenerationModes.Unique"/> or <see cref="IdGenerationModes.Random"/> is solely
        ///         specified, and it must be at least twice as <see cref="Factotum.MinimumGeneratedIdPartSize"/> if
        ///         both modes are specified.
        ///     </para>
        /// </param>
        /// <param name="modes">
        ///     Specifies the modes of identifier generation.
        /// </param>
        /// <returns>
        ///     A hexadecimal representation of the generated identifier.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="size"/> is less than required by the specified <paramref name="modes"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="modes"/> does not specify anything to generate.
        /// </exception>
        public static string GenerateIdString(int size, IdGenerationModes modes)
        {
            var id = GenerateId(size, modes);
            return id.ToHexString();
        }

        /// <summary>
        ///     <para>Gets the local path of the process executable.</para>
        ///     <para><b>NOTE</b>: The entry assembly is used to determine the executable path.
        ///     If the entry assembly is not available, the calling assembly is used.</para>
        /// </summary>
        /// <returns>
        ///     The local path of the process executable.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The entry assembly (or the calling assembly) does not have a local path.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetExecutableLocalPath()
        {
            //// TODO [vmcl] Cache the result

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            return assembly.GetLocalPath();
        }

        /// <summary>
        ///     <para>Gets the directory of the process executable.</para>
        ///     <para><b>NOTE</b>: The entry assembly is used to determine the executable directory.
        ///     If the entry assembly is not available, the calling assembly is used.</para>
        /// </summary>
        /// <returns>
        ///     The directory of the process executable.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The entry assembly (or the calling assembly) does not have a local path.
        /// </exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetExecutableDirectory()
        {
            //// [vmcl] GetExecutableLocalPath() method cannot be re-used in order to keep
            //// the correct stack for Assembly.GetCallingAssembly()

            //// TODO [vmcl] Cache the result

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var path = assembly.GetLocalPath();

            return Path.GetDirectoryName(path).EnsureNotNull();
        }

        #endregion

        #region Public Methods: Fields/Properties

        /// <summary>
        ///     Gets the <see cref="MemberInfo"/> of the field or property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type containing the field or property.
        /// </typeparam>
        /// <typeparam name="TMember">
        ///     The type of the field or property.
        /// </typeparam>
        /// <param name="memberGetterExpression">
        ///     The lambda expression in the following form: <c>(SomeType x) =&gt; x.Member</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="MemberInfo"/> containing information about the required field or property.
        /// </returns>
        [NotNull]
        public static MemberInfo GetFieldOrPropertyInfo<TObject, TMember>(
            Expression<Func<TObject, TMember>> memberGetterExpression)
        {
            return For<TObject>.GetFieldOrPropertyInfo(memberGetterExpression);
        }

        /// <summary>
        ///     Gets the <see cref="FieldInfo"/> of the field specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type containing the field.
        /// </typeparam>
        /// <typeparam name="TField">
        ///     The type of the field.
        /// </typeparam>
        /// <param name="fieldGetterExpression">
        ///     The lambda expression in the following form: <c>(SomeType x) =&gt; x.Field</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="FieldInfo"/> containing information about the required field.
        /// </returns>
        [NotNull]
        public static FieldInfo GetFieldInfo<TObject, TField>(
            Expression<Func<TObject, TField>> fieldGetterExpression)
        {
            return For<TObject>.GetFieldInfo(fieldGetterExpression);
        }

        /// <summary>
        ///     Gets the name of the field specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type containing the field.
        /// </typeparam>
        /// <typeparam name="TField">
        ///     The type of the field.
        /// </typeparam>
        /// <param name="fieldGetterExpression">
        ///     The lambda expression in the following form: <c>(SomeType x) =&gt; x.Field</c>.
        /// </param>
        /// <returns>
        ///     The name of the field specified by the lambda expression.
        /// </returns>
        [NotNull]
        public static string GetFieldName<TObject, TField>(
            Expression<Func<TObject, TField>> fieldGetterExpression)
        {
            return For<TObject>.GetFieldName(fieldGetterExpression);
        }

        /// <summary>
        ///     Gets the type-qualified name of the field specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type containing the field.
        /// </typeparam>
        /// <typeparam name="TField">
        ///     The type of the field.
        /// </typeparam>
        /// <param name="fieldGetterExpression">
        ///     The lambda expression in the following form: <c>(SomeType x) =&gt; x.Field</c>.
        /// </param>
        /// <returns>
        ///     The name of the field in the following form: <c>SomeType.Field</c>.
        /// </returns>
        [NotNull]
        public static string GetQualifiedFieldName<TObject, TField>(
            Expression<Func<TObject, TField>> fieldGetterExpression)
        {
            return For<TObject>.GetQualifiedFieldName(fieldGetterExpression);
        }

        /// <summary>
        ///     Gets the <see cref="PropertyInfo"/> of the property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type containing the property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        ///     The type of the property.
        /// </typeparam>
        /// <param name="propertyGetterExpression">
        ///     The lambda expression in the following form: <c>(SomeType x) =&gt; x.Property</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo"/> containing information about the required property.
        /// </returns>
        [NotNull]
        public static PropertyInfo GetPropertyInfo<TObject, TProperty>(
            Expression<Func<TObject, TProperty>> propertyGetterExpression)
        {
            return For<TObject>.GetPropertyInfo(propertyGetterExpression);
        }

        /// <summary>
        ///     Gets the name of the property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type containing the property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        ///     The type of the property.
        /// </typeparam>
        /// <param name="propertyGetterExpression">
        ///     The lambda expression in the following form: <c>(SomeType x) =&gt; x.Property</c>.
        /// </param>
        /// <returns>
        ///     The name of the property.
        /// </returns>
        [NotNull]
        public static string GetPropertyName<TObject, TProperty>(
            Expression<Func<TObject, TProperty>> propertyGetterExpression)
        {
            return For<TObject>.GetPropertyName(propertyGetterExpression);
        }

        /// <summary>
        ///     Gets the type-qualified name of the property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type containing the property.
        /// </typeparam>
        /// <typeparam name="TProperty">
        ///     The type of the property.
        /// </typeparam>
        /// <param name="propertyGetterExpression">
        ///     The lambda expression in the following form: <c>(SomeType x) =&gt; x.Property</c>.
        /// </param>
        /// <returns>
        ///     The name of the property in the following form: <c>SomeType.Property</c>.
        /// </returns>
        [NotNull]
        public static string GetQualifiedPropertyName<TObject, TProperty>(
            Expression<Func<TObject, TProperty>> propertyGetterExpression)
        {
            return For<TObject>.GetQualifiedPropertyName(propertyGetterExpression);
        }

        /// <summary>
        ///     Gets the <see cref="PropertyInfo"/> of the static property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TProperty">
        ///     The type of the static property.
        /// </typeparam>
        /// <param name="propertyGetterExpression">
        ///     The lambda expression in the following form: <c>() =&gt; propertyExpression</c>.
        /// </param>
        /// <returns>
        ///     The <see cref="PropertyInfo"/> containing information about the required static property.
        /// </returns>
        [NotNull]
        public static PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TProperty>> propertyGetterExpression)
        {
            #region Argument Check

            if (propertyGetterExpression == null)
            {
                throw new ArgumentNullException("propertyGetterExpression");
            }

            #endregion

            var memberExpression = propertyGetterExpression.Body as MemberExpression;
            if ((memberExpression == null) || (memberExpression.NodeType != ExpressionType.MemberAccess))
            {
                throw new ArgumentException(
                    string.Format(InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                    "propertyGetterExpression");
            }

            var result = memberExpression.Member as PropertyInfo;
            if (result == null)
            {
                throw new ArgumentException(
                    string.Format(InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                    "propertyGetterExpression");
            }

            if (result.DeclaringType == null)
            {
                throw new ArgumentException(
                    string.Format(InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                    "propertyGetterExpression");
            }

            if (memberExpression.Expression == null)
            {
                var accessor = result.GetGetMethod(true) ?? result.GetSetMethod(true);
                if ((accessor == null) || !accessor.IsStatic)
                {
                    throw new ArgumentException(
                        string.Format(InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                        "propertyGetterExpression");
                }
            }

            return result;
        }

        /// <summary>
        ///     Gets the name of the static property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TProperty">
        ///     The type of the static property.
        /// </typeparam>
        /// <param name="propertyGetterExpression">
        ///     The lambda expression in the following form: <c>() =&gt; propertyExpression</c>.
        /// </param>
        /// <returns>
        ///     The name of the static property.
        /// </returns>
        [NotNull]
        public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> propertyGetterExpression)
        {
            var propertyInfo = GetPropertyInfo(propertyGetterExpression);
            return propertyInfo.Name;
        }

        /// <summary>
        ///     Gets the type-qualified name of the static property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="TProperty">
        ///     The type of the static property.
        /// </typeparam>
        /// <param name="propertyGetterExpression">
        ///     The lambda expression in the following form: <c>() =&gt; propertyExpression</c>.
        /// </param>
        /// <returns>
        ///     The type-qualified name of the static property.
        /// </returns>
        [NotNull]
        public static string GetQualifiedPropertyName<TProperty>(Expression<Func<TProperty>> propertyGetterExpression)
        {
            var propertyInfo = GetPropertyInfo(propertyGetterExpression);
            return propertyInfo.DeclaringType.EnsureNotNull().GetQualifiedName() + Type.Delimiter + propertyInfo.Name;
        }

        #endregion

        #region Public Methods: Recursive Processing

        /// <summary>
        ///     Processes the specified instance recursively.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instances to process.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance to process. Can be <b>null</b>.
        /// </param>
        /// <param name="getItems">
        ///     A reference to the method that maps <paramref name="instance"/> to the collection of associated objects
        ///     to process them recursively.
        /// </param>
        /// <param name="processItem">
        ///     A reference to the method that processes each single item.
        /// </param>
        /// <param name="processingContext">
        ///     The context of the recursive processing, or <b>null</b> to use a new context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>
        ///         <paramref name="getItems"/> is <b>null</b>.
        ///     </para>
        ///     <para>-or-</para>
        ///     <para>
        ///         <paramref name="processItem"/> is <b>null</b>.
        ///     </para>
        /// </exception>
        public static void ProcessRecursively<T>(
            [CanBeNull] T instance,
            [NotNull] Func<T, IEnumerable<T>> getItems,
            [NotNull] Func<T, RecursiveProcessingDirective> processItem,
            [CanBeNull] RecursiveProcessingContext<T> processingContext)
        {
            #region Argument Check

            if (getItems == null)
            {
                throw new ArgumentNullException("getItems");
            }

            if (processItem == null)
            {
                throw new ArgumentNullException("processItem");
            }

            #endregion

            if (ReferenceEquals(instance, null))
            {
                return;
            }

            var actualProcessingContext = processingContext ?? new RecursiveProcessingContext<T>();
            ProcessRecursivelyInternal(instance, getItems, processItem, actualProcessingContext);
        }

        /// <summary>
        ///     Processes the specified instance recursively.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instances to process.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance to process. Can be <b>null</b>.
        /// </param>
        /// <param name="getItems">
        ///     A reference to the method that maps <paramref name="instance"/> to the collection of associated objects
        ///     to process them recursively.
        /// </param>
        /// <param name="processItem">
        ///     A reference to the method that processes each single item.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>
        ///         <paramref name="getItems"/> is <b>null</b>.
        ///     </para>
        ///     <para>-or-</para>
        ///     <para>
        ///         <paramref name="processItem"/> is <b>null</b>.
        ///     </para>
        /// </exception>
        public static void ProcessRecursively<T>(
            [CanBeNull] T instance,
            [NotNull] Func<T, IEnumerable<T>> getItems,
            [NotNull] Func<T, RecursiveProcessingDirective> processItem)
        {
            ProcessRecursively(instance, getItems, processItem, null);
        }

        /// <summary>
        ///     Processes the specified instance recursively.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instances to process.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance to process. Can be <b>null</b>.
        /// </param>
        /// <param name="getItems">
        ///     A reference to the method that maps <paramref name="instance"/> to the collection of associated objects
        ///     to process them recursively.
        /// </param>
        /// <param name="processItem">
        ///     A reference to the method that processes each single item.
        /// </param>
        /// <param name="processingContext">
        ///     The context of the recursive processing, or <b>null</b> to use a new context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>
        ///         <paramref name="getItems"/> is <b>null</b>.
        ///     </para>
        ///     <para>-or-</para>
        ///     <para>
        ///         <paramref name="processItem"/> is <b>null</b>.
        ///     </para>
        /// </exception>
        public static void ProcessRecursively<T>(
            [CanBeNull] T instance,
            [NotNull] Func<T, IEnumerable<T>> getItems,
            [NotNull] Action<T> processItem,
            [CanBeNull] RecursiveProcessingContext<T> processingContext)
        {
            #region Argument Check

            if (processItem == null)
            {
                throw new ArgumentNullException("processItem");
            }

            #endregion

            ProcessRecursively(
                instance,
                getItems,
                item =>
                {
                    processItem(item);
                    return RecursiveProcessingDirective.Continue;
                },
                processingContext);
        }

        /// <summary>
        ///     Processes the specified instance recursively.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instances to process.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance to process. Can be <b>null</b>.
        /// </param>
        /// <param name="getItems">
        ///     A reference to the method that maps <paramref name="instance"/> to the collection of associated objects
        ///     to process them recursively.
        /// </param>
        /// <param name="processItem">
        ///     A reference to the method that processes each single item.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>
        ///         <paramref name="getItems"/> is <b>null</b>.
        ///     </para>
        ///     <para>-or-</para>
        ///     <para>
        ///         <paramref name="processItem"/> is <b>null</b>.
        ///     </para>
        /// </exception>
        public static void ProcessRecursively<T>(
            T instance,
            [NotNull] Func<T, IEnumerable<T>> getItems,
            [NotNull] Action<T> processItem)
        {
            ProcessRecursively(instance, getItems, processItem, null);
        }

        #endregion

        #region Private Methods

        private static bool IsSimpleTypeInternal(this Type type)
        {
            #region Argument Check

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            #endregion

            return type.IsPrimitive
                || type.IsEnum
                || type.IsPointer
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(Pointer)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset);
        }

        private static bool ProcessRecursivelyInternal<T>(
            T instance,
            Func<T, IEnumerable<T>> getItems,
            Func<T, RecursiveProcessingDirective> processItem,
            RecursiveProcessingContext<T> processingContext)
        {
            if (ReferenceEquals(instance, null))
            {
                return true;
            }

            if (processingContext.ItemsBeingProcessed != null)
            {
                if (processingContext.ItemsBeingProcessed.Contains(instance))
                {
                    return true;
                }

                processingContext.ItemsBeingProcessed.Add(instance);
            }

            var processingResult = processItem(instance);
            switch (processingResult)
            {
                case RecursiveProcessingDirective.Continue:
                    //// Nothing to do
                    break;

                case RecursiveProcessingDirective.NoRecursionForItem:
                    return true;

                case RecursiveProcessingDirective.Terminate:
                    return false;

                default:
                    throw processingResult.CreateEnumValueNotImplementedException();
            }

            var items = getItems(instance);
            if (items == null)
            {
                return true;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery - More readable in 'foreach' style
            foreach (var item in items)
            {
                var processResult = ProcessRecursivelyInternal(item, getItems, processItem, processingContext);
                if (!processResult)
                {
                    return false;
                }
            }

            return true;
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
            const string ItemSeparator = ", ";
            const string PropertyNameValueSeparator = " = ";
            const string CollectionElementsPropertyName = "Elements";
            const string CollectionElementsOpeningBrace = "[";
            const string CollectionElementsClosingBrace = "]";
            const string ComplexObjectOpeningBrace = "{";
            const string ComplexObjectClosingBrace = "}";

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

                Action openBrace =
                    () =>
                    {
                        if (!isBraceOpen && !isRoot)
                        {
                            resultBuilder.Append(ComplexObjectOpeningBrace);
                            isBraceOpen = true;
                        }
                    };

                var nextRecursionLevel = recursionLevel + 1;

                var type = obj.GetTypeSafely();

                Func<string> renderActualType = () => string.Format("{0} :: ", type.GetQualifiedName());

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
                    openBrace();
                    resultBuilder.Append(renderActualType());
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
                        shouldRenderActualType ? string.Empty : renderActualType(),
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

                    return;
                }

                openBrace();

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

                var enumerable = obj as IEnumerable;
                if (enumerable != null)
                {
                    var elementType = type.GetCollectionElementType().EnsureNotNull();

                    propertySeparatorNeeded = true;
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

                // ReSharper disable once LoopCanBeConvertedToQuery - More readable in 'foreach' style
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

        #endregion

        #region PairReferenceHolder Class

        private struct PairReferenceHolder : IEquatable<PairReferenceHolder>
        {
            #region Constants and Fields

            private readonly object _valueA;
            private readonly object _valueB;

            #endregion

            #region Constructors

            internal PairReferenceHolder(object valueA, object valueB)
            {
                _valueA = valueA;
                _valueB = valueB;
            }

            #endregion

            #region Public Methods

            public override bool Equals(object obj)
            {
                return obj is PairReferenceHolder && Equals((PairReferenceHolder)obj);
            }

            public override int GetHashCode()
            {
                var equalityComparer = ByReferenceEqualityComparer<object>.Instance;

                return equalityComparer.GetHashCode(this._valueA)
                    .CombineHashCodeValues(equalityComparer.GetHashCode(this._valueB));
            }

            #endregion

            #region IEquatable<PairReferenceHolder> Members

            public bool Equals(PairReferenceHolder other)
            {
                return ReferenceEquals(_valueA, other._valueA) && ReferenceEquals(_valueB, other._valueB);
            }

            #endregion
        }

        #endregion

        //// TODO [vmcl] Move For<TObject> class to a separate _related_ file (make the Factotum class partial)

        #region For<T> Class

        /// <summary>
        ///     Provides a convenient access to helper methods for the specified type.
        /// </summary>
        /// <typeparam name="TObject">
        ///     The type that the helper methods are provided for.
        /// </typeparam>
        public static class For<TObject>
        {
            #region Public Methods

            /// <summary>
            ///     Gets the <see cref="MemberInfo"/> of the field or property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TMember">
            ///     The type of the field or property.
            /// </typeparam>
            /// <param name="memberGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Member</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="MemberInfo"/> containing information about the required field or property.
            /// </returns>
            [NotNull]
            public static MemberInfo GetFieldOrPropertyInfo<TMember>(
                Expression<Func<TObject, TMember>> memberGetterExpression)
            {
                var result = GetDataMemberInfo(memberGetterExpression);
                return result;
            }

            /// <summary>
            ///     Gets the <see cref="FieldInfo"/> of the field specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TField">
            ///     The type of the field.
            /// </typeparam>
            /// <param name="fieldGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Field</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="FieldInfo"/> containing information about the required field.
            /// </returns>
            [NotNull]
            public static FieldInfo GetFieldInfo<TField>(
                Expression<Func<TObject, TField>> fieldGetterExpression)
            {
                var memberInfo = GetDataMemberInfo(fieldGetterExpression);

                var result = memberInfo as FieldInfo;
                if (result == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            typeof(TObject).GetFullName(),
                            fieldGetterExpression),
                        "fieldGetterExpression");
                }

                return result;
            }

            /// <summary>
            ///     Gets the name of the field specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TField">
            ///     The type of the field.
            /// </typeparam>
            /// <param name="fieldGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Field</c>.
            /// </param>
            /// <returns>
            ///     The name of the field specified by the lambda expression.
            /// </returns>
            [NotNull]
            public static string GetFieldName<TField>(
                Expression<Func<TObject, TField>> fieldGetterExpression)
            {
                var fieldInfo = GetFieldInfo(fieldGetterExpression);
                return fieldInfo.Name;
            }

            /// <summary>
            ///     Gets the type-qualified name of the field specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TField">
            ///     The type of the field.
            /// </typeparam>
            /// <param name="fieldGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Field</c>.
            /// </param>
            /// <returns>
            ///     The name of the field in the following form: <c>SomeType.Field</c>.
            /// </returns>
            [NotNull]
            public static string GetQualifiedFieldName<TField>(
                Expression<Func<TObject, TField>> fieldGetterExpression)
            {
                var fieldInfo = GetFieldInfo(fieldGetterExpression);
                return typeof(TObject).GetQualifiedName() + Type.Delimiter + fieldInfo.Name;
            }

            /// <summary>
            ///     Gets the <see cref="PropertyInfo"/> of the property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Property</c>.
            /// </param>
            /// <returns>
            ///     The <see cref="PropertyInfo"/> containing information about the required property.
            /// </returns>
            [NotNull]
            public static PropertyInfo GetPropertyInfo<TProperty>(
                Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var memberInfo = GetDataMemberInfo(propertyGetterExpression);

                var result = memberInfo as PropertyInfo;
                if (result == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            typeof(TObject).GetFullName(),
                            propertyGetterExpression),
                        "propertyGetterExpression");
                }

                return result;
            }

            /// <summary>
            ///     Gets the name of the property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Property</c>.
            /// </param>
            /// <returns>
            ///     The name of the property.
            /// </returns>
            [NotNull]
            public static string GetPropertyName<TProperty>(
                Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var propertyInfo = GetPropertyInfo(propertyGetterExpression);
                return propertyInfo.Name;
            }

            /// <summary>
            ///     Gets the type-qualified name of the property specified by the lambda expression.
            /// </summary>
            /// <typeparam name="TProperty">
            ///     The type of the property.
            /// </typeparam>
            /// <param name="propertyGetterExpression">
            ///     The lambda expression in the following form: <c>x =&gt; x.Property</c>.
            /// </param>
            /// <returns>
            ///     The name of the property in the following form: <c>SomeType.Property</c>.
            /// </returns>
            [NotNull]
            public static string GetQualifiedPropertyName<TProperty>(
                Expression<Func<TObject, TProperty>> propertyGetterExpression)
            {
                var propertyInfo = GetPropertyInfo(propertyGetterExpression);
                return typeof(TObject).GetQualifiedName() + Type.Delimiter + propertyInfo.Name;
            }

            #endregion

            #region Private Methods

            [NotNull]
            private static MemberInfo GetDataMemberInfo<TMember>(
                Expression<Func<TObject, TMember>> memberGetterExpression)
            {
                #region Argument Check

                if (memberGetterExpression == null)
                {
                    throw new ArgumentNullException("memberGetterExpression");
                }

                #endregion

                var objectType = typeof(TObject);

                var memberExpression = memberGetterExpression.Body as MemberExpression;
                if ((memberExpression == null) || (memberExpression.NodeType != ExpressionType.MemberAccess))
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        "memberGetterExpression");
                }

                var result = memberExpression.Member;

                var fieldInfo = memberExpression.Member as FieldInfo;
                var propertyInfo = memberExpression.Member as PropertyInfo;

                if (fieldInfo == null && propertyInfo == null)
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        "memberGetterExpression");
                }

                if ((result.DeclaringType == null) || !result.DeclaringType.IsAssignableFrom(objectType))
                {
                    throw new ArgumentException(
                        string.Format(
                            InvalidExpressionMessageFormat,
                            objectType.GetFullName(),
                            memberGetterExpression),
                        "memberGetterExpression");
                }

                if (memberExpression.Expression == null)
                {
                    if (propertyInfo != null)
                    {
                        var accessor = propertyInfo.GetGetMethod(true) ?? propertyInfo.GetSetMethod(true);
                        if ((accessor == null) || !accessor.IsStatic || (result.ReflectedType != objectType))
                        {
                            throw new ArgumentException(
                                string.Format(
                                    InvalidExpressionMessageFormat,
                                    objectType.GetFullName(),
                                    memberGetterExpression),
                                "memberGetterExpression");
                        }
                    }
                }
                else
                {
                    var parameterExpression = memberExpression.Expression as ParameterExpression;
                    if ((parameterExpression == null) || (parameterExpression.NodeType != ExpressionType.Parameter) ||
                        (parameterExpression.Type != typeof(TObject)))
                    {
                        throw new ArgumentException(
                            string.Format(
                                InvalidExpressionMessageFormat,
                                objectType.GetFullName(),
                                memberGetterExpression),
                            "memberGetterExpression");
                    }
                }

                return result;
            }

            #endregion
        }

        #endregion
    }
}