using System;
using System.Linq;
using NUnit.Framework;

#if NET7_0_OR_GREATER
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
#endif

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(ValueRangeExtensions))]
internal sealed class ValueRangeExtensionsTests
{
#if NET7_0_OR_GREATER
    public static IEnumerable<TestCaseData> TestEnumerateForBinaryIntegerValueSucceedsCases
    {
        get
        {
            yield return new TestCaseData(IntPtr.MinValue);
            yield return new TestCaseData(UIntPtr.MinValue);

            yield return new TestCaseData(Int128.MinValue);
            yield return new TestCaseData(UInt128.MinValue);
        }
    }

    public static IEnumerable<TestCaseData> TestGetMidpointCases
    {
        get
        {
            yield return new TestCaseData(Half.MaxValue, Half.MaxValue, Half.PositiveInfinity);
            yield return new TestCaseData(NFloat.MaxValue, NFloat.MaxValue, NFloat.PositiveInfinity);
            yield return new TestCaseData(1m, 3m, 2m);
            yield return new TestCaseData(-1.5m, 4m, 1.25m);
        }
    }

    public static IEnumerable<TestCaseData> TestGetMidpointWhenOverflowThenThrowsCases
    {
        get
        {
            yield return new TestCaseData(decimal.MaxValue, decimal.MaxValue);
        }
    }
#endif

    [Test]
    public void TestEnumerateWithGetNextWhenInvalidArgumentsThenThrows()
    {
        // `ToArray()` is used to execute enumeration of the sequence

        Assert.That(
            () => ValueRange.Create(1, 2).Enumerate(null!).ToArray(),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("getNext"));

        Assert.That(
            () => ValueRange.Create(1, 2).Enumerate(i => i).ToArray(),
            Throws.ArgumentException
                .With.Property(nameof(ArgumentException.ParamName))
                .EqualTo("getNext")
                .With.Message.EqualTo("The next value (1) is less than or equal to the previous value (1). (Parameter 'getNext')"));

        Assert.That(
            () => ValueRange.Create(1, 2).Enumerate(i => i - 1).ToArray(),
            Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName))
                .EqualTo("getNext")
                .With.Message.EqualTo("The next value (0) is less than or equal to the previous value (1). (Parameter 'getNext')"));
    }

    [Test]
    public void TestEnumerateWithGetNextWhenValidArgumentsThenSucceeds()
    {
        Assert.That(
            () => ValueRange.Create(1, 1).Enumerate(Increment).ToArray(),
            Is.EqualTo(new[] { 1 }));

        Assert.That(
            () => ValueRange.Create(1, 2).Enumerate(Increment).ToArray(),
            Is.EqualTo(new[] { 1, 2 }));

        Assert.That(
            () => ValueRange.Create(17, 23).Enumerate(Increment).ToArray(),
            Is.EqualTo(new[] { 17, 18, 19, 20, 21, 22, 23 }));

        Assert.That(
            () => ValueRange.Create(-17, -12).Enumerate(i => i + 2).ToArray(),
            Is.EqualTo(new[] { -17, -15, -13 }));

        static int Increment(int i) => i + 1;
    }

#if NET7_0_OR_GREATER
    [Test]
    [TestCase(char.MinValue)]
    [TestCase(sbyte.MinValue)]
    [TestCase(byte.MinValue)]
    [TestCase(short.MinValue)]
    [TestCase(ushort.MinValue)]
    [TestCase(int.MinValue)]
    [TestCase(uint.MinValue)]
    [TestCase(long.MinValue)]
    [TestCase(ulong.MinValue)]
    [TestCaseSource(nameof(TestEnumerateForBinaryIntegerValueSucceedsCases))]
    public void TestEnumerateForBinaryIntegerValueSucceeds<T>(T typePlaceholderAndMinValue)
        where T : IBinaryInteger<T>
    {
        Assert.That(T.IsZero(typePlaceholderAndMinValue) | T.IsNegative(typePlaceholderAndMinValue), Is.True);

        checked
        {
            Assert.That(
                () => ValueRange.Create(GetNumber<T>(1), GetNumber<T>(1)).Enumerate().ToArray(),
                Is.EqualTo(new[] { GetNumber<T>(1) }));

            Assert.That(
                () => ValueRange.Create(GetNumber<T>(1), GetNumber<T>(2)).Enumerate().ToArray(),
                Is.EqualTo(new[] { GetNumber<T>(1), GetNumber<T>(2) }));

            Assert.That(
                () => ValueRange.Create(GetNumber<T>(17), GetNumber<T>(23)).Enumerate().ToArray(),
                Is.EqualTo(
                    new[] { GetNumber<T>(17), GetNumber<T>(18), GetNumber<T>(19), GetNumber<T>(20), GetNumber<T>(21), GetNumber<T>(22), GetNumber<T>(23) }));

            if (T.IsNegative(typePlaceholderAndMinValue))
            {
                Assert.That(
                    () => ValueRange.Create(-GetNumber<T>(2), GetNumber<T>(3)).Enumerate().ToArray(),
                    Is.EqualTo(new[] { -GetNumber<T>(2), -GetNumber<T>(1), GetNumber<T>(0), GetNumber<T>(1), GetNumber<T>(2), GetNumber<T>(3) }));
            }
        }
    }

    [Test]
    [TestCase(char.MinValue)]
    [TestCase(sbyte.MinValue)]
    [TestCase(byte.MinValue)]
    [TestCase(short.MinValue)]
    [TestCase(ushort.MinValue)]
    [TestCase(int.MinValue)]
    [TestCase(uint.MinValue)]
    [TestCase(long.MinValue)]
    [TestCase(ulong.MinValue)]
    public void TestToArrayForBinaryIntegerValueSucceeds<T>(T typePlaceholderAndMinValue)
        where T : IBinaryInteger<T>, IConvertible
    {
        Assert.That(T.IsZero(typePlaceholderAndMinValue) | T.IsNegative(typePlaceholderAndMinValue), Is.True);

        var arrayType = typeof(T).MakeArrayType();

        checked
        {
            Assert.That(
                () => ValueRange.Create(GetNumber<T>(1), GetNumber<T>(1)).ToArray(),
                Is.EqualTo(new[] { GetNumber<T>(1) }).And.TypeOf(arrayType));

            Assert.That(
                () => ValueRange.Create(GetNumber<T>(1), GetNumber<T>(2)).ToArray(),
                Is.EqualTo(new[] { GetNumber<T>(1), GetNumber<T>(2) }).And.TypeOf(arrayType));

            Assert.That(
                () => ValueRange.Create(GetNumber<T>(17), GetNumber<T>(23)).ToArray(),
                Is.EqualTo(
                        new[] { GetNumber<T>(17), GetNumber<T>(18), GetNumber<T>(19), GetNumber<T>(20), GetNumber<T>(21), GetNumber<T>(22), GetNumber<T>(23) })
                    .And.TypeOf(arrayType));

            if (T.IsNegative(typePlaceholderAndMinValue))
            {
                Assert.That(
                    () => ValueRange.Create(-GetNumber<T>(2), GetNumber<T>(3)).ToArray(),
                    Is.EqualTo(new[] { -GetNumber<T>(2), -GetNumber<T>(1), GetNumber<T>(0), GetNumber<T>(1), GetNumber<T>(2), GetNumber<T>(3) })
                        .And.TypeOf(arrayType));
            }
        }
    }
#endif

    [Test]
    [TestCase('a', 'z')]
    [TestCase(17, 29)]
    [TestCase("qaz", "wsx")]
    public void TestToValueTuple<T>(T lower, T upper)
        where T : IComparable
    {
        var range = ValueRange.Create(lower, upper);
        var tuple = range.ToValueTuple();
        Assert.That(tuple.Item1, Is.EqualTo(lower));
        Assert.That(tuple.Item2, Is.EqualTo(upper));
    }

#if NET7_0_OR_GREATER
    [Test]
    [TestCase(1f, 3f, 2f)]
    [TestCase(-1.5f, 4f, 1.25f)]
    [TestCase(1d, 3d, 2d)]
    [TestCase(-1.5d, 4d, 1.25d)]
    [TestCase(float.MaxValue, float.MaxValue, float.PositiveInfinity)]
    [TestCase(double.MaxValue, double.MaxValue, double.PositiveInfinity)]
    [TestCaseSource(nameof(TestGetMidpointCases))]
    public void TestGetMidpoint<T>(T lower, T upper, T expected)
        where T : IComparable, IFloatingPoint<T>
    {
        var range = ValueRange.Create(lower, upper);
        Assert.That(() => range.GetMidpoint(), Is.EqualTo(expected));
    }

    [Test]
    [TestCaseSource(nameof(TestGetMidpointWhenOverflowThenThrowsCases))]
    public void TestGetMidpointWhenOverflowThenThrows<T>(T lower, T upper)
        where T : IComparable, IFloatingPoint<T>
    {
        var range = ValueRange.Create(lower, upper);
        Assert.That(() => range.GetMidpoint(), Throws.TypeOf<OverflowException>());
    }
#endif

    //// Private methods

#if NET7_0_OR_GREATER
    private static T GetNumber<T>(uint number)
        where T : IBinaryInteger<T>
    {
        var result = T.Zero;
        for (var index = 0; index < number; index++)
        {
            checked
            {
                result += T.One;
            }
        }

        return result;
    }
#endif
}