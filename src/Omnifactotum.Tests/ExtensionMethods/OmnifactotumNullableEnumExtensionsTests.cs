using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumNullableEnumExtensions))]
internal sealed class OmnifactotumNullableEnumExtensionsTests
{
    [Test]
    [TestCase(null, @"The operation for the null value (type: ConsoleColor?) is not implemented.")]
    [TestCase(ConsoleColor.Magenta, @"The operation for the enumeration value ""ConsoleColor.Magenta"" is not implemented.")]
    public void TestCreateEnumValueNotImplementedExceptionWhenValidArgumentIsPassedThenSucceeds(ConsoleColor? value, string expectedExceptionMessage)
        => Assert.That(
            () => value.CreateEnumValueNotImplementedException(),
            Is.TypeOf<NotImplementedException>().With.Message.EqualTo(expectedExceptionMessage));

    [Test]
    [TestCase(null, @"The operation for the null value (type: ConsoleColor?) is not supported.")]
    [TestCase(ConsoleColor.Yellow, @"The operation for the enumeration value ""ConsoleColor.Yellow"" is not supported.")]
    public void TestCreateEnumValueNotSupportedExceptionWhenValidArgumentIsPassedThenSucceeds(ConsoleColor? value, string expectedExceptionMessage)
        => Assert.That(
            () => value.CreateEnumValueNotSupportedException(),
            Is.TypeOf<NotSupportedException>().With.Message.EqualTo(expectedExceptionMessage));
}