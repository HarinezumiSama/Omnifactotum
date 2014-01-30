using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class EnumHelperTests
    {
        #region Tests

        [Test]
        public void TestGetValue()
        {
            const FileMode EnumValue = FileMode.OpenOrCreate;
            var enumValueString = EnumValue.ToString();
            var enumValueStringLower = enumValueString.ToLowerInvariant();

            Assert.That(() => EnumHelper.GetValue<int>(enumValueString), Throws.ArgumentException);

            Assert.That(() => EnumHelper.GetValue<FileMode>(enumValueString), Is.EqualTo(EnumValue));
            Assert.That(() => EnumHelper.GetValue<FileMode>(enumValueString, false), Is.EqualTo(EnumValue));
            Assert.That(() => EnumHelper.GetValue<FileMode>(enumValueString, true), Is.EqualTo(EnumValue));

            Assert.That(() => EnumHelper.GetValue<FileMode>(null), Throws.ArgumentException);
            Assert.That(() => EnumHelper.GetValue<FileMode>(string.Empty), Throws.ArgumentException);
            Assert.That(() => EnumHelper.GetValue<FileMode>(enumValueStringLower), Throws.ArgumentException);
            Assert.That(() => EnumHelper.GetValue<FileMode>(enumValueStringLower, false), Throws.ArgumentException);
            Assert.That(() => EnumHelper.GetValue<FileMode>(enumValueStringLower, true), Is.EqualTo(EnumValue));
        }

        [Test]
        public void TestGetAllValues()
        {
            Assert.That(() => EnumHelper.GetAllValues<int>(), Throws.ArgumentException);

            Assert.That(
                () => EnumHelper.GetAllValues<FileMode>(),
                Is.EquivalentTo(Enum.GetValues(typeof(FileMode))));

            Assert.That(
                () => EnumHelper.GetAllValues<FileAttributes>(),
                Is.EquivalentTo(Enum.GetValues(typeof(FileAttributes))));
        }

        [Test]
        public void TestGetAllFlagValues()
        {
            Assert.That(() => EnumHelper.GetAllFlagValues<int>(), Throws.ArgumentException);

            Assert.That(() => EnumHelper.GetAllFlagValues<FileMode>(), Throws.ArgumentException);

            Assert.That(
                () => EnumHelper.GetAllFlagValues<FileAttributes>(),
                Is.EquivalentTo(Enum.GetValues(typeof(FileAttributes))));

            Assert.That(
                () => EnumHelper.GetAllFlagValues<FileShare>(),
                Is.EquivalentTo(
                    Enum.GetValues(typeof(FileShare))
                        .Cast<FileShare>()
                        .Except(new[] { FileShare.None, FileShare.ReadWrite })));

            Assert.That(
                () => EnumHelper.GetAllFlagValues<FlagsInt64>(),
                Is.EquivalentTo(new[] { FlagsInt64.Bit0, FlagsInt64.Bit32, FlagsInt64.Bit49, FlagsInt64.Bit63 }));

            Assert.That(
                () => EnumHelper.GetAllFlagValues<FlagsUInt64>(),
                Is.EquivalentTo(new[] { FlagsUInt64.Bit0, FlagsUInt64.Bit35, FlagsUInt64.Bit53, FlagsUInt64.Bit63 }));
        }

        #endregion

        #region FlagsInt64 Enumeration

        [Flags]
        private enum FlagsInt64 : long
        {
            [UsedImplicitly]
            Bit0 = 1L << 0,

            [UsedImplicitly]
            Bit32 = 1L << 32,

            [UsedImplicitly]
            Bit49 = 1L << 49,

            [UsedImplicitly]
            Bit63 = 1L << 63,

            [UsedImplicitly]
            Mask1 = Bit0 | Bit63,

            [UsedImplicitly]
            All = unchecked((long)0xFFFFFFFFFFFFFFFFUL)
        }

        #endregion

        #region FlagsUInt64 Enumeration

        [Flags]
        private enum FlagsUInt64 : ulong
        {
            [UsedImplicitly]
            Bit0 = 1UL << 0,

            [UsedImplicitly]
            Bit35 = 1UL << 35,

            [UsedImplicitly]
            Bit53 = 1UL << 53,

            [UsedImplicitly]
            Bit63 = 1UL << 63,

            [UsedImplicitly]
            Mask1 = Bit0 | Bit63,

            [UsedImplicitly]
            All = 0xFFFFFFFFFFFFFFFFUL
        }

        #endregion
    }
}