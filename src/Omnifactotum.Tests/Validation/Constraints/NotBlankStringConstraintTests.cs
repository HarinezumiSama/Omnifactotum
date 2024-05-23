using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

#pragma warning disable CS0618 // Type or member is obsolete
[TestFixture(TestOf = typeof(NotBlankStringConstraint))]
internal sealed class NotBlankStringConstraintTests : TypedConstraintTestsBase<NotBlankStringConstraint, string?>
#pragma warning restore CS0618 // Type or member is obsolete
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return "\x0020Z";
        yield return "A";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
        yield return string.Empty;
        yield return "\t\x0020\r\n";
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue) => "The string value must not be null or blank.";
}