using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(CaseInsensitiveString))]
internal sealed class CaseInsensitiveStringTests
{
    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("qWerTy")]
    [TestCase("a53d7ef8a3ec403296F170342a9f70bf")]
    public void TestConstruction(string? value)
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
        CaseInsensitiveString testee = value;
        Assert.That(() => testee.Value, Is.EqualTo(value));

        string? valueBack = testee;
        Assert.That(valueBack, Is.EqualTo(value));
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
    public void TestEquality(string? value1, string? value2, AssertEqualityExpectation equalityExpectation)
    {
        var testee1 = new CaseInsensitiveString(value1);
        var testee2 = new CaseInsensitiveString(value2);
        NUnitFactotum.AssertEquality(testee1, testee2, equalityExpectation, AssertEqualityOperatorExpectation.MustDefine);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("\t")]
    [TestCase("Hello-World")]
    [TestCase("fbb097b992d144E1AC898e18a7d1ba39")]
    [TestCase("Have a good day! Bonne journée ! 좋은 하루 보내세요! Гарного дня! Eigðu góðan dag!")]
    public void TestToString(string? value)
    {
        var testee = new CaseInsensitiveString(value);
        Assert.That(() => testee.ToString(), Is.EqualTo(value));
    }
}