using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.NUnit;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(OptionalValueRangeConstraintBase<>))]
internal sealed class OptionalValueRangeConstraintBaseOfDecimalTests
    : TypedConstraintTestsBase<OptionalValueRangeConstraintBaseOfDecimalTests.ValueRangeConstraint, decimal?>
{
    protected override void OnTestConstruction(ValueRangeConstraint testee)
    {
        base.OnTestConstruction(testee);

        Assert.That(() => testee.ExposedRange.Lower, Is.EqualTo(1.1m));
        Assert.That(() => testee.ExposedRange.Upper, Is.EqualTo(3.9m));
    }

    protected override IEnumerable<decimal?> GetTypedValidValues()
    {
        yield return null;
        yield return 1.1m;
        yield return 1.234m;
        yield return 2.345m;
        yield return 3.45678m;
        yield return 3.9m;
    }

    protected override IEnumerable<decimal?> GetTypedInvalidValues()
    {
        yield return decimal.MinValue;
        yield return 0m;
        yield return 1m;
        yield return 1.0999m;
        yield return 3.900001m;
        yield return decimal.MaxValue;
    }

    protected override string GetTypedInvalidValueErrorMessage(decimal? invalidValue)
        => AsInvariant($"The value {invalidValue.AssertNotNull()} is not within the valid range [1.1 ~ 3.9].");

    internal sealed class ValueRangeConstraint : OptionalValueRangeConstraintBase<decimal>
    {
        public ValueRangeConstraint()
            : base(1.1m, 3.9m)
        {
            // Nothing to do
        }

        public ValueRange<decimal> ExposedRange => Range;
    }
}