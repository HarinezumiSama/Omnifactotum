using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Type"/> type.
    /// </summary>
    public static class OmnifactotumTypeExtensions
    {
        #region Constants and Fields

        /// <summary>
        ///     The generic argument delimiter.
        /// </summary>
        private const char GenericArgumentDelimiter = '`';

        /// <summary>
        ///     A reference to <see cref="CSharpCodeProvider"/> instance.
        /// </summary>
        private static readonly CSharpCodeProvider CSharpCodeProviderInstance = new CSharpCodeProvider();

        #endregion

        #region Public Methods

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
        ///     A <see cref="System.IO.Stream"/> representing the manifest resource, or <b>null</b> if no resources
        ///     were specified during compilation or if the resource is not visible to the caller.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="type"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     <paramref name="name"/> is <b>null</b> or an empty string (<see cref="String.Empty"/>).
        /// </exception>
        public static Stream GetManifestResourceStream(this Type type, string name)
        {
            #region Argument Check

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The value can be neither empty string nor null.", "name");
            }

            #endregion

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
        public static string GetQualifiedName(this Type type)
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
        public static string GetFullName(this Type type)
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
        ///     <b>true</b> if the specified type is <see cref="Nullable{T}"/> for a certain type T;
        ///     otherwise, <b>false</b>.
        /// </returns>
        public static bool IsNullable(this Type type)
        {
            #region Argument Check

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            #endregion

            return type.IsGenericType
                && !type.IsGenericTypeDefinition
                && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        ///     <para>Determines the type of the elements in the collection represented by the specified type.</para>
        ///     <para>If the specified type is not a collection (<see cref="IEnumerable"/>) or if it is a generic type
        ///     definition, <b>null</b> is returned.</para>
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
        public static Type GetCollectionElementType(this Type type)
        {
            #region Argument Check

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            #endregion

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

        #endregion

        #region Internal Methods

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

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the name of the specified type.
        /// </summary>
        /// <param name="resultBuilder">
        ///     The resulting <see cref="StringBuilder"/>.
        /// </param>
        /// <param name="type">
        ///     The type to process.
        /// </param>
        /// <param name="fullName">
        ///     Specifies whether to get the full name.
        /// </param>
        /// <param name="genericParameters">
        ///     The list of the generic parameters.
        /// </param>
        /// <param name="genericParameterOffset">
        ///     The generic parameter offset. This value is passed by reference.
        /// </param>
        private static void GetNameInternal(
            StringBuilder resultBuilder,
            Type type,
            bool fullName,
            List<Type> genericParameters,
            ref int genericParameterOffset)
        {
            #region Argument Check

            if (resultBuilder == null)
            {
                throw new ArgumentNullException("resultBuilder");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (genericParameterOffset < 0)
            {
                throw new ArgumentOutOfRangeException("genericParameterOffset", genericParameterOffset, null);
            }

            #endregion

            if (type.IsGenericType && (genericParameters == null))
            {
                genericParameters = type.GetGenericArguments().ToList();
            }

            if (type.DeclaringType == null)
            {
                if (fullName)
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

            //// Generic type specific
            {
                var name = type.Name;
                var index = name.IndexOf(GenericArgumentDelimiter);
                if (index >= 0)
                {
                    name = name.Substring(0, index);
                }

                resultBuilder.Append(name);
            }

            if (type.IsGenericType)
            {
                var argumentCount = type.GetGenericArguments().Length - genericParameterOffset;
                if (argumentCount > 0)
                {
                    if (genericParameters == null)
                    {
                        throw new InvalidOperationException();
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
            }
        }

        /// <summary>
        ///     Gets the name of the specified type.
        /// </summary>
        /// <param name="type">
        ///     The type to process.
        /// </param>
        /// <param name="fullName">
        ///     Specifies whether to get the full name.
        /// </param>
        /// <returns>
        ///     The name of the specified type.
        /// </returns>
        private static string GetNameInternal(Type type, bool fullName)
        {
            var resultBuilder = new StringBuilder();
            var offset = 0;
            GetNameInternal(resultBuilder, type, fullName, null, ref offset);
            return resultBuilder.ToString();
        }

        #endregion
    }
}