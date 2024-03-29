﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <see cref="System.Type"/> type.
/// </summary>
public static class OmnifactotumTypeExtensions
{
    internal const char GenericArgumentsOpening = '<';
    internal const char GenericArgumentsClosing = '>';

    /// <summary>
    ///     The generic argument delimiter.
    /// </summary>
    private const char GenericArgumentDelimiter = '`';

    internal static readonly Func<Type, string> GetFullNameMethod = GetFullName;
    internal static readonly Func<Type, string> GetQualifiedNameMethod = GetQualifiedName;

    private static readonly Dictionary<Type, string> ShortTypeNameMap = new()
    {
        { typeof(bool), @"bool" },
        { typeof(byte), @"byte" },
        { typeof(char), @"char" },
        { typeof(decimal), @"decimal" },
        { typeof(double), @"double" },
        { typeof(float), @"float" },
        { typeof(int), @"int" },
        { typeof(long), @"long" },
        { typeof(object), @"object" },
        { typeof(sbyte), @"sbyte" },
        { typeof(short), @"short" },
        { typeof(string), @"string" },
        { typeof(uint), @"uint" },
        { typeof(ulong), @"ulong" },
        { typeof(ushort), @"ushort" },
        { typeof(void), @"void" }
    };

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
    ///     A <see cref="System.IO.Stream"/> representing the manifest resource, or <see langword="null"/> if no resources
    ///     were specified during compilation or if the resource is not visible to the caller.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="type"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///     <paramref name="name"/> is <see langword="null"/> or an empty string (<see cref="String.Empty"/>).
    /// </exception>
    [CanBeNull]
    public static Stream? GetManifestResourceStream([NotNull] this Type type, [NotNull] string name)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException(@"The value can be neither empty string nor null.", nameof(name));
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
    ///                 typeof(SomeNamespace1.SomeNamespace2.RootType)
    ///             </term>
    ///             <description>RootType</description>
    ///         </item>
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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static string GetQualifiedName([NotNull] this Type type) => GetNameInternal(type, false);

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
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static string GetFullName([NotNull] this Type type) => GetNameInternal(type, true);

    /// <summary>
    ///     Determines whether the specified type is <see cref="Nullable{T}"/> for a certain type T.
    /// </summary>
    /// <param name="type">
    ///     The type to check.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the specified type is <see cref="Nullable{T}"/> for a certain type T;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static bool IsNullableValueType([NotNull] this Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        return type is { IsGenericType: true, IsGenericTypeDefinition: false } && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    ///     <para>
    ///         Determines the type of the elements in the collection represented by the specified type.
    ///     </para>
    ///     <para>
    ///         If the specified type is not a collection (<see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/>) or if it is a generic type
    ///         definition, <see langword="null"/> is returned.
    ///     </para>
    ///     <para>
    ///         If the specified type is not a generic collection (<see cref="IEnumerable{T}"/>) but it is
    ///         a non-generic collection (<see cref="IEnumerable"/>), <c>typeof(<see cref="System.Object"/>)</c>
    ///         is returned.
    ///     </para>
    /// </summary>
    /// <param name="type">
    ///     The type for which to get the type of the elements in the collection represented by the specified type.
    /// </param>
    /// <returns>
    ///     The type of the elements in the collection represented by the specified type.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [CanBeNull]
    public static Type? GetCollectionElementTypeOrDefault([NotNull] this Type type)
    {
        switch (type)
        {
            case null:
                throw new ArgumentNullException(nameof(type));

            case { HasElementType: true, IsArray: true } when type.GetElementType() is { } arrayElementType:
                return arrayElementType;

            case { IsInterface: true, IsGenericType: true, IsGenericTypeDefinition: false } when type.GetGenericTypeDefinition() == typeof(IEnumerable<>):
                return type.GetGenericArguments().Single();
        }

        var enumerableInterfaceType = type
            .GetInterfaces()
            .FirstOrDefault(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableInterfaceType?.GetGenericArguments().Single() is { IsGenericParameter: false } enumerableElementType)
        {
            return enumerableElementType;
        }

        return typeof(IEnumerable).IsAssignableFrom(type) ? typeof(object) : null;
    }

    /// <summary>
    ///     Gets the <see cref="MethodInfo"/> that implements the specified interface method in the specified type.
    /// </summary>
    /// <param name="implementationType">
    ///     The type to search the interface method implementation in.
    /// </param>
    /// <param name="interfaceMethod">
    ///     The interface method to search for.
    /// </param>
    /// <returns>
    ///     A <see cref="MethodInfo"/> that represents implementation of <paramref name="interfaceMethod"/> in <paramref name="implementationType"/>.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [NotNull]
    public static MethodInfo GetInterfaceMethodImplementation(this Type implementationType, MethodInfo interfaceMethod)
    {
        if (implementationType is null)
        {
            throw new ArgumentNullException(nameof(implementationType));
        }

        if (interfaceMethod is null)
        {
            throw new ArgumentNullException(nameof(interfaceMethod));
        }

        if (implementationType.IsInterface)
        {
            throw new ArgumentException($"The type {implementationType.GetFullName().ToUIString()} is an interface.", nameof(implementationType));
        }

        var interfaceType = interfaceMethod.DeclaringType;
        if (interfaceType is null)
        {
            throw new ArgumentException($"The method {{ {interfaceMethod.GetSignature()} }} does not belong to a type.", nameof(interfaceMethod));
        }

        if (!interfaceType.IsInterface)
        {
            throw new ArgumentException(
                $"The method {{ {interfaceMethod.GetSignature()} }} belongs to the type {interfaceType.GetFullName().ToUIString()} which is not an interface.",
                nameof(interfaceMethod));
        }

        if (!interfaceType.IsAssignableFrom(implementationType))
        {
            throw new ArgumentException(
                $"The method {{ {interfaceMethod.GetSignature()} }} belongs to the interface {
                    interfaceType.GetFullName().ToUIString()} which is not implemented by the type {implementationType.GetFullName().ToUIString()}.",
                nameof(interfaceMethod));
        }

        var interfaceMapping = implementationType.GetInterfaceMap(interfaceType);
        Factotum.Assert(interfaceMapping.InterfaceMethods.Length == interfaceMapping.TargetMethods.Length);

        var foundIndex = Array.IndexOf(interfaceMapping.InterfaceMethods, interfaceMethod);
        if (foundIndex < 0)
        {
            throw new InvalidOperationException(
                $"The method {{ {interfaceMethod.GetSignature()} }} of the interface {
                    interfaceType.GetFullName().ToUIString()} is not found in the implementing type {implementationType.GetFullName().ToUIString()}.");
        }

        return interfaceMapping.TargetMethods[foundIndex];
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
    [NotNull]
    internal static string GetShortTypeNameInternal([NotNull] Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type.IsNullableValueType())
        {
            var underlyingTypeName = GetShortTypeNameInternal(Nullable.GetUnderlyingType(type)!);
            return underlyingTypeName + "?";
        }

        if (type.HasElementType)
        {
            if (type.IsPointer)
            {
                var elementType = type.GetElementType();
                var elementTypeName = GetShortTypeNameInternal(elementType!);
                return elementTypeName + "*";
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var elementTypeName = GetShortTypeNameInternal(elementType!);
                return elementTypeName + "[]";
            }
        }

        if (type.IsGenericType || type.IsGenericTypeDefinition)
        {
            var resultBuilder = new StringBuilder();
            resultBuilder.Append(GetGenericTypeNameBase(type));
            resultBuilder.Append(GenericArgumentsOpening);

            var genericArguments = type.GetGenericArguments();
            for (var index = 0; index < genericArguments.Length; index++)
            {
                if (index > 0)
                {
                    resultBuilder.Append(",\x0020");
                }

                var shortName = GetShortTypeNameInternal(genericArguments[index]);
                resultBuilder.Append(shortName);
            }

            resultBuilder.Append(GenericArgumentsClosing);

            return resultBuilder.ToString();
        }

        if (type.IsNested || type.DeclaringType is not null)
        {
            return type.Name;
        }

        var result = ShortTypeNameMap.TryGetValue(type, out var shortTypeName) ? shortTypeName : type.Name;
        return result;
    }

    private static void GetNameInternal(
        [NotNull] StringBuilder resultBuilder,
        [NotNull] Type type,
        bool fullName,
        [CanBeNull] List<Type>? genericParameters,
        ref int genericParameterOffset)
    {
        if (genericParameterOffset < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(genericParameterOffset),
                genericParameterOffset,
                "Internal error: generic parameter offset is out of range.");
        }

        if (type.IsGenericType && genericParameters is null)
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

        if (type.DeclaringType is null)
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

        if (!fullName && (type.IsNullableValueType() || type is { IsGenericType: false, IsGenericTypeDefinition: false }))
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

        if (genericParameters is null)
        {
            throw new InvalidOperationException("INTERNAL ERROR: Generic parameter list is not initialized.");
        }

        resultBuilder.Append(GenericArgumentsOpening);

        var genericArguments = genericParameters
            .Skip(genericParameterOffset)
            .Take(argumentCount)
            .ToArray();

        genericParameterOffset += argumentCount;
        for (var index = 0; index < argumentCount; index++)
        {
            if (index > 0)
            {
                resultBuilder.Append(",\x0020");
            }

            var offset = 0;
            GetNameInternal(resultBuilder, genericArguments[index], fullName, null, ref offset);
        }

        resultBuilder.Append(GenericArgumentsClosing);
    }

    [NotNull]
    private static string GetNameInternal([NotNull] Type type, bool fullName)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var resultBuilder = new StringBuilder();
        var offset = 0;
        GetNameInternal(resultBuilder, type, fullName, null, ref offset);
        return resultBuilder.ToString();
    }

    [NotNull]
    //// ReSharper disable once SuggestBaseTypeForParameter
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