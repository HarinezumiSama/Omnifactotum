using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint.Ref<>))]
internal sealed class NotNullConstraintRefObjectTests : NotNullConstraintRefObjectBaseTests<NotNullConstraint.Ref<object>, object>
{
    protected override IEnumerable<object> GetTypedValidValues() => InternalGetTypedValidValues();

    protected override IEnumerable<object> GetTypedInvalidValues() => InternalGetTypedInvalidValues();
}
