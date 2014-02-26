using System.Linq.Expressions;
using Omnifactotum.Annotations;
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

        protected MemberExpression CreateTestMemberExpression()
        {
            return Expression.MakeMemberAccess(
                Expression.Parameter(GetType(), "instance"),
                Factotum.GetPropertyInfo((ConstraintTestsBase obj) => obj.DummyProperty));
        }

        protected MemberConstraintValidationContext CreateTestValidationContext()
        {
            return new MemberConstraintValidationContext(this, CreateTestMemberExpression());
        }

        #endregion
    }
}