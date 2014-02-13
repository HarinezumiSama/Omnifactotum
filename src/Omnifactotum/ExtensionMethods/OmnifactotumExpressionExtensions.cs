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

        public static MethodInfo GetMethod<TDelegate>(this Expression<TDelegate> expression)
        {
            #region Argument Check

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            #endregion

            return (expression.Body as MethodCallExpression).EnsureNotNull().Method.EnsureNotNull();
        }

        #endregion
    }
}