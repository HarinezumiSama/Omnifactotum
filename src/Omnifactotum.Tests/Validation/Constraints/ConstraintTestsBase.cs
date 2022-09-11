using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Tests.Validation.Constraints
{
    internal abstract class ConstraintTestsBase
    {
        [UsedImplicitly]
        protected object? DummyProperty { get; set; }

        [NotNull]
        protected static ObjectValidatorContext CreateObjectValidatorContext(
            [CanBeNull] RecursiveProcessingContext<MemberData>? recursiveProcessingContext = null)
            => new(recursiveProcessingContext);

        [NotNull]
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