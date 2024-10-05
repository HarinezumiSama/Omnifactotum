using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using NUnit.Framework;
using static Omnifactotum.FormattableStringFactotum;

#if NET6_0_OR_GREATER
using System.Reflection;
using System.Reflection.Emit;
#endif

//// ReSharper disable UseRawString
//// ReSharper disable VariableLengthStringHexEscapeSequence

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumEnumExtensions))]
internal sealed class OmnifactotumEnumExtensionsTests
{
    [Test]
    [TestCase(FileAccess.Read)]
    [TestCase(ConsoleModifiers.Alt)]
    [TestCase(ConsoleColor.Green)]
    [TestCase(NoZeroValueEnumeration.NonZeroValue)]
    [TestCase(NoZeroValueFlags.NonZeroFlag)]
    [TestCase(UIntTestFlags.Flag2)]
    public void TestEnsureDefinedWhenDefinedEnumerationValueArgumentIsPassedThenSucceeds<TEnum>(TEnum enumValue)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.EnsureDefined(), Is.EqualTo(enumValue));

    [Test]
    [TestCase(
        (ConsoleColor)(-1),
        @"The value -1 is not defined in the enumeration ""System.ConsoleColor"".")]
    [TestCase(
        (NoZeroValueEnumeration)0,
        @"The value 0 is not defined in the enumeration ""Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.NoZeroValueEnumeration"".")]
    [TestCase(
        (NoZeroValueFlags)0,
        @"The value 0 is not defined in the enumeration ""Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.NoZeroValueFlags"".")]
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
    [TestCase(NoZeroValueEnumeration.NonZeroValue, true)]
    [TestCase(NoZeroValueFlags.NonZeroFlag, true)]
    [TestCase(UIntTestFlags.Flag2, true)]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control, false)]
    [TestCase((ConsoleColor)(-1), false)]
    [TestCase((NoZeroValueEnumeration)0, false)]
    [TestCase((NoZeroValueFlags)0, false)]
    [TestCase(UIntTestFlags.Flag1 | UIntTestFlags.Flag2, false)]
    [TestCase((UIntTestFlags)1024, false)]
    public void TestIsDefinedWhenEnumerationValueArgumentIsPassedThenSucceeds<TEnum>(TEnum enumValue, bool expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.IsDefined(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(FileAccess.Read, "Read")]
    [TestCase(ConsoleModifiers.Alt, "Alt")]
    [TestCase(ConsoleColor.Green, "Green")]
    [TestCase(NoZeroValueEnumeration.NonZeroValue, "NonZeroValue")]
    [TestCase(NoZeroValueFlags.NonZeroFlag, "NonZeroFlag")]
    [TestCase(UIntTestFlags.Flag2, "Flag2")]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control, "Alt, Shift, Control")]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024, "1031")]
    [TestCase(FileShare.None, "None")]
    [TestCase((ConsoleColor)(-1), "-1")]
    [TestCase((NoZeroValueEnumeration)0, "0")]
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
    [TestCase(NoZeroValueEnumeration.NonZeroValue, "NoZeroValueEnumeration.NonZeroValue")]
    [TestCase(NoZeroValueFlags.NonZeroFlag, "NoZeroValueFlags.NonZeroFlag")]
    [TestCase(UIntTestFlags.Flag2, "UIntTestFlags.Flag2")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
        "ConsoleModifiers.Alt, ConsoleModifiers.Shift, ConsoleModifiers.Control")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
        "ConsoleModifiers.1031")]
    [TestCase(FileShare.None, "FileShare.None")]
    [TestCase((ConsoleColor)(-1), "ConsoleColor.-1")]
    [TestCase((NoZeroValueEnumeration)0, "NoZeroValueEnumeration.0")]
    [TestCase((NoZeroValueFlags)0, "NoZeroValueFlags.0")]
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
    [TestCase(NoZeroValueEnumeration.NonZeroValue, "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.NoZeroValueEnumeration.NonZeroValue")]
    [TestCase(NoZeroValueFlags.NonZeroFlag, "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.NoZeroValueFlags.NonZeroFlag")]
    [TestCase(UIntTestFlags.Flag2, "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.UIntTestFlags.Flag2")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
        "System.ConsoleModifiers.Alt, System.ConsoleModifiers.Shift, System.ConsoleModifiers.Control")]
    [TestCase(
        ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
        "System.ConsoleModifiers.1031")]
    [TestCase(FileShare.None, "System.IO.FileShare.None")]
    [TestCase((ConsoleColor)(-1), nameof(System) + "." + nameof(ConsoleColor) + "." + "-1")]
    [TestCase(
        (NoZeroValueEnumeration)0,
        "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.NoZeroValueEnumeration.0")]
    [TestCase(
        (NoZeroValueFlags)0,
        "Omnifactotum.Tests.ExtensionMethods.OmnifactotumEnumExtensionsTests.NoZeroValueFlags.0")]
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
#if NET8_0_OR_GREATER
    [TestCase(ConsoleModifiers.None, @"""None""")]
#else
    [TestCase((ConsoleModifiers)0, @"""0""")]
#endif
    [TestCase(ConsoleModifiers.Alt | (ConsoleModifiers)16_777_216, @"""16777217""")]
    [TestCase(FileShare.None, @"""None""")]
    [TestCase(FileOptions.None, @"""None""")]
    [TestCase(FileOptions.WriteThrough | FileOptions.Encrypted | FileOptions.DeleteOnClose, @"""Encrypted"", ""DeleteOnClose"", ""WriteThrough""")]
    [TestCase((NoZeroValueEnumeration)0, @"""0""")]
    [TestCase(NoZeroValueEnumeration.NonZeroValue, @"""NonZeroValue""")]
    [TestCase((NoZeroValueFlags)0, @"""0""")]
    [TestCase(NoZeroValueFlags.NonZeroFlag, @"""NonZeroFlag""")]
    [TestCase((ULongTestFlags)0, @"""0""")]
    [TestCase(ULongTestFlags.Flag2 | ULongTestFlags.Flag34, @"""Flag2"", ""Flag34""")]
    [TestCase(ULongTestFlags.Flag2 | ULongTestFlags.Flag64, @"""Flag2"", ""Flag64""")]
    public void TestToUIString<TEnum>(TEnum enumValue, string expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.ToUIString(), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(ConsoleColor.Green, "10 (Green)")]
    [TestCase((ConsoleColor)(-1), "-1")]
    [TestCase(HttpStatusCode.NotFound, "404 (NotFound)")]
    [TestCase(HttpStatusCode.Unauthorized, "401 (Unauthorized)")]
    [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Control | ConsoleModifiers.Shift, "0x00000007 (Alt, Shift, Control)")]
#if NET8_0_OR_GREATER
    [TestCase(ConsoleModifiers.None, "0x00000000 (None)")]
#else
    [TestCase((ConsoleModifiers)0, "0x00000000")]
#endif
    [TestCase(ConsoleModifiers.Alt | (ConsoleModifiers)16_777_216, "0x01000001")]
    [TestCase(FileShare.None, "0x00000000 (None)")]
    [TestCase(FileOptions.None, "0x00000000 (None)")]
    [TestCase(FileOptions.WriteThrough | FileOptions.Encrypted | FileOptions.DeleteOnClose, "0x84004000 (Encrypted, DeleteOnClose, WriteThrough)")]
    [TestCase((NoZeroValueEnumeration)0, "0")]
    [TestCase((NoZeroValueFlags)0, "0x00000000")]
    [TestCase((UIntTestFlags)0, "0x00000000")]
    [TestCase((ULongTestFlags)0, "0x0000000000000000")]
    [TestCase(ULongTestFlags.Flag2 | ULongTestFlags.Flag34, "0x0000000200000002 (Flag2, Flag34)")]
    [TestCase(ULongTestFlags.Flag2 | ULongTestFlags.Flag64, "0x8000000000000002 (Flag2, Flag64)")]
    public void TestGetDescription<TEnum>(TEnum enumValue, string expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.GetDescription(), Is.EqualTo(expectedResult));

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

    [Test]
    [TestCase((FlagsInt8)1, 1UL)]
    [TestCase((FlagsUInt8)1, 1UL)]
    [TestCase((FlagsInt16)1, 1UL)]
    [TestCase((FlagsUInt16)1, 1UL)]
    [TestCase((FlagsInt32)1, 1UL)]
    [TestCase((FlagsUInt32)1, 1UL)]
    [TestCase((FlagsInt64)1, 1UL)]
    [TestCase((FlagsUInt64)1, 1UL)]
    [TestCase(FlagsInt8.Bit7, 0x80UL)]
    [TestCase(FlagsUInt8.Bit7, 0x80UL)]
    [TestCase(FlagsInt16.Bit15, 0x8000UL)]
    [TestCase(FlagsUInt16.Bit15, 0x8000UL)]
    [TestCase(FlagsInt32.Bit31, 0x80000000UL)]
    [TestCase(FlagsUInt32.Bit31, 0x80000000UL)]
    [TestCase(FlagsInt64.Bit63, 0x8000000000000000UL)]
    [TestCase(FlagsUInt64.Bit63, 0x8000000000000000UL)]
    [TestCase(System.Diagnostics.Tracing.EventChannel.None, 0UL)]
    [TestCase(System.Diagnostics.Tracing.EventChannel.Debug, 19UL)]
    [TestCase(System.Runtime.InteropServices.ComTypes.TYPEFLAGS.TYPEFLAG_FAPPOBJECT, 1UL)]
    [TestCase(System.Runtime.InteropServices.ComTypes.TYPEFLAGS.TYPEFLAG_FPROXY, 0x4000UL)]
    [TestCase(FileMode.Append, 6UL)]
    [TestCase(FileOptions.None, 0UL)]
    [TestCase(FileOptions.WriteThrough, 0x80000000UL)]
    [TestCase(System.Diagnostics.Tracing.EventKeywords.None, 0UL)]
    [TestCase(System.Diagnostics.Tracing.EventKeywords.AuditSuccess, 0x0020000000000000UL)]
    [TestCase(System.Diagnostics.Tracing.EventKeywords.All, 0xFFFFFFFFFFFFFFFFUL)]
    public void TestToUInt64<TEnum>(TEnum enumValue, ulong expectedResult)
        where TEnum : struct, Enum
        => ExecuteToUInt64TestCase(enumValue, expectedResult);

#if NET6_0_OR_GREATER

//// `Boolean` as underlying `Enum` type is not supported as of .NET 8:
//// - https://github.com/dotnet/runtime/issues/79224
//// - https://github.com/dotnet/runtime/issues/79962
#if !NET8_0_OR_GREATER
    [Test]
    public void TestToUInt64WhenBoolBasedEnum()
    {
        var assemblyName = $"{nameof(TestToUInt64WhenBoolBasedEnum)}_{Guid.NewGuid():N}";
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndCollect);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{assemblyName}_Module");

        var enumBuilder = moduleBuilder.DefineEnum("BoolEnum", TypeAttributes.Public, typeof(bool));
        enumBuilder.DefineLiteral("FalseValue", false);
        enumBuilder.DefineLiteral("TrueValue", true);
        var enumType = enumBuilder.CreateType().EnsureNotNull();

        var executeTestCaseMethodDefinition =
            ((Action<NoZeroValueEnumeration, ulong>)ExecuteToUInt64TestCase).Method.GetGenericMethodDefinition().EnsureNotNull();

        var executeTestCaseMethod = executeTestCaseMethodDefinition.MakeGenericMethod(enumType);

        var enumValues = Enum.GetValues(enumType);
        foreach (var enumValue in enumValues)
        {
            executeTestCaseMethod.Invoke(null, new[] { enumValue, (bool)enumValue ? 1UL : 0UL });
        }
    }
#endif

    [Test]
    public void TestToUInt64WhenCharBasedEnum()
    {
        var assemblyName = $"{nameof(TestToUInt64WhenCharBasedEnum)}_{Guid.NewGuid():N}";
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndCollect);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{assemblyName}_Module");

        var enumBuilder = moduleBuilder.DefineEnum("CharEnum", TypeAttributes.Public, typeof(char));
        enumBuilder.DefineLiteral("Nil", '\x0000');
        enumBuilder.DefineLiteral("Whitespace", '\x0020');
        enumBuilder.DefineLiteral("Zero", '0');
        enumBuilder.DefineLiteral("A", 'A');
        enumBuilder.DefineLiteral("w", 'w');
        enumBuilder.DefineLiteral("YearInJapanese", '年');
        enumBuilder.DefineLiteral("CapitalLetterYeInUkrainian", 'Є');
        var enumType = enumBuilder.CreateType().EnsureNotNull();

        var executeTestCaseMethodDefinition = ((Action<NoZeroValueEnumeration, ulong>)ExecuteToUInt64TestCase).Method.GetGenericMethodDefinition().EnsureNotNull();
        var executeTestCaseMethod = executeTestCaseMethodDefinition.MakeGenericMethod(enumType);

        var enumValues = Enum.GetValues(enumType);
        foreach (var enumValue in enumValues)
        {
            executeTestCaseMethod.Invoke(null, new[] { enumValue, (ulong)(char)enumValue });
        }
    }

#endif

    private static void ExecuteToUInt64TestCase<TEnum>(TEnum enumValue, ulong expectedResult)
        where TEnum : struct, Enum
        => Assert.That(() => enumValue.ToUInt64(), Is.EqualTo(expectedResult));

    [Flags]
    private enum FlagsInt8 : sbyte
    {
        Bit7 = unchecked((sbyte)(1 << 7))
    }

    [Flags]
    private enum FlagsUInt8 : byte
    {
        Bit7 = 1 << 7
    }

    [Flags]
    private enum FlagsInt16 : short
    {
        Bit15 = unchecked((short)(1 << 15))
    }

    [Flags]
    private enum FlagsUInt16 : ushort
    {
        Bit15 = 1 << 15
    }

    [Flags]
    [SuppressMessage("ReSharper", "EnumUnderlyingTypeIsInt")]
    private enum FlagsInt32 : int
    {
        Bit31 = 1 << 31
    }

    [Flags]
    private enum FlagsUInt32 : uint
    {
        Bit31 = (uint)1 << 31
    }

    [Flags]
    private enum FlagsInt64 : long
    {
        Bit63 = 1L << 63
    }

    [Flags]
    private enum FlagsUInt64 : ulong
    {
        Bit63 = 1UL << 63
    }

    [Flags]
    public enum ULongTestFlags : ulong
    {
        Flag1 = 0b1,
        Flag2 = 0b10,
        Flag3 = 0b100,
        Flag4 = 0b1000,
        Flag5 = 0b10000,
        Flag34 = 0b1000000000000000000000000000000000,
        Flag64 = 0b1000000000000000000000000000000000000000000000000000000000000000
    }

    private enum NoZeroValueEnumeration
    {
        NonZeroValue = 1
    }

    [Flags]
    private enum NoZeroValueFlags
    {
        NonZeroFlag = 0x1
    }

    [Flags]
    private enum UIntTestFlags : uint
    {
        Flag1 = 0b1,
        Flag2 = 0b10
    }
}