using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(CaseInsensitiveString))]
internal sealed class CaseInsensitiveStringTests
{
    [Test]
    public void TestStaticFields()
    {
        Assert.That(CaseInsensitiveString.Comparer, Is.SameAs(StringComparer.OrdinalIgnoreCase));
        Assert.That(CaseInsensitiveString.Empty.Value, Is.EqualTo(string.Empty));
    }

    [Test]
    public void TestParameterlessConstruction()
    {
        Assert.That(() => new CaseInsensitiveString().Value, Is.EqualTo(string.Empty));
        Assert.That(() => default(CaseInsensitiveString).Value, Is.EqualTo(string.Empty));
        Assert.That(() => (new CaseInsensitiveString[1])[0].Value, Is.EqualTo(string.Empty));
    }

    [Test]
    public void TestConstructionWhenInvalidArgumentThenThrows() => Assert.That(() => new CaseInsensitiveString(null!), Throws.ArgumentNullException);

    [Test]
    [TestCase("")]
    [TestCase("qWerTy")]
    [TestCase("a53d7ef8a3ec403296F170342a9f70bf")]
    [TestCase("Have a good day! Bonne journée ! 좋은 하루 보내세요! Гарного дня! Eigðu góðan dag!")]
    public void TestConstructionWhenValidArgumentThenSucceeds(string value)
    {
        var testee = new CaseInsensitiveString(value);
        Assert.That(() => testee.Value, Is.EqualTo(value));
    }

    [Test]
    public void TestPropertyAccess() => NUnitFactotum.For<CaseInsensitiveString>.AssertReadableWritable(static obj => obj.Value, PropertyAccessMode.ReadOnly);

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("\t")]
    [TestCase("Hello-World")]
    [TestCase("fbb097b992d144E1AC898e18a7d1ba39")]
    [TestCase("Have a good day! Bonne journée ! 좋은 하루 보내세요! Гарного дня! Eigðu góðan dag!")]
    public void TestConversionOperators(string? value)
    {
        // Conversion to nullable
        {
            CaseInsensitiveString? testee = value;
            Assert.That(() => testee?.Value, Is.EqualTo(value));

            string? valueBack = testee;
            Assert.That(valueBack, Is.EqualTo(value));
        }

        // Regular conversion
        {
            if (value is null)
            {
                Assert.That(() => (CaseInsensitiveString)value!, Throws.ArgumentNullException);
            }
            else
            {
                CaseInsensitiveString testee = value;
                Assert.That(() => testee.Value, Is.EqualTo(value));

                string valueBack = testee;
                Assert.That(valueBack, Is.EqualTo(value));
            }
        }
    }

    [Test]
    [TestCase(null, null, AssertEqualityExpectation.EqualAndCannotBeSame)]
    [TestCase(null, "", AssertEqualityExpectation.NotEqual)]
    [TestCase(null, "\x0020", AssertEqualityExpectation.NotEqual)]
    [TestCase(null, "2076092343F146AB98988C33745e2eb8", AssertEqualityExpectation.NotEqual)]
    [TestCase("", "\x0020", AssertEqualityExpectation.NotEqual)]
    [TestCase("\t", "\x0020", AssertEqualityExpectation.NotEqual)]
    [TestCase("", "2076092343F146AB98988C33745e2eb8", AssertEqualityExpectation.NotEqual)]
    [TestCase("", "", AssertEqualityExpectation.EqualAndCannotBeSame)]
    [TestCase(
        "Have a good day! Bonne journée ! 좋은 하루 보내세요! Гарного дня! Eigðu góðan dag!",
        "HAVE a gOod day! bonnE jOurnÉE ! 좋은 하루 보내세요! гАрнОгО Дня! eIGÐU GóðaN dAg!",
        AssertEqualityExpectation.EqualAndCannotBeSame)]
    [TestCase(
        "have A GoOD DAY! BONNe JoURNée ! 좋은 하루 보내세요! ГаРНоГо дНЯ! Eigðu gÓÐAn DaG!",
        "HAVE a gOod day! bonnE jOurnÉE ! 좋은 하루 보내세요! гАрнОгО Дня! eIGÐU GóðaN dAg!",
        AssertEqualityExpectation.EqualAndCannotBeSame)]
    [SuppressMessage("ReSharper", "InvertIf")]
    public void TestEquality(string? value1, string? value2, AssertEqualityExpectation equalityExpectation)
    {
        // Nullable
        {
            var testee1 = CaseInsensitiveString.Create(value1);
            var testee2 = CaseInsensitiveString.Create(value2);
            NUnitFactotum.AssertEquality(testee1, testee2, equalityExpectation, AssertEqualityOperatorExpectation.MustDefine);
        }

        // Regular
        if (value1 is not null && value2 is not null)
        {
            var testee1 = new CaseInsensitiveString(value1);
            var testee2 = new CaseInsensitiveString(value2);
            NUnitFactotum.AssertEquality(testee1, testee2, equalityExpectation, AssertEqualityOperatorExpectation.MustDefine);
        }
    }

    [Test]
    [TestCase("")]
    [TestCase("\t")]
    [TestCase("Hello-World")]
    [TestCase("fbb097b992d144E1AC898e18a7d1ba39")]
    [TestCase("Have a good day! Bonne journée ! 좋은 하루 보내세요! Гарного дня! Eigðu góðan dag!")]
    public void TestToString(string value)
    {
        var testee = new CaseInsensitiveString(value);
        Assert.That(() => testee.ToString(), Is.EqualTo(value));
    }
}