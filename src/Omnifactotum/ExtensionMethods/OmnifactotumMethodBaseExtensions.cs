using System.Text;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace

namespace System.Reflection
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Reflection.MethodBase"/> class.
    /// </summary>
    public static class OmnifactotumMethodBaseExtensions
    {
        private static readonly Type VoidType = typeof(void);

        private static readonly string VoidTypeName = OmnifactotumTypeExtensions.GetShortTypeNameInternal(VoidType);

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
        public static string GetFullSignature([NotNull] this MethodBase method)
        {
            if (method is null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            return GetSignatureInternal(method, true);
        }

        private static Type GetMethodType([NotNull] MethodBase method) => method.DeclaringType ?? method.ReflectedType;

        private static string GetSignatureInternal([NotNull] MethodBase method, bool fullNames)
        {
            var resultBuilder = new StringBuilder();

            string GetTypeName(Type type) => GetTypeNameInternal(type, fullNames);

            if (method is MethodInfo methodInfo)
            {
                resultBuilder.Append(GetTypeName(methodInfo.ReturnType));
                resultBuilder.Append(" ");
            }

            resultBuilder.Append(GetTypeName(GetMethodType(method)));
            resultBuilder.Append(Type.Delimiter);
            resultBuilder.Append(method.Name);

            if (method.IsGenericMethod || method.IsGenericMethodDefinition)
            {
                var genericArguments = method.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    resultBuilder.Append("<");

                    for (var index = 0; index < genericArguments.Length; index++)
                    {
                        if (index > 0)
                        {
                            resultBuilder.Append(", ");
                        }

                        resultBuilder.Append(GetTypeName(genericArguments[index]));
                    }

                    resultBuilder.Append(">");
                }
            }

            var parameters = method.GetParameters();
            resultBuilder.Append("(");

            for (var index = 0; index < parameters.Length; index++)
            {
                if (index > 0)
                {
                    resultBuilder.Append(", ");
                }

                var parameter = parameters[index];
                var parameterType = parameter.ParameterType;

                if (parameter.IsOut)
                {
                    resultBuilder.Append("out ");
                }
                else if (parameterType.IsByRef)
                {
                    resultBuilder.Append("ref ");
                }

                var actualParameterType = parameterType.IsByRef && parameterType.HasElementType
                    ? parameterType.GetElementType()
                    : parameterType;

                resultBuilder.Append(GetTypeName(actualParameterType));
            }

            resultBuilder.Append(")");

            return resultBuilder.ToString();
        }

        private static string GetTypeNameInternal([NotNull] Type type, bool fullName)
            //// ReSharper disable once ArrangeRedundantParentheses :: For clarity
            => type == VoidType
                ? VoidTypeName
                : (fullName ? type.GetFullName() : OmnifactotumTypeExtensions.GetShortTypeNameInternal(type));

        private static string GetNameInternal([NotNull] MethodBase method, bool fullName)
        {
            var resultBuilder = new StringBuilder();

            var type = GetMethodType(method);
            if (type != null)
            {
                var typeName = fullName ? type.GetFullName() : type.GetQualifiedName();
                resultBuilder.Append(typeName);
                resultBuilder.Append(Type.Delimiter);
            }

            resultBuilder.Append(method.Name);

            if (method.IsGenericMethod || method.IsGenericMethodDefinition)
            {
                var genericArguments = method.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    resultBuilder.Append("<");

                    for (var index = 0; index < genericArguments.Length; index++)
                    {
                        if (index > 0)
                        {
                            resultBuilder.Append(", ");
                        }

                        resultBuilder.Append(GetTypeNameInternal(genericArguments[index], fullName));
                    }

                    resultBuilder.Append(">");
                }
            }

            return resultBuilder.ToString();
        }
    }
}