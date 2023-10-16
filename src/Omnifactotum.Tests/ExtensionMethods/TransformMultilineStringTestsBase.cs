using System;
using NUnit.Framework;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture]
internal abstract class TransformMultilineStringTestsBase
{
    [Test]
    public void TestTransformMultilineStringWhenInvalidArgumentThenThrows([Values] bool normalizeLineEndings)
        => ExecuteTestTransformMultilineStringWhenInvalidArgumentThenThrows(normalizeLineEndings);

    [Test]
    [TestCase("", "")]
    [TestCase("\n", "[#00:]\n")]
    [TestCase("\r", "[#00:]\r")]
    [TestCase("\r\n", "[#00:]\r\n")]
    [TestCase("\n\r", "[#00:]\n[#01:]\r")]
    [TestCase("X", "[#00:X]")]
    [TestCase("a878-543f", "[#00:a878_543f]")]
    [TestCase("76f-1b7\n", "[#00:76f_1b7]\n")]
    [TestCase("76f-1b7\r", "[#00:76f_1b7]\r")]
    [TestCase("76f-1b7\r\n", "[#00:76f_1b7]\r\n")]
    [TestCase("76f-1b7\n\r", "[#00:76f_1b7]\n[#01:]\r")]
    [TestCase("64aD5-04Ff9\rA-d4\n2C8-a36c\r\nb65\n\r7a315-8D", "[#00:64aD5_04Ff9]\r[#01:A_d4]\n[#02:2C8_a36c]\r\n[#03:b65]\n[#04:]\r[#05:7a315_8D]")]
    public void TestTransformMultilineStringWhenValidArgumentAndWithoutNormalizingLineEndingsThenSucceeds(string input, string expectedResult)
    {
        Assert.That(() => ExecuteTransformMultilineString(input, TransformLine, false), Is.EqualTo(expectedResult));
        Assert.That(() => ExecuteTransformMultilineStringWithDefaultOptionalParameters(input, TransformLine), Is.EqualTo(expectedResult));

        static string TransformLine(string line, int index) => AsInvariant($"[#{index:00}:{line.Replace('-', '_')}]");
    }

    [Test]
    [TestCase("", new[] { "" }, false)]
    [TestCase("\n", new[] { "[#00:]" }, true)]
    [TestCase("\r", new[] { "[#00:]" }, true)]
    [TestCase("\r\n", new[] { "[#00:]" }, true)]
    [TestCase("\n\r", new[] { "[#00:]", "[#01:]" }, true)]
    [TestCase("X", new[] { "[#00:X]" }, false)]
    [TestCase("a878-543f", new[] { "[#00:a878_543f]" }, false)]
    [TestCase("76f-1b7\n", new[] { "[#00:76f_1b7]" }, true)]
    [TestCase("76f-1b7\r", new[] { "[#00:76f_1b7]" }, true)]
    [TestCase("76f-1b7\r\n", new[] { "[#00:76f_1b7]" }, true)]
    [TestCase("76f-1b7\n\r", new[] { "[#00:76f_1b7]", "[#01:]" }, true)]
    [TestCase(
        "64aD5-04Ff9\rA-d4\n2C8-a36c\r\nb65\n\r7a315-8D",
        new[]
        {
            "[#00:64aD5_04Ff9]",
            "[#01:A_d4]",
            "[#02:2C8_a36c]",
            "[#03:b65]",
            "[#04:]",
            "[#05:7a315_8D]"
        },
        false)]
    [TestCase(
        "64aD5-04Ff9\rA-d4\n2C8-a36c\r\nb65\n\r7a315-8D\r",
        new[]
        {
            "[#00:64aD5_04Ff9]",
            "[#01:A_d4]",
            "[#02:2C8_a36c]",
            "[#03:b65]",
            "[#04:]",
            "[#05:7a315_8D]"
        },
        true)]
    public void TestTransformMultilineStringWhenValidArgumentAndWithNormalizingLineEndingsThenSucceeds(
        string input,
        string[] expectedResultParts,
        bool expectedTrailingLineEnding)
    {
        var expectedResult = expectedResultParts.Join(Environment.NewLine) + (expectedTrailingLineEnding ? Environment.NewLine : string.Empty);

        Assert.That(() => ExecuteTransformMultilineString(input, TransformLine, true), Is.EqualTo(expectedResult));

        static string TransformLine(string line, int index) => AsInvariant($"[#{index:00}:{line.Replace('-', '_')}]");
    }

    protected virtual void ExecuteTestTransformMultilineStringWhenInvalidArgumentThenThrows(bool normalizeLineEndings)
    {
        Assert.That(
            () => ExecuteTransformMultilineString(string.Empty, null!, normalizeLineEndings),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("transformLine"));

        Assert.That(
            () => ExecuteTransformMultilineString("581fa40b021b4283964642edceea808b", null!, normalizeLineEndings),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo("transformLine"));
    }

    protected abstract string ExecuteTransformMultilineString(string input, Func<string, int, string> transformLine, bool normalizeLineEndings);

    protected abstract string ExecuteTransformMultilineStringWithDefaultOptionalParameters(string input, Func<string, int, string> transformLine);
}