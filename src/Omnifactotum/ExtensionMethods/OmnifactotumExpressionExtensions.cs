using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    ///     Contains extension methods for the classes in the <b>System.Linq.Expressions</b> namespace.
    /// </summary>
    public static class OmnifactotumExpressionExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the last method called in the expression.
        /// </summary>
        /// <typeparam name="TDelegate">
        ///     The type of the delegate.
        /// </typeparam>
        /// <param name="expression">
        ///     The expression to get the last called method of.
        /// </param>
        /// <returns>
        ///     The last called method, or <b>null</b> if the last element in the expression is not a method call.
        /// </returns>
        public static MethodInfo GetLastMethod<TDelegate>(this Expression<TDelegate> expression)
        {
            #region Argument Check

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            #endregion

            var methodCallExpression = expression.Body as MethodCallExpression;
            return methodCallExpression == null ? null : methodCallExpression.Method.EnsureNotNull();
        }

        #endregion
    }
}