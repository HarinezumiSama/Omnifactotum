using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint<>))]
internal sealed class TypedNotNullConstraintTests : TypedConstraintTestsBase<NotNullConstraint<string>, string?>
{
    protected override IEnumerable<string?> GetTypedValidValues()
    {
        yield return "A";
    }

    protected override IEnumerable<string?> GetTypedInvalidValues()
    {
        yield return null;
    }
}