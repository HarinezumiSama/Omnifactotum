#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Omnifactotum.Annotations;
using NotNullIfNotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum
{
    /// <summary>
    ///     Provides the helper methods for common use.
    /// </summary>
    public static partial class Factotum
    {
        /// <summary>
        ///     The minimum size of a generated identifier part (see <see cref="Factotum.GenerateId"/> and
        ///     <see cref="IdGenerationModes"/>).
        /// </summary>
        public static readonly int MinimumGeneratedIdPartSize = Marshal.SizeOf(typeof(Guid));

        private const string InvalidExpressionMessageFormat =
            "Invalid expression (must be a getter of a property of the type {0}): {{ {1} }}.";

        private const string InvalidExpressionMessageAutoFormat =
            "Invalid expression (must be a getter of a property of some type): {{ {0} }}.";

        private static readonly int GuidSize = Marshal.SizeOf(typeof(Guid));
        private static readonly RNGCryptoServiceProvider IdGenerator = new();

        /// <summary>
        ///     <para>
        ///         Calls the <see cref="IDisposable.Dispose"/> method of the specified reference type instance,
        ///         passed by reference, implementing the <see cref="IDisposable"/> interface and
        ///         sets the reference to this object to <see langword="null"/>.
        ///     </para>
        ///     <para>
        ///         If the specified instance is already <see langword="null"/>, nothing is done.
        ///     </para>
        ///     <para>
        ///         If the <see cref="IDisposable.Dispose"/> method implementation throws an exception,
        ///         the reference to the instance remains unchanged.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the disposable instance.
        /// </typeparam>
        /// <param name="disposable">
        ///     A reference to an object to dispose and set to <see langword="null"/>.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeAndNull<T>([CanBeNull] ref T? disposable)
            where T : class, IDisposable
        {
            if (disposable is null)
            {
                return;
            }

            disposable.Dispose();
            disposable = null;
        }

        /// <summary>
        ///     <para>
        ///         Calls the <see cref="IDisposable.Dispose"/> method of the specified nullable instance,
        ///         passed by reference, implementing the <see cref="IDisposable"/> interface and
        ///         sets the reference to this object to <see langword="null"/>.
        ///     </para>
        ///     <para>
        ///         If the specified instance is already <see langword="null"/> (that is, <see cref="Nullable{T}.HasValue"/> is
        ///         <see langword="false"/>), nothing is done.
        ///     </para>
        ///     <para>
        ///         If the <see cref="IDisposable.Dispose"/> method implementation throws an exception,
        ///         the reference to the instance remains unchanged.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the disposable instance.
        /// </typeparam>
        /// <param name="disposable">
        ///     A reference to an object to dispose and set to <see langword="null"/>.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DisposeAndNull<T>([CanBeNull] ref T? disposable)
            where T : struct, IDisposable
        {
            if (!disposable.HasValue)
            {
                return;
            }

            disposable.Value.Dispose();
            disposable = default;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Exchange<T>(ref T value1, ref T value2)
        {
#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
            (value1, value2) = (value2, value1);
#else
            //// ReSharper disable once SwapViaDeconstruction :: Avoiding multi-target issues
            var temporary = value1;
            value1 = value2;
            value2 = temporary;
#endif
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
        [return: NotNullIfNotNull("obj")]
        public static T Identity<T>(T obj) => obj;

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
        public static T SetDefaultValues<T>([NotNull] T obj)
            where T : class
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var propertyRecords = obj
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(info => info.CanWrite && info.GetIndexParameters().Length == 0)
                .Select(info => OmnifactotumKeyValuePair.Create(info, info.GetSingleOrDefaultCustomAttribute<DefaultValueAttribute>(false)))
                .Where(pair => pair.Value is not null)
                .Select(pair => OmnifactotumKeyValuePair.Create(pair.Key, pair.Value!.Value))
                .ToArray();

            foreach (var propertyRecord in propertyRecords)
            {
                propertyRecord.Key.SetValue(obj, propertyRecord.Value, null);
            }

            return obj;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>(T x, T y)
            where T : IComparable
            => Comparer<T>.Default.Compare(x, y) > 0 ? x : y;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>(T x, T y)
            where T : IComparable
            => Comparer<T>.Default.Compare(x, y) < 0 ? x : y;

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

            if (!generateUniquePart && !generateRandomPart)
            {
                throw new ArgumentException("There is nothing to generate.", nameof(modes));
            }

            if (size < minimumSize)
            {
                var errorMessage = AsInvariant($@"For the specified mode(s), the size must be at least {minimumSize}.");
                throw new ArgumentOutOfRangeException(nameof(size), size, errorMessage);
            }

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

            //// ReSharper disable once InvertIf
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
            //// TODO [HarinezumiSama] Cache the result

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
            //// [HarinezumiSama] GetExecutableLocalPath() method cannot be re-used in order to keep
            //// the correct stack for Assembly.GetCallingAssembly()

            //// TODO [HarinezumiSama] Cache the result

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            var path = assembly.GetLocalPath();

            return Path.GetDirectoryName(path).EnsureNotNull();
        }

        /// <summary>
        ///     Creates a task that does nothing and whose status is <see cref="TaskStatus.RanToCompletion"/>.
        /// </summary>
        /// <returns>
        ///     An empty completed task.
        /// </returns>
        [Obsolete("This method has been deprecated. Use `" + nameof(Task) + "." + nameof(Task.CompletedTask) + "` instead.")]
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task CreateEmptyCompletedTask() => Task.CompletedTask;

        /// <summary>
        ///     Creates a task that does nothing and whose status is <see cref="TaskStatus.Faulted"/>.
        /// </summary>
        /// <returns>
        ///     An empty faulted task.
        /// </returns>
        [Obsolete("This method has been deprecated. Use `" + nameof(Task) + "." + nameof(Task.FromException) + "(Exception)` instead.")]
        [NotNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task CreateEmptyFaultedTask([NotNull] Exception exception) => Task.FromException(exception);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemberInfo GetFieldOrPropertyInfo<TObject, TMember>([NotNull] Expression<Func<TObject, TMember>> memberGetterExpression)
            => For<TObject>.GetFieldOrPropertyInfo(memberGetterExpression);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FieldInfo GetFieldInfo<TObject, TField>([NotNull] Expression<Func<TObject, TField>> fieldGetterExpression)
            => For<TObject>.GetFieldInfo(fieldGetterExpression);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetFieldName<TObject, TField>([NotNull] Expression<Func<TObject, TField>> fieldGetterExpression)
            => For<TObject>.GetFieldName(fieldGetterExpression);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetQualifiedFieldName<TObject, TField>([NotNull] Expression<Func<TObject, TField>> fieldGetterExpression)
            => For<TObject>.GetQualifiedFieldName(fieldGetterExpression);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo GetPropertyInfo<TObject, TProperty>([NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            => For<TObject>.GetPropertyInfo(propertyGetterExpression);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetPropertyName<TObject, TProperty>([NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            => For<TObject>.GetPropertyName(propertyGetterExpression);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetQualifiedPropertyName<TObject, TProperty>([NotNull] Expression<Func<TObject, TProperty>> propertyGetterExpression)
            => For<TObject>.GetQualifiedPropertyName(propertyGetterExpression);

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
        public static PropertyInfo GetPropertyInfo<TProperty>([NotNull] Expression<Func<TProperty>> propertyGetterExpression)
        {
            if (propertyGetterExpression is null)
            {
                throw new ArgumentNullException(nameof(propertyGetterExpression));
            }

            if (propertyGetterExpression.Body is not MemberExpression { NodeType: ExpressionType.MemberAccess } memberExpression)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                    nameof(propertyGetterExpression));
            }

            if (memberExpression.Member is not PropertyInfo result)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                    nameof(propertyGetterExpression));
            }

            if (result.DeclaringType is null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                    nameof(propertyGetterExpression));
            }

            //// ReSharper disable once InvertIf
            if (memberExpression.Expression is null)
            {
                var accessor = result.GetGetMethod(true) ?? result.GetSetMethod(true);
                if (accessor is null || !accessor.IsStatic)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.InvariantCulture, InvalidExpressionMessageAutoFormat, propertyGetterExpression),
                        nameof(propertyGetterExpression));
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetPropertyName<TProperty>([NotNull] Expression<Func<TProperty>> propertyGetterExpression)
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
        public static string GetQualifiedPropertyName<TProperty>([NotNull] Expression<Func<TProperty>> propertyGetterExpression)
        {
            var propertyInfo = GetPropertyInfo(propertyGetterExpression);
            return propertyInfo.DeclaringType.EnsureNotNull().GetQualifiedName() + Type.Delimiter + propertyInfo.Name;
        }

        /// <summary>
        ///     Processes the specified instance recursively.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the instances to process.
        /// </typeparam>
        /// <param name="instance">
        ///     The instance to process. Can be <see langword="null"/>.
        /// </param>
        /// <param name="getItems">
        ///     A reference to the method that maps <paramref name="instance"/> to the collection of associated objects
        ///     to process them recursively.
        /// </param>
        /// <param name="processItem">
        ///     A reference to the method that processes each single item.
        /// </param>
        /// <param name="processingContext">
        ///     The context of the recursive processing, or <see langword="null"/> to use a new context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>
        ///         <paramref name="getItems"/> is <see langword="null"/>.
        ///     </para>
        ///     <para>-or-</para>
        ///     <para>
        ///         <paramref name="processItem"/> is <see langword="null"/>.
        ///     </para>
        /// </exception>
        public static void ProcessRecursively<T>(
            T instance,
            [NotNull] [InstantHandle] Func<T, IEnumerable<T>> getItems,
            [NotNull] [InstantHandle] Func<T, RecursiveProcessingDirective> processItem,
            [CanBeNull] RecursiveProcessingContext<T>? processingContext = null)
        {
            if (getItems is null)
            {
                throw new ArgumentNullException(nameof(getItems));
            }

            if (processItem is null)
            {
                throw new ArgumentNullException(nameof(processItem));
            }

            if (instance is null)
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
        ///     The instance to process. Can be <see langword="null"/>.
        /// </param>
        /// <param name="getItems">
        ///     A reference to the method that maps <paramref name="instance"/> to the collection of associated objects
        ///     to process them recursively.
        /// </param>
        /// <param name="processItem">
        ///     A reference to the method that processes each single item.
        /// </param>
        /// <param name="processingContext">
        ///     The context of the recursive processing, or <see langword="null"/> to use a new context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <para>
        ///         <paramref name="getItems"/> is <see langword="null"/>.
        ///     </para>
        ///     <para>-or-</para>
        ///     <para>
        ///         <paramref name="processItem"/> is <see langword="null"/>.
        ///     </para>
        /// </exception>
        public static void ProcessRecursively<T>(
            T instance,
            [NotNull] [InstantHandle] Func<T, IEnumerable<T>> getItems,
            [NotNull] [InstantHandle] Action<T> processItem,
            [CanBeNull] RecursiveProcessingContext<T>? processingContext = null)
        {
            if (processItem is null)
            {
                throw new ArgumentNullException(nameof(processItem));
            }

            ProcessRecursively(instance, getItems, InternalProcessItem, processingContext);

            RecursiveProcessingDirective InternalProcessItem(T item)
            {
                processItem(item);
                return RecursiveProcessingDirective.Continue;
            }
        }

        private static bool ProcessRecursivelyInternal<T>(
            T instance,
            [NotNull] [InstantHandle] Func<T, IEnumerable<T>> getItems,
            [NotNull] [InstantHandle] Func<T, RecursiveProcessingDirective> processItem,
            [NotNull] RecursiveProcessingContext<T> processingContext)
        {
            if (instance is null)
            {
                return true;
            }

            if (processingContext.ItemsBeingProcessed is not null)
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
            //// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract :: Double checking
            if (items is null)
            {
                return true;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery :: More readable in 'foreach' style
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
    }
}