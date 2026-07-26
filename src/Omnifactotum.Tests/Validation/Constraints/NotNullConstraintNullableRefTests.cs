using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint.NullableRef<>))]
internal sealed class NotNullConstraintNullableRefTests : TypedConstraintTestsBase<NotNullConstraint.NullableRef<string>, string?>
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return string.Empty;
        yield return "A";
        yield return "2487251784294a1FA87E4CE42cfb0e59";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(string? invalidValue) => "The 'string' value must not be null.";
}