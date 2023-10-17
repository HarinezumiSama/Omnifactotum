using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.NUnit;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(OptionalEnumValueDefinedConstraint<>))]
internal sealed class OptionalEnumValueDefinedConstraintTests : TypedConstraintTestsBase<OptionalEnumValueDefinedConstraint<ConsoleColor>, ConsoleColor?>
{
    protected override IEnumerable<ConsoleColor?> GetTypedValidValues()
        => EnumFactotum.GetAllValues<ConsoleColor>().Select(item => (ConsoleColor?)item).Prepend(null);

    protected override IEnumerable<ConsoleColor?> GetTypedInvalidValues()
    {
        yield return (ConsoleColor)(-1);
        yield return (ConsoleColor)int.MinValue;
        yield return (ConsoleColor)int.MaxValue;
    }

    protected override string GetTypedInvalidValueErrorMessage(ConsoleColor? invalidValue)
        => AsInvariant($@"The value {(int)invalidValue.AssertNotNull()} is not defined in the enumeration ""System.ConsoleColor"".");
}