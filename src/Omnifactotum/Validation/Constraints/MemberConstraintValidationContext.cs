using System;
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
        /// <param name="rootParameterExpression">
        ///     The root parameter expression.
        /// </param>
        internal MemberConstraintValidationContext(
            [NotNull] object root,
            [NotNull] object container,
            [NotNull] Expression expression,
            [NotNull] ParameterExpression rootParameterExpression)
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

            if (rootParameterExpression == null)
            {
                throw new ArgumentNullException("rootParameterExpression");
            }

            #endregion

            this.Root = root;
            this.Container = container;
            this.Expression = expression;
            this.RootParameterExpression = rootParameterExpression;
            this.LambdaExpression = Expression.Lambda(expression, rootParameterExpression);
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

        /// <summary>
        ///     Gets the root parameter expression.
        /// </summary>
        internal ParameterExpression RootParameterExpression
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

            var combined = Expression
                .Lambda(Expression.Convert(parameterExpression, rootType), parameterExpression)
                .InjectInto(this.LambdaExpression);

            var temporaryParameterExpression = Expression.Parameter(combined.ReturnType, "x");

            var result = combined.InjectInto(
                Expression.Lambda(
                    Expression.Convert(temporaryParameterExpression, typeof(object)),
                    temporaryParameterExpression));

            return (Expression<Func<object, object>>)result;
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
        /// <param name="parameterExpression">
        ///     When this method returns, contains the parameter expression used in the created lambda expression.
        /// </param>
        /// <returns>
        ///     A lambda expression based on the expression describing the path to the value from the root object.
        /// </returns>
        public Expression<Func<object, object>> CreateLambdaExpression(out ParameterExpression parameterExpression)
        {
            return CreateLambdaExpression(ObjectValidator.RootObjectParameterName, out parameterExpression);
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