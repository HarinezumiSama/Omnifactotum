using System;
using System.Linq;
using System.Linq.Expressions;

namespace Omnifactotum.Validation
{
    internal static class ValidationFactotum
    {
        #region Public Methods

        public static Expression ConvertTypeAuto(Expression expression, Type valueType)
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

        public static Expression ConvertTypeAuto(Expression expression, object value)
        {
            return value == null ? expression : ConvertTypeAuto(expression, value.GetType());
        }

        #endregion
    }
}