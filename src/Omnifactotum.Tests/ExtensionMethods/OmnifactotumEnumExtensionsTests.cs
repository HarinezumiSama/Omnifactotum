﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumEnumExtensions))]
    internal sealed class OmnifactotumEnumExtensionsTests
    {
        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestEnsureDefinedWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Enum)null).EnsureDefined(), Throws.ArgumentNullException);

        [Test]
        [TestCase(FileAccess.Read)]
        [TestCase(ConsoleModifiers.Alt)]
        [TestCase(ConsoleColor.Green)]
        [TestCase(TestEnumeration.NoZeroValue)]
        [TestCase(UIntTestFlags.Flag2)]
        public void TestEnsureDefinedWhenDefinedEnumerationValueArgumentIsPassedThenSucceeds(Enum enumValue)
            => Assert.That(enumValue.EnsureDefined, Throws.Nothing);

        [Test]
        [TestCase((ConsoleColor)(-1))]
        [TestCase((TestEnumeration)0)]
        [TestCase(UIntTestFlags.Flag1 | UIntTestFlags.Flag2)]
        [TestCase((UIntTestFlags)1024)]
        public void TestEnsureDefinedWhenUndefinedEnumerationValueArgumentIsPassedThenThrows(Enum enumValue)
            => Assert.That(enumValue.EnsureDefined, Throws.TypeOf<InvalidEnumArgumentException>());

        [Test]
        [TestCase(FileAccess.Read, true)]
        [TestCase(ConsoleModifiers.Alt, true)]
        [TestCase(ConsoleColor.Green, true)]
        [TestCase(TestEnumeration.NoZeroValue, true)]
        [TestCase(UIntTestFlags.Flag2, true)]
        [TestCase(null, false)]
        [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control, false)]
        [TestCase((ConsoleColor)(-1), false)]
        [TestCase((TestEnumeration)0, false)]
        [TestCase(UIntTestFlags.Flag1 | UIntTestFlags.Flag2, false)]
        [TestCase((UIntTestFlags)1024, false)]
        public void TestIsDefinedWhenEnumerationValueArgumentIsPassedThenSucceeds(Enum enumValue, bool expectedResult)
            => Assert.That(enumValue.IsDefined, Is.EqualTo(expectedResult));

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestGetNameWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Enum)null).GetName(), Throws.ArgumentNullException);

        [Test]
        [TestCase(FileAccess.Read, nameof(FileAccess.Read))]
        [TestCase(ConsoleModifiers.Alt, nameof(ConsoleModifiers.Alt))]
        [TestCase(ConsoleColor.Green, nameof(ConsoleColor.Green))]
        [TestCase(TestEnumeration.NoZeroValue, nameof(TestEnumeration.NoZeroValue))]
        [TestCase(UIntTestFlags.Flag2, nameof(UIntTestFlags.Flag2))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
            nameof(ConsoleModifiers.Alt) + ", " + nameof(ConsoleModifiers.Shift) + ", "
                + nameof(ConsoleModifiers.Control))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
            "1031")]
        [TestCase((ConsoleColor)(-1), "-1")]
        [TestCase((TestEnumeration)0, "0")]
        [TestCase((UIntTestFlags)0, "0")]
        [TestCase(UIntTestFlags.Flag1 | UIntTestFlags.Flag2,
            nameof(UIntTestFlags.Flag1) + ", " + nameof(UIntTestFlags.Flag2))]
        [TestCase((UIntTestFlags)1024, "1024")]
        public void TestGetNameWhenEnumerationValueArgumentIsPassedThenSucceeds(Enum enumValue, string expectedResult)
            => Assert.That(enumValue.GetName, Is.EqualTo(expectedResult));

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestGetQualifiedNameWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Enum)null).GetQualifiedName(), Throws.ArgumentNullException);

        [Test]
        [TestCase(FileAccess.Read, nameof(FileAccess) + "." + nameof(FileAccess.Read))]
        [TestCase(ConsoleModifiers.Alt, nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Alt))]
        [TestCase(ConsoleColor.Green, nameof(ConsoleColor) + "." + nameof(ConsoleColor.Green))]
        [TestCase(TestEnumeration.NoZeroValue, nameof(TestEnumeration) + "." + nameof(TestEnumeration.NoZeroValue))]
        [TestCase(UIntTestFlags.Flag2, nameof(UIntTestFlags) + "." + nameof(UIntTestFlags.Flag2))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
            nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Alt) + ", "
                + nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Shift) + ", "
                + nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Control))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
            nameof(ConsoleModifiers) + "." + "1031")]
        [TestCase((ConsoleColor)(-1), nameof(ConsoleColor) + "." + "-1")]
        [TestCase((TestEnumeration)0, nameof(TestEnumeration) + "." + "0")]
        [TestCase((UIntTestFlags)0, nameof(UIntTestFlags) + "." + "0")]
        [TestCase(
            UIntTestFlags.Flag1 | UIntTestFlags.Flag2,
            nameof(UIntTestFlags) + "." + nameof(UIntTestFlags.Flag1) + ", "
                + nameof(UIntTestFlags) + "." + nameof(UIntTestFlags.Flag2))]
        [TestCase((UIntTestFlags)1024, nameof(UIntTestFlags) + "." + "1024")]
        public void TestGetQualifiedNameWhenEnumerationValueArgumentIsPassedThenSucceeds(
            Enum enumValue,
            string expectedResult)
            => Assert.That(enumValue.GetQualifiedName, Is.EqualTo(expectedResult));

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestGetFullNameWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Enum)null).GetFullName(), Throws.ArgumentNullException);

        [Test]
        [TestCase(
            FileAccess.Read,
            nameof(System) + "." + nameof(System.IO) + "." + nameof(FileAccess) + "." + nameof(FileAccess.Read))]
        [TestCase(
            ConsoleModifiers.Alt,
            nameof(System) + "." + nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Alt))]
        [TestCase(
            ConsoleColor.Green,
            nameof(System) + "." + nameof(ConsoleColor) + "." + nameof(ConsoleColor.Green))]
        [TestCase(
            TestEnumeration.NoZeroValue,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(TestEnumeration) + "."
                + nameof(TestEnumeration.NoZeroValue))]
        [TestCase(
            UIntTestFlags.Flag2,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(UIntTestFlags) + "."
                + nameof(UIntTestFlags.Flag2))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
            nameof(System) + "." + nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Alt) + ", "
                + nameof(System) + "." + nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Shift) + ", "
                + nameof(System) + "." + nameof(ConsoleModifiers) + "." + nameof(ConsoleModifiers.Control))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
            nameof(System) + "." + nameof(ConsoleModifiers) + "." + "1031")]
        [TestCase((ConsoleColor)(-1), nameof(System) + "." + nameof(ConsoleColor) + "." + "-1")]
        [TestCase(
            (TestEnumeration)0,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(TestEnumeration) + "." + "0")]
        [TestCase(
            (UIntTestFlags)0,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(UIntTestFlags) + "." + "0")]
        [TestCase(
            UIntTestFlags.Flag1 | UIntTestFlags.Flag2,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "."
                + nameof(UIntTestFlags) + "." + nameof(UIntTestFlags.Flag1) + ", "
                + nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(UIntTestFlags) + "."
                + nameof(UIntTestFlags.Flag2))]
        [TestCase(
            (UIntTestFlags)1024,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(UIntTestFlags) + "." + "1024")]
        public void TestGetFullNameWhenEnumerationValueArgumentIsPassedThenSucceeds(
            Enum enumValue,
            string expectedResult)
            => Assert.That(enumValue.GetFullName, Is.EqualTo(expectedResult));

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
        [TestCase(17, new[] { 17 })]
        [TestCase(17, new[] { 17, 18 })]
        [TestCase(17, new[] { 18 })]
        public void TestIsOneOfWhenNonEnumerationArgumentsArePassedThenThrows(int input, int[] otherValues)
        {
            Assert.That(() => input.IsOneOf(otherValues), Throws.ArgumentException);
            Assert.That(() => input.IsOneOf((IEnumerable<int>)otherValues), Throws.ArgumentException);
        }

        [Test]
        public void TestIsOneOfWhenNullOtherValuesArgumentIsPassedThenThrows()
        {
            Assert.That(() => ConsoleColor.Gray.IsOneOf(null), Throws.ArgumentNullException);

            Assert.That(
                () => ConsoleColor.Gray.IsOneOf(((IEnumerable<ConsoleColor>)null)!),
                Throws.ArgumentNullException);
        }

        [Test]
        public void TestCreateEnumValueNotImplementedExceptionWhenValidArgumentIsPassedThenSucceeds()
            => Assert.That(
                () => ConsoleColor.DarkRed.CreateEnumValueNotImplementedException(),
                Is.TypeOf<NotImplementedException>()
                    .With
                    .Message
                    .Contains(AsInvariant($@"""{nameof(ConsoleColor)}.{nameof(ConsoleColor.DarkRed)}""")));

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestCreateEnumValueNotImplementedExceptionWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Enum)null).CreateEnumValueNotImplementedException(), Throws.ArgumentNullException);

        [Test]
        public void TestCreateEnumValueNotSupportedExceptionWhenValidArgumentIsPassedThenSucceeds()
            => Assert.That(
                () => ConsoleColor.DarkRed.CreateEnumValueNotSupportedException(),
                Is.TypeOf<NotSupportedException>()
                    .With
                    .Message
                    .Contains(AsInvariant($@"""{nameof(ConsoleColor)}.{nameof(ConsoleColor.DarkRed)}""")));

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestCreateEnumValueNotSupportedExceptionWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Enum)null).CreateEnumValueNotSupportedException(), Throws.ArgumentNullException);

        private enum TestEnumeration
        {
            NoZeroValue = 1
        }

        [Flags]
        public enum UIntTestFlags : uint
        {
            Flag1 = 0b1,
            Flag2 = 0b10
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
    }
}