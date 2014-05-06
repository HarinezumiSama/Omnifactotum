using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Omnifactotum.Annotations;

//// ReSharper disable CheckNamespace - Namespace is intentionally root and not default to simplify usage in other solutions

namespace System.Linq.Expressions
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

        /// <summary>
        ///     Injects the specified source lambda expression instead of the sole parameter of the target expression.
        /// </summary>
        /// <typeparam name="TInput">
        ///     The type of the sole input parameter of the source lambda expression.
        /// </typeparam>
        /// <typeparam name="TIntermediate">
        ///     The type of the result of the source lambda expression
        ///     (as well as the type of the sole input parameter of the target lambda expression).
        /// </typeparam>
        /// <typeparam name="TResult">
        ///     The type of the result of the target lambda expression.
        /// </typeparam>
        /// <param name="sourceExpression">
        ///     The lambda expression to inject into the target lambda expression.
        /// </param>
        /// <param name="targetExpression">
        ///     The lambda expression to inject the source lambda expression into.
        /// </param>
        /// <returns>
        ///     A new lambda expression.
        /// </returns>
        [NotNull]
        public static Expression<Func<TInput, TResult>> InjectInto<TInput, TIntermediate, TResult>(
            [NotNull] this Expression<Func<TInput, TIntermediate>> sourceExpression,
            [NotNull] Expression<Func<TIntermediate, TResult>> targetExpression)
        {
            #region Argument Check

            if (sourceExpression == null)
            {
                throw new ArgumentNullException("sourceExpression");
            }

            if (targetExpression == null)
            {
                throw new ArgumentNullException("targetExpression");
            }

            #endregion

            var parameterExpression = sourceExpression.Parameters.Single();

            var newSecondBody =
                new ReplaceExpressionVisitor(targetExpression.Parameters.Single(), sourceExpression.Body)
                    .Visit(targetExpression.Body);

            var result = Expression.Lambda<Func<TInput, TResult>>(newSecondBody, parameterExpression);
            return result;
        }

        /// <summary>
        ///     Injects the specified source lambda expression instead of the sole parameter of the target expression.
        /// </summary>
        /// <param name="sourceExpression">
        ///     The lambda expression to inject into the target lambda expression.
        /// </param>
        /// <param name="targetExpression">
        ///     The lambda expression to inject the source lambda expression into.
        /// </param>
        /// <returns>
        ///     A new lambda expression.
        /// </returns>
        [NotNull]
        public static LambdaExpression InjectInto(
            [NotNull] this LambdaExpression sourceExpression,
            [NotNull] LambdaExpression targetExpression)
        {
            #region Argument Check

            if (sourceExpression == null)
            {
                throw new ArgumentNullException("sourceExpression");
            }

            if (sourceExpression.Parameters.Count != 1)
            {
                throw new ArgumentException("The source expression must have a single parameter.", "sourceExpression");
            }

            if (targetExpression == null)
            {
                throw new ArgumentNullException("targetExpression");
            }

            if (targetExpression.Parameters.Count != 1)
            {
                throw new ArgumentException("The target expression must have a single parameter.", "targetExpression");
            }

            if (sourceExpression.ReturnType != targetExpression.Parameters.Single().Type)
            {
                throw new ArgumentException(
                    "The type of the result of the source expression does not match the type of the sole input parameter of the target expression.");
            }

            #endregion

            var parameterExpression = sourceExpression.Parameters.Single();

            var newSecondBody =
                new ReplaceExpressionVisitor(targetExpression.Parameters.Single(), sourceExpression.Body)
                    .Visit(targetExpression.Body);

            var funcType = Expression.GetFuncType(parameterExpression.Type, targetExpression.ReturnType);

            var result = Expression.Lambda(funcType, newSecondBody, parameterExpression);
            return result;
        }

        #endregion

        #region ReplaceExpressionVisitor Class

        private sealed class ReplaceExpressionVisitor : ExpressionVisitor
        {
            #region Constants and Fields

            private readonly Expression _from;
            private readonly Expression _to;

            #endregion

            #region Constructors

            public ReplaceExpressionVisitor(Expression from, Expression to)
            {
                #region Argument Check

                if (from == null)
                {
                    throw new ArgumentNullException("from");
                }

                if (to == null)
                {
                    throw new ArgumentNullException("to");
                }

                #endregion

                _from = from;
                _to = to;
            }

            #endregion

            #region Public Methods

            public override Expression Visit(Expression node)
            {
                return node == _from ? _to : base.Visit(node);
            }

            #endregion
        }

        #endregion
    }
}