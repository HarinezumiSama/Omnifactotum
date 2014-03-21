using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests
{
    public abstract class ConstraintTestsBase
    {
        #region Protected Properties

        [UsedImplicitly]
        protected string DummyProperty
        {
            get;
            set;
        }

        #endregion

        #region Protected Methods

        protected MemberConstraintValidationContext CreateTestValidationContext()
        {
            var parameterExpression = Expression.Parameter(GetType(), ObjectValidator.RootObjectParameterName);

            var expression = Expression.MakeMemberAccess(
                parameterExpression,
                Factotum.For<ConstraintTestsBase>.GetPropertyInfo(obj => obj.DummyProperty));

            return new MemberConstraintValidationContext(
                this,
                this,
                expression,
                Expression.Lambda(expression, parameterExpression));
        }

        #endregion
    }
}