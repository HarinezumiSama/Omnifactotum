using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(NullableEnumValueDefinedConstraint<>))]
internal sealed class NullableEnumValueDefinedConstraintTests : TypedConstraintTestsBase<NullableEnumValueDefinedConstraint<ConsoleColor>, ConsoleColor?>
{
    protected override IEnumerable<ConsoleColor?> GetTypedValidValues() => EnumFactotum.GetAllValues<ConsoleColor>().Select(item => (ConsoleColor?)item);

    protected override IEnumerable<ConsoleColor?> GetTypedInvalidValues()
    {
        yield return (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), -1);
        yield return (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), int.MinValue);
        yield return (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), int.MaxValue);
    }
}