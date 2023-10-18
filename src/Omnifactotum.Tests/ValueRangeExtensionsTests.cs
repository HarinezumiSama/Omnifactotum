using System;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(ValueRangeExtensions))]
internal sealed class ValueRangeExtensionsTests
{
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
    public void TestEnumerateForBinaryIntegerValueSucceeds()
    {
        Assert.That(
            () => ValueRange.Create(1, 1).Enumerate().ToArray(),
            Is.EqualTo(new[] { 1 }));

        Assert.That(
            () => ValueRange.Create(1, 2).Enumerate().ToArray(),
            Is.EqualTo(new[] { 1, 2 }));

        Assert.That(
            () => ValueRange.Create(17, 23).Enumerate().ToArray(),
            Is.EqualTo(new[] { 17, 18, 19, 20, 21, 22, 23 }));
    }

    [Test]
    public void TestToArrayForBinaryIntegerValueSucceeds()
    {
        Assert.That(
            () => ValueRange.Create(1, 1).ToArray(),
            Is.EqualTo(new[] { 1 }).And.TypeOf<int[]>());

        Assert.That(
            () => ValueRange.Create(1, 2).ToArray(),
            Is.EqualTo(new[] { 1, 2 }).And.TypeOf<int[]>());

        Assert.That(
            () => ValueRange.Create(17, 23).ToArray(),
            Is.EqualTo(new[] { 17, 18, 19, 20, 21, 22, 23 }).And.TypeOf<int[]>());
    }
#endif
}