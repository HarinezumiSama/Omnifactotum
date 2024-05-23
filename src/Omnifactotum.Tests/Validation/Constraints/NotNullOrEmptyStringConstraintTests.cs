using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

#pragma warning disable CS0618 // Type or member is obsolete
[TestFixture(TestOf = typeof(NotNullOrEmptyStringConstraint))]
internal sealed class NotNullOrEmptyStringConstraintTests : TypedConstraintTestsBase<NotNullOrEmptyStringConstraint, string?>
#pragma warning restore CS0618 // Type or member is obsolete
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return "A";
        yield return "\x0020";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
        yield return string.Empty;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue) => "The value must not be null or an empty string.";
}