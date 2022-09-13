using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumDateTimeExtensions))]
internal sealed class OmnifactotumDateTimeExtensionsTests
{
    [Test]
    [TestCase(DateTimeKind.Local)]
    [TestCase(DateTimeKind.Unspecified)]
    [TestCase(DateTimeKind.Utc)]
    public void TestToFixedString(DateTimeKind kind)
    {
        var value = new DateTime(2001, 2, 3, 7, 8, 9, 456, kind);

        var actualResult = value.ToFixedString();
        Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09"));
    }

    [Test]
    [TestCase(DateTimeKind.Local)]
    [TestCase(DateTimeKind.Unspecified)]
    [TestCase(DateTimeKind.Utc)]
    public void TestToFixedStringWithMilliseconds(DateTimeKind kind)
    {
        var value = new DateTime(2001, 2, 3, 7, 8, 9, 456, kind);

        var actualResult = value.ToFixedStringWithMilliseconds();
        Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09.456"));
    }

    [Test]
    [TestCase(DateTimeKind.Local)]
    [TestCase(DateTimeKind.Unspecified)]
    [TestCase(DateTimeKind.Utc)]
    public void TestToPreciseFixedString(DateTimeKind kind)
    {
        var value = new DateTime(2001, 2, 3, 7, 8, 5, 456, kind);

        var actualResult = value.ToPreciseFixedString();
        Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:05.4560000"));
    }

    [Test]
    public void TestEnsureKind([Values] DateTimeKind kind, [Values] DateTimeKind requiredKind)
    {
        var value = new DateTime(2001, 2, 3, 7, 8, 11, 321, kind);

#if NET5_0_OR_GREATER
        const string ErrorDetails =
            $"\x0020Expression: {{ {nameof(ValueContainer)}.{nameof(ValueContainer.Create)}({nameof(value)}).{nameof(ValueContainer<DateTime>.Value)} }}.";
#else
        const string? ErrorDetails = default(string);
#endif

        Assert.That(
            () => ValueContainer.Create(value).Value.EnsureKind(requiredKind),
            kind == requiredKind
                ? Is.EqualTo(value).With.Property(nameof(DateTime.Kind)).EqualTo(kind)
                : Throws.ArgumentException.With.Message.EqualTo(
                    $@"The specified DateTime value must be of the {requiredKind} kind, but is {value.Kind}.{ErrorDetails}{
                        LocalFactotum.GetArgumentExceptionParameterDetails("value")}"));
    }

    [Test]
    public void TestEnsureUtc([Values] DateTimeKind kind)
    {
        const DateTimeKind RequiredKind = DateTimeKind.Utc;

        var value = new DateTime(2001, 2, 3, 7, 8, 13, 432, kind);

#if NET5_0_OR_GREATER
        const string ErrorDetails =
            $"\x0020Expression: {{ {nameof(ValueContainer)}.{nameof(ValueContainer.Create)}({nameof(value)}).{nameof(ValueContainer<DateTime>.Value)} }}.";
#else
        const string? ErrorDetails = default(string);
#endif

        Assert.That(
            () => ValueContainer.Create(value).Value.EnsureUtc(),
            kind == RequiredKind
                ? Is.EqualTo(value).With.Property(nameof(DateTime.Kind)).EqualTo(kind)
                : Throws.ArgumentException.With.Message.EqualTo(
                    $@"The specified DateTime value must be of the {RequiredKind} kind, but is {value.Kind}.{ErrorDetails}{
                        LocalFactotum.GetArgumentExceptionParameterDetails("value")}"));
    }

    [Test]
    public void TestEnsureLocal([Values] DateTimeKind kind)
    {
        const DateTimeKind RequiredKind = DateTimeKind.Local;

        var value = new DateTime(2001, 2, 3, 7, 8, 13, 432, kind);

#if NET5_0_OR_GREATER
        const string ErrorDetails =
            $"\x0020Expression: {{ {nameof(ValueContainer)}.{nameof(ValueContainer.Create)}({nameof(value)}).{nameof(ValueContainer<DateTime>.Value)} }}.";
#else
        const string? ErrorDetails = default(string);
#endif

        Assert.That(
            () => ValueContainer.Create(value).Value.EnsureLocal(),
            kind == RequiredKind
                ? Is.EqualTo(value).With.Property(nameof(DateTime.Kind)).EqualTo(kind)
                : Throws.ArgumentException.With.Message.EqualTo(
                    $@"The specified DateTime value must be of the {RequiredKind} kind, but is {value.Kind}.{ErrorDetails}{
                        LocalFactotum.GetArgumentExceptionParameterDetails("value")}"));
    }
}