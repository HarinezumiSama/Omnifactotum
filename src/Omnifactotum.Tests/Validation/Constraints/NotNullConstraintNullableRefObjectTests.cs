using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint.NullableRef<>))]
internal sealed class NotNullConstraintNullableRefObjectTests : NotNullConstraintRefObjectBaseTests<NotNullConstraint.NullableRef<object>, object?>
{
    protected override IEnumerable<object?> GetTypedValidValues() => InternalGetTypedValidValues();

    protected override IEnumerable<object?> GetTypedInvalidValues() => InternalGetTypedInvalidValues();
}