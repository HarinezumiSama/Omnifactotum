using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumNullableBooleanExtensions))]
internal sealed class OmnifactotumNullableBooleanExtensionsTests
{
    [Test]
    [TestCase(null, "Dunno", "Yup", "Nope", "Dunno")]
    [TestCase(null, null, "Yup", "Nope", null)]
    [TestCase(null, "", "Yup", "Nope", "")]
    [TestCase(true, "Dunno", "Yup", "Nope", "Yup")]
    [TestCase(true, "Dunno", null, "Nope", null)]
    [TestCase(true, "Dunno", "", "Nope", "")]
    [TestCase(false, "Dunno", "Yup", "Nope", "Nope")]
    [TestCase(false, "Dunno", "Yup", null, null)]
    [TestCase(false, "Dunno", "Yup", "", "")]
    public void TestToStringOverloadsSucceed(
        bool? value,
        string? noValueString,
        string? trueValueString,
        string? falseValueString,
        string? expectedResult)
    {
        Assert.That(new[] { noValueString, trueValueString, falseValueString }, Is.Unique);

        var actualResult1 = value.ToString(noValueString, trueValueString, falseValueString);
        Assert.That(actualResult1, Is.EqualTo(expectedResult));

        var actualResult2 = value.ToString(() => noValueString, () => trueValueString, () => falseValueString);
        Assert.That(actualResult2, Is.EqualTo(expectedResult));
    }

    [Test]
    public void TestToStringWithNullProviderThrows()
    {
        Assert.That(
            () => ((bool?)null).ToString(null!, () => "Yes", () => "No"),
            Throws.ArgumentNullException
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("noValueProvider"));

        Assert.That(
            () => ((bool?)true).ToString(() => "Dunno", null!, () => "No"),
            Throws.ArgumentNullException
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("trueValueProvider"));

        Assert.That(
            () => ((bool?)false).ToString(() => "Dunno", () => "Yes", null!),
            Throws.ArgumentNullException
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("falseValueProvider"));
    }
}