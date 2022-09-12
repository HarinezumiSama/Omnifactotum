using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumTimeSpanExtensions))]
internal sealed class OmnifactotumTimeSpanExtensionsTests : OmnifactotumTimeSpanExtensionsTestsBase
{
    [Test]
    public void TestMultiply()
    {
        const decimal Coefficient = 1.4m;

        var ts = new TimeSpan(1, 2, 39);
        var result = ts.Multiply(Coefficient);
        Assert.That(result.Ticks, Is.EqualTo((long)(ts.Ticks * Coefficient)));
    }

    [Test]
    public void TestDivideWhenValidArgumentThenSucceeds()
    {
        const decimal Coefficient = 1.2m;

        var ts = new TimeSpan(2, 3, 57);
        var result = ts.Divide(Coefficient);
        Assert.That(result.Ticks, Is.EqualTo((long)(ts.Ticks / Coefficient)));
    }

    [Test]
    public void TestDivideWhenInvalidArgumentThenThrows()
    {
        var ts = new TimeSpan(3, 2, 1);
        Assert.That(() => ts.Divide(0m), Throws.ArgumentException);
    }

    [Test]
    [TestCaseSource(nameof(GetToFixedStringMethodsTestCases), new object?[] { false })]
    public void TestToFixedStringMethods(
        TimeSpan value,
        string expectedFixedString,
        string expectedFixedStringWithMilliseconds,
        string expectedPreciseFixedString)
    {
        Assert.That(() => value.ToFixedString(), Is.EqualTo(expectedFixedString));
        Assert.That(() => value.ToFixedStringWithMilliseconds(), Is.EqualTo(expectedFixedStringWithMilliseconds));
        Assert.That(() => value.ToPreciseFixedString(), Is.EqualTo(expectedPreciseFixedString));
    }
}