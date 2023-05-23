using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumEnumExtensions))]
internal sealed class OmnifactotumEnumExtensionsTests
{
    [Test]
    [TestCase(FileAccess.Read)]
    [TestCase(ConsoleModifiers.Alt)]
    [TestCase(ConsoleColor.Green)]
    [TestCase(TestEnumeration.NoZeroValue)]
    [TestCase(UIntTestFlags.Flag2)]
    public void TestEnsureDefinedWhenDefinedEnumerationValueArgumentIsPassedThenSucceeds<TEnum>(TEnum enumValue)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.EnsureDefined(), Is.EqualTo(enumValue));

    [Test]
    [TestCase(
        (ConsoleColor)(-1),
        @"The value -1 is not defined in the enumeration ""System.ConsoleColor"".")]
    [TestCase(
        (TestEnumeration)0,
        @"The value 0 is not defined in the enumeration ""Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.TestEnumeration"".")]
    [TestCase(
        UIntTestFlags.Flag1 | UIntTestFlags.Flag2,
        @"The value 3 is not defined in the enumeration ""Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags"".")]
    [TestCase(
        (UIntTestFlags)1024,
        @"The value 1024 is not defined in the enumeration ""Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags"".")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Multiple target frameworks.")]
    public void TestEnsureDefinedWhenUndefinedEnumerationValueArgumentIsPassedThenThrows<TEnum>(
        TEnum enumValue,
        string baseExpectedErrorMessage)
        where TEnum : struct, Enum
    {
#if NET5_0_OR_GREATER
        const string enumValueDetails = $"\x0020Expression: {{ {nameof(enumValue)} }}.";
#else
        var enumValueDetails = string.Empty;
#endif
        var expectedEnumValueErrorMessage = baseExpectedErrorMessage + enumValueDetails;
        Assert.That(() => enumValue.EnsureDefined(), Throws.TypeOf<InvalidEnumArgumentException>().With.Message.EqualTo(expectedEnumValueErrorMessage));

        var enumValueContainer = ValueContainer.Create(enumValue);

#if NET5_0_OR_GREATER
        const string enumValueContainerDetails = $"\x0020Expression: {{ {nameof(enumValueContainer)}.{nameof(enumValueContainer.Value)} }}.";
#else
        var enumValueContainerDetails = string.Empty;
#endif
        var expectedEnumValueContainerErrorMessage = baseExpectedErrorMessage + enumValueContainerDetails;

        Assert.That(
            () => enumValueContainer.Value.EnsureDefined(),
            Throws.TypeOf<InvalidEnumArgumentException>().With.Message.EqualTo(expectedEnumValueContainerErrorMessage));
    }

    [Test]
    [TestCase(FileAccess.Read, true)]
    [TestCase(ConsoleModifiers.Alt, true)]
    [TestCase(ConsoleColor.Green, true)]
    [TestCase(TestEnumeration.NoZeroValue, true)]
    [TestCase(UIntTestFlags.Flag2, true)]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control, false)]
    [TestCase((ConsoleColor)(-1), false)]
    [TestCase((TestEnumeration)0, false)]
    [TestCase(UIntTestFlags.Flag1 | UIntTestFlags.Flag2, false)]
    [TestCase((UIntTestFlags)1024, false)]
    public void TestIsDefinedWhenEnumerationValueArgumentIsPassedThenSucceeds<TEnum>(TEnum enumValue, bool expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.IsDefined(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(FileAccess.Read, "Read")]
    [TestCase(ConsoleModifiers.Alt, "Alt")]
    [TestCase(ConsoleColor.Green, "Green")]
    [TestCase(TestEnumeration.NoZeroValue, "NoZeroValue")]
    [TestCase(UIntTestFlags.Flag2, "Flag2")]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control, "Alt, Shift, Control")]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024, "1031")]
    [TestCase((ConsoleColor)(-1), "-1")]
    [TestCase((TestEnumeration)0, "0")]
    [TestCase((UIntTestFlags)0, "0")]
    [TestCase(UIntTestFlags.Flag1 | UIntTestFlags.Flag2, "Flag1, Flag2")]
    [TestCase((UIntTestFlags)1024, "1024")]
    public void TestGetNameWhenEnumerationValueArgumentIsPassedThenSucceeds<TEnum>(TEnum enumValue, string expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.GetName(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(FileAccess.Read, "FileAccess.Read")]
    [TestCase(ConsoleModifiers.Alt, "ConsoleModifiers.Alt")]
    [TestCase(ConsoleColor.Green, "ConsoleColor.Green")]
    [TestCase(TestEnumeration.NoZeroValue, "TestEnumeration.NoZeroValue")]
    [TestCase(UIntTestFlags.Flag2, "UIntTestFlags.Flag2")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
        "ConsoleModifiers.Alt, ConsoleModifiers.Shift, ConsoleModifiers.Control")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
        "ConsoleModifiers.1031")]
    [TestCase((ConsoleColor)(-1), "ConsoleColor.-1")]
    [TestCase((TestEnumeration)0, "TestEnumeration.0")]
    [TestCase((UIntTestFlags)0, "UIntTestFlags.0")]
    [TestCase(UIntTestFlags.Flag1 | UIntTestFlags.Flag2, "UIntTestFlags.Flag1, UIntTestFlags.Flag2")]
    [TestCase((UIntTestFlags)1024, "UIntTestFlags.1024")]
    public void TestGetQualifiedNameWhenEnumerationValueArgumentIsPassedThenSucceeds<TEnum>(TEnum enumValue, string expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.GetQualifiedName(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(FileAccess.Read, "System.IO.FileAccess.Read")]
    [TestCase(ConsoleModifiers.Alt, "System.ConsoleModifiers.Alt")]
    [TestCase(ConsoleColor.Green, "System.ConsoleColor.Green")]
    [TestCase(TestEnumeration.NoZeroValue, "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.TestEnumeration.NoZeroValue")]
    [TestCase(UIntTestFlags.Flag2, "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags.Flag2")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
        "System.ConsoleModifiers.Alt, System.ConsoleModifiers.Shift, System.ConsoleModifiers.Control")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
        "System.ConsoleModifiers.1031")]
    [TestCase((ConsoleColor)(-1), nameof(System) + "." + nameof(ConsoleColor) + "." + "-1")]
    [TestCase(
        (TestEnumeration)0,
        "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.TestEnumeration.0")]
    [TestCase(
        (UIntTestFlags)0,
        "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags.0")]
    [TestCase(
        UIntTestFlags.Flag1 | UIntTestFlags.Flag2,
        "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags.Flag1"
        + ", Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags.Flag2")]
    [TestCase(
        (UIntTestFlags)1024,
        "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags.1024")]
    public void TestGetFullNameWhenEnumerationValueArgumentIsPassedThenSucceeds<TEnum>(TEnum enumValue, string expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.GetFullName(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(ConsoleColor.Green, @"""Green""")]
    [TestCase((ConsoleColor)(-1), @"""-1""")]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Control | ConsoleModifiers.Shift, @"""Alt"", ""Shift"", ""Control""")]
    [TestCase((ConsoleModifiers)0, @"""0""")]
    [TestCase(ConsoleModifiers.Alt | (ConsoleModifiers)16_777_216, @"""16777217""")]
    public void TestToUIString<TEnum>(TEnum enumValue, string expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.ToUIString(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(FileAttributes.Hidden, FileAttributes.Hidden, true)]
    [TestCase(FileAttributes.Hidden, FileAttributes.Normal, false)]
    [TestCase(FileAttributes.Normal | FileAttributes.ReadOnly, FileAttributes.Normal, true)]
    [TestCase(
        FileAttributes.Normal | FileAttributes.ReadOnly,
        FileAttributes.Normal | FileAttributes.ReadOnly,
        true)]
    [TestCase(
        FileAttributes.Normal | FileAttributes.ReadOnly | FileAttributes.Archive,
        FileAttributes.Normal | FileAttributes.ReadOnly,
        true)]
    [TestCase(
        FileAttributes.Normal | FileAttributes.ReadOnly | FileAttributes.System,
        FileAttributes.Normal | FileAttributes.ReadOnly | FileAttributes.Archive,
        false)]
    public void TestIsAllSetWhenValidArgumentOfFileAttributesIsPassedThenSucceeds(
        FileAttributes input,
        FileAttributes flags,
        bool expectedResult)
        => Assert.That(() => input.IsAllSet(flags), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(ULongTestFlags.Flag1, ULongTestFlags.Flag1, true)]
    [TestCase(ULongTestFlags.Flag1, ULongTestFlags.Flag2, false)]
    [TestCase(ULongTestFlags.Flag2 | ULongTestFlags.Flag3, ULongTestFlags.Flag2, true)]
    [TestCase(
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3,
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3,
        true)]
    [TestCase(
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3 | ULongTestFlags.Flag4,
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3,
        true)]
    [TestCase(
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3 | ULongTestFlags.Flag5,
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3 | ULongTestFlags.Flag4,
        false)]
    public void TestIsAllSetWhenValidArgumentOfTestFlagsUnsignedIsPassedThenSucceeds(
        ULongTestFlags input,
        ULongTestFlags flags,
        bool expectedResult)
        => Assert.That(() => input.IsAllSet(flags), Is.EqualTo(expectedResult));

    [Test]
    public void TestIsAllSetWhenNonFlagArgumentIsPassedThenThrows()
        => Assert.That(() => ConsoleColor.Gray.IsAllSet(ConsoleColor.Gray), Throws.ArgumentException);

    [Test]
    [TestCase(FileAttributes.Hidden, FileAttributes.Hidden, true)]
    [TestCase(FileAttributes.Hidden, FileAttributes.Normal, false)]
    [TestCase(FileAttributes.Normal | FileAttributes.ReadOnly, FileAttributes.Normal, true)]
    [TestCase(
        FileAttributes.Normal | FileAttributes.ReadOnly,
        FileAttributes.Normal | FileAttributes.ReadOnly,
        true)]
    [TestCase(
        FileAttributes.Normal | FileAttributes.ReadOnly | FileAttributes.Archive,
        FileAttributes.Normal | FileAttributes.ReadOnly,
        true)]
    [TestCase(
        FileAttributes.Normal | FileAttributes.ReadOnly | FileAttributes.System,
        FileAttributes.Normal | FileAttributes.ReadOnly | FileAttributes.Archive,
        true)]
    public void TestIsAnySetWhenValidArgumentOfFileAttributesIsPassedThenSucceeds(
        FileAttributes input,
        FileAttributes flags,
        bool expectedResult)
        => Assert.That(() => input.IsAnySet(flags), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(ULongTestFlags.Flag1, ULongTestFlags.Flag1, true)]
    [TestCase(ULongTestFlags.Flag1, ULongTestFlags.Flag2, false)]
    [TestCase(ULongTestFlags.Flag2 | ULongTestFlags.Flag3, ULongTestFlags.Flag2, true)]
    [TestCase(
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3,
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3,
        true)]
    [TestCase(
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3 | ULongTestFlags.Flag4,
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3,
        true)]
    [TestCase(
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3 | ULongTestFlags.Flag5,
        ULongTestFlags.Flag2 | ULongTestFlags.Flag3 | ULongTestFlags.Flag4,
        true)]
    public void TestIsAnySetWhenValidArgumentOfTestFlagsUnsignedIsPassedThenSucceeds(
        ULongTestFlags input,
        ULongTestFlags flags,
        bool expectedResult)
        => Assert.That(() => input.IsAnySet(flags), Is.EqualTo(expectedResult));

    [Test]
    public void TestIsAnySetWhenNonFlagArgumentIsPassedThenThrows()
        => Assert.That(() => ConsoleColor.Gray.IsAnySet(ConsoleColor.Gray), Throws.ArgumentException);

    [Test]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.Red }, false)]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.Red, ConsoleColor.DarkRed }, false)]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.Cyan }, true)]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.Cyan, ConsoleColor.DarkCyan }, true)]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.DarkCyan, ConsoleColor.Cyan }, true)]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.DarkCyan }, true)]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.Yellow, ConsoleColor.Cyan, ConsoleColor.DarkCyan }, true)]
    [TestCase(ConsoleColor.Cyan, new[] { ConsoleColor.Yellow, ConsoleColor.DarkCyan, ConsoleColor.Cyan }, true)]
    public void TestIsOneOfWhenValidArgumentsArePassedThenSucceeds(
        ConsoleColor input,
        ConsoleColor[] otherValues,
        bool expectedResult)
    {
        Assert.That(() => input.IsOneOf(otherValues), Is.EqualTo(expectedResult));
        Assert.That(() => input.IsOneOf((IEnumerable<ConsoleColor>)otherValues), Is.EqualTo(expectedResult));
    }

    [Test]
    public void TestIsOneOfWhenNullOtherValuesArgumentIsPassedThenThrows()
    {
        Assert.That(() => ConsoleColor.Gray.IsOneOf(null!), Throws.ArgumentNullException);

        Assert.That(
            () => ConsoleColor.Gray.IsOneOf(((IEnumerable<ConsoleColor>?)null)!),
            Throws.ArgumentNullException);
    }

    [Test]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Multiple target frameworks.")]
    public void TestCreateEnumValueNotImplementedExceptionWhenValidArgumentIsPassedThenSucceeds()
    {
        const ConsoleColor EnumValue = ConsoleColor.DarkRed;

        var baseExpectedErrorMessage = AsInvariant(
            $@"The operation for the enumeration value ""{nameof(ConsoleColor)}.{nameof(ConsoleColor.DarkRed)}"" is not implemented.");

#if NET5_0_OR_GREATER
        const string enumValueDetails = $"\x0020Expression: {{ {nameof(EnumValue)} }}.";
#else
        var enumValueDetails = string.Empty;
#endif
        var expectedEnumValueErrorMessage = baseExpectedErrorMessage + enumValueDetails;

        Assert.That(
            () => EnumValue.CreateEnumValueNotImplementedException(),
            Is.TypeOf<NotImplementedException>().With.Message.EqualTo(expectedEnumValueErrorMessage));

        var enumValueContainer = ValueContainer.Create(EnumValue);

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
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Multiple target frameworks.")]
    public void TestCreateEnumValueNotSupportedExceptionWhenValidArgumentIsPassedThenSucceeds()
    {
        const ConsoleColor EnumValue = ConsoleColor.DarkCyan;

        var baseExpectedErrorMessage = AsInvariant(
            $@"The operation for the enumeration value ""{nameof(ConsoleColor)}.{nameof(ConsoleColor.DarkCyan)}"" is not supported.");

#if NET5_0_OR_GREATER
        const string enumValueDetails = $"\x0020Expression: {{ {nameof(EnumValue)} }}.";
#else
        var enumValueDetails = string.Empty;
#endif
        var expectedEnumValueErrorMessage = baseExpectedErrorMessage + enumValueDetails;

        Assert.That(
            () => EnumValue.CreateEnumValueNotSupportedException(),
            Is.TypeOf<NotSupportedException>().With.Message.EqualTo(expectedEnumValueErrorMessage));

        var enumValueContainer = ValueContainer.Create(EnumValue);

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

    [Flags]
    public enum ULongTestFlags : ulong
    {
        Flag1 = 0b1,
        Flag2 = 0b10,
        Flag3 = 0b100,
        Flag4 = 0b1000,
        Flag5 = 0b10000
    }

    private enum TestEnumeration
    {
        NoZeroValue = 1
    }

    [Flags]
    private enum UIntTestFlags : uint
    {
        Flag1 = 0b1,
        Flag2 = 0b10
    }
}