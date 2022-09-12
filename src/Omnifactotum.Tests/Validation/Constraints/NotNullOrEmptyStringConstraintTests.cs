using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullOrEmptyStringConstraint))]
internal sealed class NotNullOrEmptyStringConstraintTests : TypedConstraintTestsBase<NotNullOrEmptyStringConstraint, string?>
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
}