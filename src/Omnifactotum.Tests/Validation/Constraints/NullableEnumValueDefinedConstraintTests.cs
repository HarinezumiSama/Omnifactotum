using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NullableEnumValueDefinedConstraint<>))]
internal sealed class NullableEnumValueDefinedConstraintTests : TypedConstraintTestsBase<NullableEnumValueDefinedConstraint<ConsoleColor>, ConsoleColor?>
{
    protected override IEnumerable<ConsoleColor?> GetTypedValidValues() => EnumFactotum.GetAllValues<ConsoleColor>().Select(item => (ConsoleColor?)item);

    protected override IEnumerable<ConsoleColor?> GetTypedInvalidValues()
    {
        yield return null;
        yield return (ConsoleColor)(-1);
        yield return (ConsoleColor)int.MinValue;
        yield return (ConsoleColor)int.MaxValue;
    }

    protected override ValidationErrorDetails GetTypedInvalidValueErrorDetails(ConsoleColor? invalidValue)
        => invalidValue is null
            ? "The value cannot be null."
            : AsInvariant($"The value {(int)invalidValue} is not defined in the enumeration 'ConsoleColor'.");
}