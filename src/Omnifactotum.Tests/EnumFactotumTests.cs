using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(EnumFactotum))]
internal sealed class EnumFactotumTests
{
    [Test]
    public void TestGetValue()
    {
        const FileMode EnumValue = FileMode.OpenOrCreate;
        var enumValueString = EnumValue.ToString();
        var enumValueStringLower = enumValueString.ToLowerInvariant();

        Assert.That(() => EnumFactotum.GetValue<FileMode>(enumValueString), Is.EqualTo(EnumValue));
        Assert.That(() => EnumFactotum.GetValue<FileMode>(enumValueString, false), Is.EqualTo(EnumValue));
        Assert.That(() => EnumFactotum.GetValue<FileMode>(enumValueString, true), Is.EqualTo(EnumValue));

        Assert.That(() => EnumFactotum.GetValue<FileMode>(null!), Throws.ArgumentException);
        Assert.That(() => EnumFactotum.GetValue<FileMode>(string.Empty), Throws.ArgumentException);
        Assert.That(() => EnumFactotum.GetValue<FileMode>(enumValueStringLower), Throws.ArgumentException);
        Assert.That(() => EnumFactotum.GetValue<FileMode>(enumValueStringLower, false), Throws.ArgumentException);
        Assert.That(() => EnumFactotum.GetValue<FileMode>(enumValueStringLower, true), Is.EqualTo(EnumValue));
    }

    [Test]
    public void TestGetAllValues()
    {
        Assert.That(
            EnumFactotum.GetAllValues<FileMode>,
            Is.EquivalentTo(Enum.GetValues(typeof(FileMode))));

        Assert.That(
            EnumFactotum.GetAllValues<FileAttributes>,
            Is.EquivalentTo(Enum.GetValues(typeof(FileAttributes))));
    }

    [Test]
    public void TestGetAllFlagValues()
    {
        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsInt8>,
            Is.EquivalentTo(new[] { FlagsInt8.Bit0, FlagsInt8.Bit3, FlagsInt8.Bit7 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsUInt8>,
            Is.EquivalentTo(new[] { FlagsUInt8.Bit0, FlagsUInt8.Bit5, FlagsUInt8.Bit7 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsInt16>,
            Is.EquivalentTo(new[] { FlagsInt16.Bit0, FlagsInt16.Bit13, FlagsInt16.Bit15 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsUInt16>,
            Is.EquivalentTo(new[] { FlagsUInt16.Bit0, FlagsUInt16.Bit11, FlagsUInt16.Bit15 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsInt32>,
            Is.EquivalentTo(new[] { FlagsInt32.Bit0, FlagsInt32.Bit19, FlagsInt32.Bit31 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsUInt32>,
            Is.EquivalentTo(new[] { FlagsUInt32.Bit0, FlagsUInt32.Bit17, FlagsUInt32.Bit31 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsInt64>,
            Is.EquivalentTo(new[] { FlagsInt64.Bit0, FlagsInt64.Bit11, FlagsInt64.Bit31, FlagsInt64.Bit32, FlagsInt64.Bit37, FlagsInt64.Bit63 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FlagsUInt64>,
            Is.EquivalentTo(
                new[] { FlagsUInt64.Bit0, FlagsUInt64.Bit7, FlagsUInt64.Bit31, FlagsUInt64.Bit32, FlagsUInt64.Bit53, FlagsUInt64.Bit63 }));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FileAttributes>,
            Is.EquivalentTo(
                Enum.GetValues(typeof(FileAttributes))
#if NET8_0_OR_GREATER
                    .Cast<FileAttributes>()
                    .Except([FileAttributes.None])
#endif
            ));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FileOptions>,
            Is.EquivalentTo(
                Enum.GetValues(typeof(FileOptions))
                    .Cast<FileOptions>()
                    .Except(new[] { FileOptions.None })));

        Assert.That(
            EnumFactotum.GetAllFlagValues<FileShare>,
            Is.EquivalentTo(
                Enum.GetValues(typeof(FileShare))
                    .Cast<FileShare>()
                    .Except(new[] { FileShare.None, FileShare.ReadWrite })));

        Assert.That(EnumFactotum.GetAllFlagValues<FileMode>, Throws.ArgumentException);
    }

    [Flags]
    private enum FlagsInt8 : sbyte
    {
        [UsedImplicitly]
        Bit0 = 1 << 0,

        [UsedImplicitly]
        Bit3 = 1 << 3,

        [UsedImplicitly]
        Bit7 = unchecked((sbyte)(1 << 7)),

        [UsedImplicitly]
        MaskValue = Bit0 | Bit3,

        [UsedImplicitly]
        All = unchecked((sbyte)0xFF)
    }

    [Flags]
    private enum FlagsUInt8 : byte
    {
        [UsedImplicitly]
        Bit0 = 1 << 0,

        [UsedImplicitly]
        Bit5 = 1 << 5,

        [UsedImplicitly]
        Bit7 = 1 << 7,

        [UsedImplicitly]
        MaskValue = Bit0 | Bit5,

        [UsedImplicitly]
        All = 0xFF
    }

    [Flags]
    private enum FlagsInt16 : short
    {
        [UsedImplicitly]
        Bit0 = 1 << 0,

        [UsedImplicitly]
        Bit13 = 1 << 13,

        [UsedImplicitly]
        Bit15 = unchecked((short)(1 << 15)),

        [UsedImplicitly]
        MaskValue = Bit0 | Bit13,

        [UsedImplicitly]
        All = unchecked((short)0xFFFF)
    }

    [Flags]
    private enum FlagsUInt16 : ushort
    {
        [UsedImplicitly]
        Bit0 = 1 << 0,

        [UsedImplicitly]
        Bit11 = 1 << 11,

        [UsedImplicitly]
        Bit15 = 1 << 15,

        [UsedImplicitly]
        MaskValue = Bit0 | Bit11,

        [UsedImplicitly]
        All = 0xFFFF
    }

    [Flags]
    [SuppressMessage("ReSharper", "EnumUnderlyingTypeIsInt")]
    private enum FlagsInt32 : int
    {
        [UsedImplicitly]
        Bit0 = 1 << 0,

        [UsedImplicitly]
        Bit19 = 1 << 19,

        [UsedImplicitly]
        Bit31 = 1 << 31,

        [UsedImplicitly]
        MaskValue = Bit0 | Bit31,

        [UsedImplicitly]
        All = unchecked((int)0xFFFFFFFF)
    }

    [Flags]
    private enum FlagsUInt32 : uint
    {
        [UsedImplicitly]
        Bit0 = (uint)1 << 0,

        [UsedImplicitly]
        Bit17 = (uint)1 << 17,

        [UsedImplicitly]
        Bit31 = (uint)1 << 31,

        [UsedImplicitly]
        MaskValue = Bit0 | Bit31,

        [UsedImplicitly]
        All = 0xFFFFFFFF
    }

    [Flags]
    private enum FlagsInt64 : long
    {
        [UsedImplicitly]
        Bit0 = 1L << 0,

        [UsedImplicitly]
        Bit11 = 1L << 11,

        [UsedImplicitly]
        Bit31 = 1L << 31,

        [UsedImplicitly]
        Bit32 = 1L << 32,

        [UsedImplicitly]
        Bit37 = 1L << 37,

        [UsedImplicitly]
        Bit63 = 1L << 63,

        [UsedImplicitly]
        MaskValue = Bit0 | Bit63,

        [UsedImplicitly]
        All = unchecked((long)0xFFFFFFFFFFFFFFFFUL)
    }

    [Flags]
    private enum FlagsUInt64 : ulong
    {
        [UsedImplicitly]
        Bit0 = 1UL << 0,

        [UsedImplicitly]
        Bit7 = 1UL << 7,

        [UsedImplicitly]
        Bit31 = 1UL << 31,

        [UsedImplicitly]
        Bit32 = 1UL << 32,

        [UsedImplicitly]
        Bit53 = 1UL << 53,

        [UsedImplicitly]
        Bit63 = 1UL << 63,

        [UsedImplicitly]
        MaskValue = Bit0 | Bit63,

        [UsedImplicitly]
        All = 0xFFFFFFFFFFFFFFFFUL
    }
}