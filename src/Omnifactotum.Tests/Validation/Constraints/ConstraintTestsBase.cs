using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    internal abstract class ConstraintTestsBase
    {
        [UsedImplicitly]
        protected object DummyProperty { get; set; }

        protected static ObjectValidatorContext CreateObjectValidatorContext(
            [CanBeNull] RecursiveProcessingContext<MemberData> recursiveProcessingContext = null)
            => new(recursiveProcessingContext);

        protected MemberConstraintValidationContext CreateMemberConstraintValidationContext()
        {
            var parameterExpression = Expression.Parameter(GetType(), ObjectValidator.RootObjectParameterName);

            var expression = Expression.MakeMemberAccess(
                parameterExpression,
                Factotum.For<ConstraintTestsBase>.GetPropertyInfo(obj => obj.DummyProperty));

            return new MemberConstraintValidationContext(this, this, expression, parameterExpression);
        }
    }
}