using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumNullableEnumExtensions))]
internal sealed class OmnifactotumNullableEnumExtensionsTests
{
    [Test]
    [TestCase(null, @"The operation for the null value (type: ConsoleColor?) is not implemented.")]
    [TestCase(ConsoleColor.Magenta, @"The operation for the enumeration value ""ConsoleColor.Magenta"" is not implemented.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Multiple target frameworks.")]
    public void TestCreateEnumValueNotImplementedExceptionWhenValidArgumentIsPassedThenSucceeds(ConsoleColor? enumValue, string baseExpectedErrorMessage)
    {
#if NET5_0_OR_GREATER
        const string enumValueDetails = $"\x0020Expression: {{ {nameof(enumValue)} }}.";
#else
        var enumValueDetails = string.Empty;
#endif
        var expectedEnumValueErrorMessage = baseExpectedErrorMessage + enumValueDetails;

        Assert.That(
            () => enumValue.CreateEnumValueNotImplementedException(),
            Is.TypeOf<NotImplementedException>().With.Message.EqualTo(expectedEnumValueErrorMessage));

        var enumValueContainer = ValueContainer.Create(enumValue);

#if NET5_0_OR_GREATER
        const string enumValueContainerDetails = $"\x0020Expression: {{ {nameof(enumValueContainer)}.{nameof(enumValueContainer.Value)} }}.";
#else
        var enumValueContainerDetails = string.Empty;
#endif
        var expectedEnumValueContainerErrorMessage = baseExpectedErrorMessage + enumValueContainerDetails;

        Assert.That(
            () => enumValueContainer.Value.CreateEnumValueNotImplementedException(),
            Is.TypeOf<NotImplementedException>().With.Message.EqualTo(expectedEnumValueContainerErrorMessage));
    }

    [Test]
    [TestCase(null, @"The operation for the null value (type: ConsoleColor?) is not supported.")]
    [TestCase(ConsoleColor.Yellow, @"The operation for the enumeration value ""ConsoleColor.Yellow"" is not supported.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Multiple target frameworks.")]
    public void TestCreateEnumValueNotSupportedExceptionWhenValidArgumentIsPassedThenSucceeds(ConsoleColor? enumValue, string baseExpectedErrorMessage)
    {
#if NET5_0_OR_GREATER
        const string enumValueDetails = $"\x0020Expression: {{ {nameof(enumValue)} }}.";
#else
        var enumValueDetails = string.Empty;
#endif
        var expectedEnumValueErrorMessage = baseExpectedErrorMessage + enumValueDetails;

        Assert.That(
            () => enumValue.CreateEnumValueNotSupportedException(),
            Is.TypeOf<NotSupportedException>().With.Message.EqualTo(expectedEnumValueErrorMessage));

        var enumValueContainer = ValueContainer.Create(enumValue);

#if NET5_0_OR_GREATER
        const string enumValueContainerDetails = $"\x0020Expression: {{ {nameof(enumValueContainer)}.{nameof(enumValueContainer.Value)} }}.";
#else
        var enumValueContainerDetails = string.Empty;
#endif
        var expectedEnumValueContainerErrorMessage = baseExpectedErrorMessage + enumValueContainerDetails;

        Assert.That(
            () => enumValueContainer.Value.CreateEnumValueNotSupportedException(),
            Is.TypeOf<NotSupportedException>().With.Message.EqualTo(expectedEnumValueContainerErrorMessage));
    }
}