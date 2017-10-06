using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests
{
    internal abstract class ConstraintTestsBase
    {
        [UsedImplicitly]
        protected string DummyProperty
        {
            get;
            set;
        }

        protected ObjectValidatorContext CreateObjectValidatorContext(
            [CanBeNull] RecursiveProcessingContext<MemberData> recursiveProcessingContext = null)
        {
            return new ObjectValidatorContext(recursiveProcessingContext);
        }

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
                parameterExpression);
        }
    }
}