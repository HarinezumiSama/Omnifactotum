using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NotNullConstraint.NullableValue<>))]
internal sealed class NotNullConstraintNullableValueInt32Tests : TypedConstraintTestsBase<NotNullConstraint.NullableValue<int>, int?>
{
    protected override IEnumerable<int?> GetTypedValidValues()
    {
        yield return int.MinValue;
        yield return -13;
        yield return 0;
        yield return 42;
        yield return int.MaxValue;
    }

    protected override IEnumerable<int?> GetTypedInvalidValues()
    {
        yield return null;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(int? invalidValue) => "The 'int?' value must not be null.";
}