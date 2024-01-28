using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Tests.Validation.Constraints;

internal abstract class ConstraintTestsBase
{
    [UsedImplicitly]
    protected object? DummyProperty { get; set; }

    protected MemberConstraintValidationContext CreateMemberConstraintValidationContext(
        RecursiveProcessingContext<MemberData>? recursiveProcessingContext = null)
    {
        var parameterExpression = Expression.Parameter(GetType(), ObjectValidator.DefaultRootObjectParameterName);

        var expression = Expression.MakeMemberAccess(
            parameterExpression,
            Factotum.For<ConstraintTestsBase>.GetPropertyInfo(obj => obj.DummyProperty));

        return new MemberConstraintValidationContext(new ObjectValidatorContext(recursiveProcessingContext), this, this, expression, parameterExpression);
    }
}