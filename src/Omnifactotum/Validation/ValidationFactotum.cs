using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the internal helper for the <see cref="ObjectValidator"/>.
    /// </summary>
    internal static class ValidationFactotum
    {
        #region Constants and Fields

        private static readonly Type CompatibleMemberConstraintType = typeof(IMemberConstraint);

        #endregion

        #region Public Methods

        /// <summary>
        ///     Converts the type of the expression, if needed.
        /// </summary>
        /// <param name="expression">
        ///     The expression to convert.
        /// </param>
        /// <param name="valueType">
        ///     The type of the value.
        /// </param>
        /// <returns>
        ///     An original expression, if conversion was not needed; otherwise, a converted expression.
        /// </returns>
        public static Expression ConvertTypeAuto([NotNull] Expression expression, [NotNull] Type valueType)
        {
            #region Argument Check

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (valueType == null)
            {
                throw new ArgumentNullException("valueType");
            }

            #endregion

            var expressionType = expression.Type.GetCollectionElementType() ?? expression.Type;

            var result = expressionType == valueType ? expression : Expression.Convert(expression, valueType);
            return result;
        }

        /// <summary>
        ///     Converts the type of the expression, if needed.
        /// </summary>
        /// <param name="expression">
        ///     The expression to convert.
        /// </param>
        /// <param name="value">
        ///     The value to which type to convert the expression.
        /// </param>
        /// <returns>
        ///     An original expression, if conversion was not needed; otherwise, a converted expression.
        /// </returns>
        public static Expression ConvertTypeAuto([NotNull] Expression expression, object value)
        {
            return value == null ? expression : ConvertTypeAuto(expression, value.GetType());
        }

        /// <summary>
        ///     Determines whether the specified constraint type is a valid member constraint type.
        /// </summary>
        /// <param name="constraintType">
        ///     The type of the constraint to check.
        /// </param>
        /// <returns>
        ///     <b>true</b> if specified constraint type is a valid member constraint type; otherwise, <b>false</b>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintType"/> is <b>null</b>.
        /// </exception>
        public static bool IsValidMemberConstraintType([NotNull] this Type constraintType)
        {
            #region Argument Check

            if (constraintType == null)
            {
                throw new ArgumentNullException("constraintType");
            }

            #endregion

            return !constraintType.IsInterface && CompatibleMemberConstraintType.IsAssignableFrom(constraintType);
        }

        /// <summary>
        ///     Ensures that the specified constraint type is a valid member constraint type.
        /// </summary>
        /// <param name="constraintType">
        ///     The type of the constraint to check.
        /// </param>
        /// <returns>
        ///     The input constraint type, if it is a valid member constraint type. 
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintType"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     The specified constraint type is not a valid member constraint type.
        /// </exception>
        public static Type EnsureValidMemberConstraintType([NotNull] this Type constraintType)
        {
            if (IsValidMemberConstraintType(constraintType))
            {
                return constraintType;
            }

            var message = string.Format(
                CultureInfo.InvariantCulture,
                @"The specified type '{0}' is not a valid constraint type (must implement '{1}').",
                constraintType.GetFullName(),
                CompatibleMemberConstraintType.GetFullName());

            throw new ArgumentException(message, "constraintType");
        }

        #endregion
    }
}