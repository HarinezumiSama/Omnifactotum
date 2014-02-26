using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Reflection.MethodBase"/> class.
    /// </summary>
    [DebuggerNonUserCode]
    public static class OmnifactotumMethodBaseExtensions
    {
        #region Constants and Fields

        /// <summary>
        ///     The type delimiter.
        /// </summary>
        private static readonly string TypeDelimiter = Type.Delimiter.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        ///     The <b>void</b> type.
        /// </summary>
        private static readonly Type VoidType = typeof(void);

        /// <summary>
        ///     The <b>void</b> type name.
        /// </summary>
        private static readonly string VoidTypeName = OmnifactotumTypeExtensions.GetShortTypeNameInternal(VoidType);

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets the full name of the specified method in the form &quot;Namespace.DeclaringType.MethodName&quot;
        /// </summary>
        /// <param name="method">
        ///     The method to get the full name of.
        /// </param>
        /// <returns>
        ///     The full name of the <paramref name="method"/>.
        /// </returns>
        public static string GetFullName(this MethodBase method)
        {
            #region Argument Check

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            #endregion

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
        public static string GetQualifiedName(this MethodBase method)
        {
            #region Argument Check

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            #endregion

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
        public static string GetSignature(this MethodBase method)
        {
            #region Argument Check

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            #endregion

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
        public static string GetFullSignature(this MethodBase method)
        {
            #region Argument Check

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            #endregion

            return GetSignatureInternal(method, true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the type containing the method.
        /// </summary>
        /// <param name="method">
        ///     The method.
        /// </param>
        /// <returns>
        ///     The type containing the method.
        /// </returns>
        private static Type GetMethodType(MethodBase method)
        {
            return method.DeclaringType ?? method.ReflectedType;
        }

        /// <summary>
        ///     Gets the signature of the method.
        /// </summary>
        /// <param name="method">
        ///     The method to get the signature of.
        /// </param>
        /// <param name="fullNames">
        ///     Specifies whether to get the full type names.
        /// </param>
        /// <returns>
        ///     The signature of the method.
        /// </returns>
        private static string GetSignatureInternal(MethodBase method, bool fullNames)
        {
            var resultBuilder = new StringBuilder();

            Func<Type, string> getTypeName =
                type => type == VoidType
                    ? VoidTypeName
                    : (fullNames ? type.GetFullName() : OmnifactotumTypeExtensions.GetShortTypeNameInternal(type));

            //// MethodInfo specific code
            {
                var methodInfo = method as MethodInfo;
                if (methodInfo != null)
                {
                    resultBuilder.Append(getTypeName(methodInfo.ReturnType));
                    resultBuilder.Append(" ");
                }
            }

            resultBuilder.Append(getTypeName(GetMethodType(method)));
            resultBuilder.Append(Type.Delimiter);
            resultBuilder.Append(method.Name);

            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length > 0)
            {
                resultBuilder.Append("<");

                for (int index = 0; index < genericArguments.Length; index++)
                {
                    if (index > 0)
                    {
                        resultBuilder.Append(", ");
                    }

                    resultBuilder.Append(getTypeName(genericArguments[index]));
                }

                resultBuilder.Append(">");
            }

            var parameters = method.GetParameters();
            resultBuilder.Append("(");

            for (int index = 0; index < parameters.Length; index++)
            {
                if (index > 0)
                {
                    resultBuilder.Append(", ");
                }

                var parameter = parameters[index];
                if (parameter.IsOut)
                {
                    resultBuilder.Append("out ");
                }
                else if (parameter.ParameterType.IsByRef)
                {
                    resultBuilder.Append("ref ");
                }

                resultBuilder.Append(getTypeName(parameter.ParameterType));
            }

            resultBuilder.Append(")");

            return resultBuilder.ToString();
        }

        /// <summary>
        ///     Gets the name of the method.
        /// </summary>
        /// <param name="method">
        ///     The method to get the name of.
        /// </param>
        /// <param name="fullName">
        ///     Specifies whether to get the full type name.
        /// </param>
        /// <returns>
        ///     The name of the method.
        /// </returns>
        private static string GetNameInternal(MethodBase method, bool fullName)
        {
            var type = GetMethodType(method);
            return (type == null ? string.Empty : (fullName ? type.FullName : type.Name) + TypeDelimiter)
                + method.Name;
        }

        #endregion
    }
}