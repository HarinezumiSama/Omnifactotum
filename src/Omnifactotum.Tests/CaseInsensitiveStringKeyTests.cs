using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(CaseInsensitiveStringKey))]
internal sealed class CaseInsensitiveStringKeyTests
{
    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("qWerTy")]
    [TestCase("a53d7ef8a3ec403296F170342a9f70bf")]
    public void TestConstruction(string? value)
    {
        var testee = new CaseInsensitiveStringKey(value);
        Assert.That(() => testee.Value, Is.EqualTo(value));
    }

    [Test]
    public void TestPropertyAccess() => NUnitFactotum.For<CaseInsensitiveStringKey>.AssertReadableWritable(obj => obj.Value, PropertyAccessMode.ReadOnly);

    [Test]
    [TestCase(null, null, AssertEqualityExpectation.EqualAndCannotBeSame)]
    [TestCase(null, "", AssertEqualityExpectation.NotEqual)]
    [TestCase(null, "\x0020", AssertEqualityExpectation.NotEqual)]
    [TestCase(null, "2076092343F146AB98988C33745e2eb8", AssertEqualityExpectation.NotEqual)]
    [TestCase("", "\x0020", AssertEqualityExpectation.NotEqual)]
    [TestCase("\t", "\x0020", AssertEqualityExpectation.NotEqual)]
    [TestCase("", "2076092343F146AB98988C33745e2eb8", AssertEqualityExpectation.NotEqual)]
    [TestCase("", "", AssertEqualityExpectation.EqualAndCannotBeSame)]
    [TestCase("Hello-World", "Hello-World", AssertEqualityExpectation.EqualAndCannotBeSame)]
    [TestCase("Hello-World", "hello-world", AssertEqualityExpectation.EqualAndCannotBeSame)]
    public void TestEquality(string? value1, string? value2, AssertEqualityExpectation equalityExpectation)
    {
        var testee1 = new CaseInsensitiveStringKey(value1);
        var testee2 = new CaseInsensitiveStringKey(value2);
        NUnitFactotum.AssertEquality(testee1, testee2, equalityExpectation, AssertEqualityOperatorExpectation.MustDefine);
    }

    [Test]
    [TestCase(null, @"CaseInsensitiveStringKey { Value = null }")]
    [TestCase("", @"CaseInsensitiveStringKey { Value = """" }")]
    [TestCase("Hello-World", @"CaseInsensitiveStringKey { Value = ""Hello-World"" }")]
    [TestCase("fbb097b992d144E1AC898e18a7d1ba39", @"CaseInsensitiveStringKey { Value = ""fbb097b992d144E1AC898e18a7d1ba39"" }")]
    public void TestToString(string? value, string expectedResult)
    {
        var testee = new CaseInsensitiveStringKey(value);
        Assert.That(() => testee.ToString(), Is.EqualTo(expectedResult));
    }
}