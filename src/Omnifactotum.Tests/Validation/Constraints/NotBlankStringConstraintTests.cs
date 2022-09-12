using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotBlankStringConstraint))]
internal sealed class NotBlankStringConstraintTests : TypedConstraintTestsBase<NotBlankStringConstraint, string?>
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
}