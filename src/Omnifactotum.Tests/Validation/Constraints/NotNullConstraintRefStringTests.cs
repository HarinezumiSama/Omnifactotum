using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint.Ref<>))]
internal sealed class NotNullConstraintRefStringTests : TypedConstraintTestsBase<NotNullConstraint.Ref<string>, string>
{
    protected override IEnumerable<string> GetTypedValidValues()
    {
        yield return string.Empty;
        yield return "A";
        yield return "f0f6806923e84EEFB3A76db6121ba0b5";
    }

    protected override IEnumerable<string> GetTypedInvalidValues()
    {
        yield return null!;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string invalidValue) => "The 'string' value must not be null.";
}