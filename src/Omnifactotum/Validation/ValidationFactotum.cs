using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    internal static class ValidationFactotum
    {
        #region Constants and Fields

        private static readonly Type CompatibleMemberConstraintType = typeof(IMemberConstraint);

        #endregion

        #region Public Methods

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

        public static Expression ConvertTypeAuto([NotNull] Expression expression, object value)
        {
            return value == null ? expression : ConvertTypeAuto(expression, value.GetType());
        }

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