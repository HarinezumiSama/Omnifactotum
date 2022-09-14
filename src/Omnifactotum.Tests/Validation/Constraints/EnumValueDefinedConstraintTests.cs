using System;
using System.Collections.Generic;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

[TestFixture(TestOf = typeof(EnumValueDefinedConstraint<>))]
internal sealed class EnumValueDefinedConstraintTests : TypedConstraintTestsBase<EnumValueDefinedConstraint<ConsoleColor>, ConsoleColor>
{
    protected override IEnumerable<ConsoleColor> GetTypedValidValues() => EnumFactotum.GetAllValues<ConsoleColor>();

    protected override IEnumerable<ConsoleColor> GetTypedInvalidValues()
    {
        yield return (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), -1);
        yield return (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), int.MinValue);
        yield return (ConsoleColor)Enum.ToObject(typeof(ConsoleColor), int.MaxValue);
    }
}