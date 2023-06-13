using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(ValueRangeConstraintBase<>))]
internal sealed class ValueRangeConstraintBaseOfInt32Tests
    : TypedConstraintTestsBase<ValueRangeConstraintBaseOfInt32Tests.ValueRangeConstraint, int>
{
    protected override void OnTestConstruction(ValueRangeConstraint testee)
    {
        base.OnTestConstruction(testee);

        Assert.That(() => testee.ExposedRange.Lower, Is.EqualTo(-17));
        Assert.That(() => testee.ExposedRange.Upper, Is.EqualTo(23));
    }

    protected override IEnumerable<int> GetTypedValidValues() => Enumerable.Range(-17, 23);

    protected override IEnumerable<int> GetTypedInvalidValues()
    {
        yield return int.MinValue;
        yield return -91;
        yield return -18;
        yield return 24;
        yield return 37;
        yield return int.MaxValue;
    }

    protected override string GetTypedInvalidValueErrorMessage(int invalidValue)
        => $"The value {invalidValue} is not within the valid range ({{ <-17> .:. <23> }}).";

    internal sealed class ValueRangeConstraint : ValueRangeConstraintBase<int>
    {
        public ValueRangeConstraint()
            : base(ValueRange.Create(-17, 23))
        {
            // Nothing to do
        }

        public ValueRange<int> ExposedRange => Range;

        protected override string FormatRange() => AsInvariant($"({{ <{Range.Lower}> .:. <{Range.Upper}> }})");
    }
}