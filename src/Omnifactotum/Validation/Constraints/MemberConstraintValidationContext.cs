﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Represents the context of member constraint validation.
    /// </summary>
    public sealed class MemberConstraintValidationContext
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberConstraintValidationContext"/> class.
        /// </summary>
        /// <param name="root">
        ///     The root object that is being, or was, validated.
        /// </param>
        /// <param name="container">
        ///     The object containing the value that is being, or was, validated.
        /// </param>
        /// <param name="expression">
        ///     The expression describing the path to the value starting from the root object.
        /// </param>
        /// <param name="lambdaExpression">
        ///     The lambda expression describing the path to the value starting from the root object.
        /// </param>
        internal MemberConstraintValidationContext(
            [NotNull] object root,
            [NotNull] object container,
            [NotNull] Expression expression,
            [NotNull] LambdaExpression lambdaExpression)
        {
            #region Argument Check

            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (lambdaExpression == null)
            {
                throw new ArgumentNullException("lambdaExpression");
            }

            #endregion

            this.Root = root;
            this.Container = container;
            this.Expression = expression;
            this.LambdaExpression = lambdaExpression;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the root object that is being, or was, checked.
        /// </summary>
        public object Root
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the object containing the value that is being, or was, validated.
        /// </summary>
        public object Container
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the expression describing the path to the value from the root object.
        /// </summary>
        public Expression Expression
        {
            get;
            private set;
        }

        #endregion

        #region Internal Properties

        /// <summary>
        ///     Gets the lambda expression describing the path to the value from the root object.
        /// </summary>
        internal LambdaExpression LambdaExpression
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a lambda expression, using the specified parameter name, based on the expression describing
        ///     the path to the value from the root object.
        /// </summary>
        /// <param name="parameterName">
        ///     The name of the parameter to use in the lambda expression.
        /// </param>
        /// <param name="parameterExpression">
        ///     When this method returns, contains the parameter expression used in the created lambda expression.
        /// </param>
        /// <returns>
        ///     A lambda expression based on the expression describing the path to the value from the root object.
        /// </returns>
        public Expression<Func<object, object>> CreateLambdaExpression(
            string parameterName,
            out ParameterExpression parameterExpression)
        {
            #region Argument Check

            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentException(
                    @"The value can be neither empty nor whitespace-only string nor null.",
                    "parameterName");
            }

            #endregion

            var rootType = this.Root.GetTypeSafely();

            parameterExpression = Expression.Parameter(typeof(object), parameterName);
            var proxyExpression = Expression.Convert(
                Expression.Invoke(
                    this.LambdaExpression,
                    Expression.Convert(parameterExpression, rootType)),
                typeof(object));

            var result = Expression.Lambda<Func<object, object>>(proxyExpression, parameterExpression);
            return result;
        }

        /// <summary>
        ///     Creates a lambda expression, using the specified parameter name, based on the expression describing
        ///     the path to the value from the root object.
        /// </summary>
        /// <param name="parameterName">
        ///     The name of the parameter to use in the lambda expression.
        /// </param>
        /// <returns>
        ///     A lambda expression based on the expression describing the path to the value from the root object.
        /// </returns>
        public Expression<Func<object, object>> CreateLambdaExpression(string parameterName)
        {
            ParameterExpression parameterExpression;
            return CreateLambdaExpression(parameterName, out parameterExpression);
        }

        /// <summary>
        ///     Creates a lambda expression, using the default parameter name, based on the expression describing
        ///     the path to the value from the root object.
        /// </summary>
        /// <returns>
        ///     A lambda expression based on the expression describing the path to the value from the root object.
        /// </returns>
        public Expression<Func<object, object>> CreateLambdaExpression()
        {
            return CreateLambdaExpression(ObjectValidator.RootObjectParameterName);
        }

        #endregion
    }
}