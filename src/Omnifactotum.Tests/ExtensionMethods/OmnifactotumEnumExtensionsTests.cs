using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

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
        [TestCase(TestFlags.Flag2)]
        public void TestEnsureDefinedWhenDefinedEnumValueIsPassedThenSucceeds(Enum enumValue)
            => Assert.That(enumValue.EnsureDefined, Throws.Nothing);

        [Test]
        [TestCase((ConsoleColor)(-1))]
        [TestCase((TestEnumeration)0)]
        [TestCase(TestFlags.Flag1 | TestFlags.Flag2)]
        [TestCase((TestFlags)1024)]
        public void TestEnsureDefinedWhenUndefinedEnumValueIsPassedThenThrows(Enum enumValue)
            => Assert.That(enumValue.EnsureDefined, Throws.TypeOf<InvalidEnumArgumentException>());

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestIsDefinedWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Enum)null).IsDefined(), Throws.ArgumentNullException);

        [Test]
        [TestCase(FileAccess.Read, true)]
        [TestCase(ConsoleModifiers.Alt, true)]
        [TestCase(ConsoleColor.Green, true)]
        [TestCase(TestEnumeration.NoZeroValue, true)]
        [TestCase(TestFlags.Flag2, true)]
        [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control, false)]
        [TestCase((ConsoleColor)(-1), false)]
        [TestCase((TestEnumeration)0, false)]
        [TestCase(TestFlags.Flag1 | TestFlags.Flag2, false)]
        [TestCase((TestFlags)1024, false)]
        public void TestIsDefinedWhenEnumValueIsPassedThenSucceeds(Enum enumValue, bool expectedResult)
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
        [TestCase(TestFlags.Flag2, nameof(TestFlags.Flag2))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control,
            nameof(ConsoleModifiers.Alt) + ", " + nameof(ConsoleModifiers.Shift) + ", "
                + nameof(ConsoleModifiers.Control))]
        [TestCase(
            ConsoleModifiers.Alt | ConsoleModifiers.Shift | ConsoleModifiers.Control | (ConsoleModifiers)1024,
            "1031")]
        [TestCase((ConsoleColor)(-1), "-1")]
        [TestCase((TestEnumeration)0, "0")]
        [TestCase((TestFlags)0, "0")]
        [TestCase(TestFlags.Flag1 | TestFlags.Flag2, nameof(TestFlags.Flag1) + ", " + nameof(TestFlags.Flag2))]
        [TestCase((TestFlags)1024, "1024")]
        public void TestGetNameWhenEnumValueIsPassedThenSucceeds(Enum enumValue, string expectedResult)
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
        [TestCase(TestFlags.Flag2, nameof(TestFlags) + "." + nameof(TestFlags.Flag2))]
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
        [TestCase((TestFlags)0, nameof(TestFlags) + "." + "0")]
        [TestCase(
            TestFlags.Flag1 | TestFlags.Flag2,
            nameof(TestFlags) + "." + nameof(TestFlags.Flag1) + ", "
                + nameof(TestFlags) + "." + nameof(TestFlags.Flag2))]
        [TestCase((TestFlags)1024, nameof(TestFlags) + "." + "1024")]
        public void TestGetQualifiedNameWhenEnumValueIsPassedThenSucceeds(Enum enumValue, string expectedResult)
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
            TestFlags.Flag2,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(TestFlags) + "." + nameof(TestFlags.Flag2))]
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
            (TestFlags)0,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(TestFlags) + "." + "0")]
        [TestCase(
            TestFlags.Flag1 | TestFlags.Flag2,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "."
                + nameof(TestFlags) + "." + nameof(TestFlags.Flag1) + ", "
                + nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(TestFlags) + "." + nameof(TestFlags.Flag2))]
        [TestCase(
            (TestFlags)1024,
            nameof(Omnifactotum) + "." + nameof(Tests) + "." + nameof(ExtensionMethods) + "."
                + nameof(OmnifactotumEnumExtensionsTests) + "." + nameof(TestFlags) + "." + "1024")]
        public void TestGetFullNameWhenEnumValueIsPassedThenSucceeds(Enum enumValue, string expectedResult)
            => Assert.That(enumValue.GetFullName, Is.EqualTo(expectedResult));

        private enum TestEnumeration
        {
            NoZeroValue = 1
        }

        [Flags]
        private enum TestFlags : uint
        {
            Flag1 = 1,
            Flag2 = 2
        }
    }
}