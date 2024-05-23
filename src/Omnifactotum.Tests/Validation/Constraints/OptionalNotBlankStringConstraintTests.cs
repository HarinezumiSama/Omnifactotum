using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(OptionalNotBlankStringConstraint))]
internal sealed class OptionalNotBlankStringConstraintTests : TypedConstraintTestsBase<OptionalNotBlankStringConstraint, string?>
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return null;
        yield return "\x0020Z";
        yield return "A";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return string.Empty;
        yield return "\t\x0020\r\n";
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue) => "The string value must not be blank.";
}