#nullable enable

using System.Runtime.CompilerServices;
using System.Text;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Reflection
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Reflection.MethodBase"/> class.
    /// </summary>
    public static class OmnifactotumMethodBaseExtensions
    {
        private static readonly Type VoidType = typeof(void);
        private static readonly string VoidTypeName = OmnifactotumTypeExtensions.GetShortTypeNameInternal(VoidType);

        private const char ReturnTypeSeparator = '\x0020';
        private const char ParametersOpening = '(';
        private const char ParametersClosing = ')';

        /// <summary>
        ///     Gets the full name of the specified method in the form &quot;Namespace.DeclaringType.MethodName&quot;
        /// </summary>
        /// <param name="method">
        ///     The method to get the full name of.
        /// </param>
        /// <returns>
        ///     The full name of the <paramref name="method"/>.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static string GetFullName([NotNull] this MethodBase method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return GetNameInternal(method, true);
        }

        /// <summary>
        ///     Gets the qualified name of the specified method in the form &quot;DeclaringType.MethodName&quot;
        /// </summary>
        /// <param name="method">
        ///     The method to get the qualified name of.
        /// </param>
        /// <returns>
        ///     The qualified name of the <paramref name="method"/>.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static string GetQualifiedName([NotNull] this MethodBase method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return GetNameInternal(method, false);
        }

        /// <summary>
        ///     Gets the method signature, including return type, generic type arguments, and parameter types;
        ///     types names are short at that.
        /// </summary>
        /// <param name="method">
        ///     The method to get the signature of.
        /// </param>
        /// <returns>
        ///     The string representation of the method signature.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static string GetSignature([NotNull] this MethodBase method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return GetSignatureInternal(method, false);
        }

        /// <summary>
        ///     Gets the method signature, including return type, generic type arguments, and parameter types;
        ///     types names are full at that.
        /// </summary>
        /// <param name="method">
        ///     The method to get the signature of.
        /// </param>
        /// <returns>
        ///     The string representation of the method signature.
        /// </returns>
        [Pure]
        [Omnifactotum.Annotations.Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static string GetFullSignature([NotNull] this MethodBase method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return GetSignatureInternal(method, true);
        }

        [CanBeNull]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //// ReSharper disable once SuggestBaseTypeForParameter
        private static Type? GetMethodContainingType([NotNull] MethodBase method)
            => method.DeclaringType ?? method.ReflectedType;

        [NotNull]
        private static string GetSignatureInternal([NotNull] MethodBase method, bool fullNames)
        {
            var resultBuilder = new StringBuilder();

            string GetTypeName(Type type) => GetTypeNameInternal(type, fullNames);

            if (method is MethodInfo methodInfo)
            {
                resultBuilder.Append(GetTypeName(methodInfo.ReturnType));
                resultBuilder.Append(ReturnTypeSeparator);
            }

            var methodContainingType = GetMethodContainingType(method);
            if (methodContainingType is not null)
            {
                resultBuilder.Append(GetTypeName(methodContainingType));
                resultBuilder.Append(Type.Delimiter);
            }

            resultBuilder.Append(method.Name);

            if (method.IsGenericMethod || method.IsGenericMethodDefinition)
            {
                var genericArguments = method.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    resultBuilder.Append(OmnifactotumTypeExtensions.GenericArgumentsOpening);

                    for (var index = 0; index < genericArguments.Length; index++)
                    {
                        if (index > 0)
                        {
                            resultBuilder.Append(",\x0020");
                        }

                        resultBuilder.Append(GetTypeName(genericArguments[index]));
                    }

                    resultBuilder.Append(OmnifactotumTypeExtensions.GenericArgumentsClosing);
                }
            }

            var parameters = method.GetParameters();
            resultBuilder.Append(ParametersOpening);

            for (var index = 0; index < parameters.Length; index++)
            {
                if (index > 0)
                {
                    resultBuilder.Append(",\x0020");
                }

                var parameter = parameters[index];
                var parameterType = parameter.ParameterType;

                if (parameter.IsOut)
                {
                    resultBuilder.Append("out\x0020");
                }
                else if (parameterType.IsByRef)
                {
                    resultBuilder.Append("ref\x0020");
                }

                var actualParameterType = parameterType.IsByRef && parameterType.HasElementType
                    ? parameterType.GetElementType().EnsureNotNull()
                    : parameterType;

                resultBuilder.Append(GetTypeName(actualParameterType));
            }

            resultBuilder.Append(ParametersClosing);

            return resultBuilder.ToString();
        }

        [NotNull]
        private static string GetTypeNameInternal([NotNull] Type type, bool fullName)
            //// ReSharper disable once ArrangeRedundantParentheses :: For clarity
            => type == VoidType
                ? VoidTypeName
                : (fullName ? type.GetFullName() : OmnifactotumTypeExtensions.GetShortTypeNameInternal(type));

        [NotNull]
        private static string GetNameInternal([NotNull] MethodBase method, bool fullName)
        {
            var resultBuilder = new StringBuilder();

            var type = GetMethodContainingType(method);
            if (type != null)
            {
                var typeName = fullName ? type.GetFullName() : type.GetQualifiedName();
                resultBuilder.Append(typeName);
                resultBuilder.Append(Type.Delimiter);
            }

            resultBuilder.Append(method.Name);

            //// ReSharper disable once InvertIf
            if (method.IsGenericMethod || method.IsGenericMethodDefinition)
            {
                var genericArguments = method.GetGenericArguments();

                //// ReSharper disable once InvertIf
                if (genericArguments.Length > 0)
                {
                    resultBuilder.Append(OmnifactotumTypeExtensions.GenericArgumentsOpening);

                    for (var index = 0; index < genericArguments.Length; index++)
                    {
                        if (index > 0)
                        {
                            resultBuilder.Append(",\x0020");
                        }

                        resultBuilder.Append(GetTypeNameInternal(genericArguments[index], fullName));
                    }

                    resultBuilder.Append(OmnifactotumTypeExtensions.GenericArgumentsClosing);
                }
            }

            return resultBuilder.ToString();
        }
    }
}