using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace

namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Type"/> type.
    /// </summary>
    public static class OmnifactotumTypeExtensions
    {
        /// <summary>
        ///     The generic argument delimiter.
        /// </summary>
        private const char GenericArgumentDelimiter = '`';

        /// <summary>
        ///     A reference to <see cref="CSharpCodeProvider"/> instance.
        /// </summary>
        private static readonly CSharpCodeProvider CSharpCodeProviderInstance = new CSharpCodeProvider();

        /// <summary>
        ///     Loads the specified manifest resource, scoped by the namespace of the specified
        ///     type, from the assembly containing this type.
        /// </summary>
        /// <param name="type">
        ///     The type that defines the assembly and namespace of the resource to load.
        /// </param>
        /// <param name="name">
        ///     The case-sensitive name of the resource to load.
        /// </param>
        /// <returns>
        ///     A <see cref="System.IO.Stream"/> representing the manifest resource, or <c>null</c> if no resources
        ///     were specified during compilation or if the resource is not visible to the caller.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="name"/> is <c>null</c> or an empty string (<see cref="String.Empty"/>).
        /// </exception>
        public static Stream GetManifestResourceStream([NotNull] this Type type, [NotNull] string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The value can be neither empty string nor null.", nameof(name));
            }

            return type.Assembly.GetManifestResourceStream(type, name);
        }

        /// <summary>
        ///     Gets the qualified name of the specified type, including its declaring type, if any, and generic
        ///     arguments and excluding the namespace.
        /// </summary>
        /// <param name="type">
        ///     The type to get the qualified name of.
        /// </param>
        /// <returns>
        ///     The qualified name of the specified type.
        /// </returns>
        /// <example>
        ///     Let there is the following type declaration.
        ///     <code>
        ///         namespace SomeNamespace1.SomeNamespace2
        ///         {
        ///             class RootType
        ///             {
        ///                 class NestedType1&lt;TKey, TValue&gt;
        ///                 {
        ///                     class NestedType2
        ///                     {
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     </code>
        ///     Then <see cref="OmnifactotumTypeExtensions.GetQualifiedName"/> returns a result as described below.
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Input Argument</term>
        ///             <description>Method Result</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 typeof(SomeNamespace1.SomeNamespace2.RootType.NestedType1&lt;,&gt;.NestedType2)
        ///             </term>
        ///             <description>RootType.NestedType1&lt;TKey, TValue&gt;.NestedType2</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 typeof(SomeNamespace1.SomeNamespace2.RootType.NestedType1&lt;int, string&gt;.NestedType2)
        ///             </term>
        ///             <description>RootType.NestedType1&lt;Int32, String&gt;.NestedType2</description>
        ///         </item>
        ///     </list>
        /// </example>
        public static string GetQualifiedName([NotNull] this Type type)
        {
            return GetNameInternal(type, false);
        }

        /// <summary>
        ///     Gets the full name of the specified type, including its declaring type, if any, and generic
        ///     arguments and including the namespaces of all the participating types.
        /// </summary>
        /// <param name="type">
        ///     The type to get the full name of.
        /// </param>
        /// <returns>
        ///     The full name of the specified type.
        /// </returns>
        /// <example>
        ///     Let there is the following type declaration.
        ///     <code>
        ///         namespace SomeNamespace1.SomeNamespace2
        ///         {
        ///             class RootType
        ///             {
        ///                 class NestedType1&lt;TKey, TValue&gt;
        ///                 {
        ///                     class NestedType2
        ///                     {
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     </code>
        ///     Then <see cref="OmnifactotumTypeExtensions.GetFullName"/> returns a result as described below.
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Input Argument</term>
        ///             <description>Method Result</description>
        ///         </listheader>
        ///         <item>
        ///             <term>typeof(SomeNamespace1.SomeNamespace2.RootType.NestedType1&lt;,&gt;.NestedType2)</term>
        ///             <description>
        ///                 SomeNamespace1.SomeNamespace2.RootType.NestedType1&lt;TKey, TValue&gt;.NestedType2
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 typeof(SomeNamespace1.SomeNamespace2.RootType.NestedType1&lt;int, string&gt;.NestedType2)
        ///             </term>
        ///             <description>
        ///                 SomeNamespace1.SomeNamespace2.RootType.NestedType1&lt;Int32, String&gt;.NestedType2
        ///             </description>
        ///         </item>
        ///     </list>
        /// </example>
        public static string GetFullName([NotNull] this Type type)
        {
            return GetNameInternal(type, true);
        }

        /// <summary>
        ///     Determines whether the specified type is <see cref="Nullable{T}"/> for a certain type T.
        /// </summary>
        /// <param name="type">
        ///     The type to check.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified type is <see cref="Nullable{T}"/> for a certain type T;
        ///     otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        ///     <para>Determines the type of the elements in the collection represented by the specified type.</para>
        ///     <para>If the specified type is not a collection (<see cref="IEnumerable"/>) or if it is a generic type
        ///     definition, <c>null</c> is returned.</para>
        ///     <para>If the specified type is not a generic collection (<see cref="IEnumerable{T}"/>) but it is
        ///     a non-generic collection (<see cref="IEnumerable"/>), <c>typeof(<see cref="System.Object"/>)</c>
        ///     is returned.</para>
        /// </summary>
        /// <param name="type">
        ///     The type for which to get the type of the elements in the collection represented by the specified type.
        /// </param>
        /// <returns>
        ///     The type of the elements in the collection represented by the specified type.
        /// </returns>
        public static Type GetCollectionElementType([NotNull] this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.HasElementType && type.IsArray)
            {
                var defaultElementType = type.GetElementType();
                if (defaultElementType != null)
                {
                    return defaultElementType;
                }
            }

            var interfaces = type.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (!@interface.IsGenericType)
                {
                    continue;
                }

                var definition = @interface.GetGenericTypeDefinition();
                if (definition != typeof(IEnumerable<>))
                {
                    continue;
                }

                var elementType = @interface.GetGenericArguments().Single();
                if (elementType.IsGenericParameter)
                {
                    break;
                }

                return elementType;
            }

            return typeof(IEnumerable).IsAssignableFrom(type) ? typeof(object) : null;
        }

        /// <summary>
        ///     Gets the short type name.
        /// </summary>
        /// <param name="type">
        ///     The type to get the name of.
        /// </param>
        /// <returns>
        ///     The short type name.
        /// </returns>
        internal static string GetShortTypeNameInternal(Type type)
        {
            if (type.IsNullable())
            {
                var underlyingTypeName = GetShortTypeNameInternal(Nullable.GetUnderlyingType(type));
                return underlyingTypeName + "?";
            }

            if (type.HasElementType)
            {
                if (type.IsPointer)
                {
                    var elementType = type.GetElementType();
                    var elementTypeName = GetShortTypeNameInternal(elementType);
                    return elementTypeName + "*";
                }

                if (type.IsArray)
                {
                    var elementType = type.GetElementType();
                    var elementTypeName = GetShortTypeNameInternal(elementType);
                    return elementTypeName + "[]";
                }
            }

            if (type.IsGenericType || type.IsGenericTypeDefinition)
            {
                var resultBuilder = new StringBuilder();
                resultBuilder.Append(GetGenericTypeNameBase(type));
                resultBuilder.Append("<");

                var genericArguments = type.GetGenericArguments();
                for (var index = 0; index < genericArguments.Length; index++)
                {
                    if (index > 0)
                    {
                        resultBuilder.Append(", ");
                    }

                    var shortName = GetShortTypeNameInternal(genericArguments[index]);
                    resultBuilder.Append(shortName);
                }

                resultBuilder.Append(">");

                return resultBuilder.ToString();
            }

            if (type.IsNested || type.DeclaringType != null)
            {
                return type.Name;
            }

            if (string.IsNullOrEmpty(type.FullName))
            {
                return type.Name;
            }

            var result = CSharpCodeProviderInstance.GetTypeOutput(new CodeTypeReference(type));
            if (result == type.FullName)
            {
                result = type.Name;
            }

            return result;
        }

        private static void GetNameInternal(
            [NotNull] StringBuilder resultBuilder,
            [NotNull] Type type,
            bool fullName,
            [CanBeNull] List<Type> genericParameters,
            ref int genericParameterOffset)
        {
            if (resultBuilder == null)
            {
                throw new ArgumentNullException(nameof(resultBuilder));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (genericParameterOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(genericParameterOffset), genericParameterOffset, null);
            }

            if (type.IsGenericType && genericParameters == null)
            {
                genericParameters = type.GetGenericArguments().ToList();
            }

            if (type.HasElementType)
            {
                var elementType = type.GetElementType().EnsureNotNull();

                if (type.IsPointer)
                {
                    GetNameInternal(
                        resultBuilder,
                        elementType,
                        fullName,
                        genericParameters,
                        ref genericParameterOffset);

                    resultBuilder.Append('*');
                    return;
                }

                if (type.IsArray)
                {
                    GetNameInternal(
                        resultBuilder,
                        elementType,
                        fullName,
                        genericParameters,
                        ref genericParameterOffset);

                    resultBuilder.Append("[]");
                    return;
                }
            }

            if (type.DeclaringType == null)
            {
                if (fullName && !type.Namespace.IsNullOrEmpty())
                {
                    resultBuilder.Append(type.Namespace);
                    resultBuilder.Append(Type.Delimiter);
                }
            }
            else if (!type.IsGenericParameter)
            {
                GetNameInternal(
                    resultBuilder,
                    type.DeclaringType,
                    fullName,
                    genericParameters,
                    ref genericParameterOffset);
                resultBuilder.Append(Type.Delimiter);
            }

            if (!fullName && (type.IsNullable() || !type.IsGenericType && !type.IsGenericTypeDefinition))
            {
                var shortTypeName = GetShortTypeNameInternal(type);
                resultBuilder.Append(shortTypeName);
                return;
            }

            resultBuilder.Append(GetGenericTypeNameBase(type));

            if (!type.IsGenericType)
            {
                return;
            }

            var argumentCount = type.GetGenericArguments().Length - genericParameterOffset;
            if (argumentCount <= 0)
            {
                return;
            }

            if (genericParameters == null)
            {
                throw new InvalidOperationException("INTERNAL ERROR: Generic parameter list is not initialized.");
            }

            resultBuilder.Append('<');

            var genericArguments = genericParameters
                .Skip(genericParameterOffset)
                .Take(argumentCount)
                .ToArray();
            genericParameterOffset += argumentCount;
            for (var index = 0; index < argumentCount; index++)
            {
                if (index > 0)
                {
                    resultBuilder.Append(", ");
                }

                var offset = 0;
                GetNameInternal(resultBuilder, genericArguments[index], fullName, null, ref offset);
            }

            resultBuilder.Append('>');
        }

        private static string GetNameInternal(Type type, bool fullName)
        {
            var resultBuilder = new StringBuilder();
            var offset = 0;
            GetNameInternal(resultBuilder, type, fullName, null, ref offset);
            return resultBuilder.ToString();
        }

        private static string GetGenericTypeNameBase([NotNull] Type type)
        {
            var result = type.Name;

            var index = result.IndexOf(GenericArgumentDelimiter);
            if (index >= 0)
            {
                result = result.Substring(0, index);
            }

            return result;
        }
    }
}