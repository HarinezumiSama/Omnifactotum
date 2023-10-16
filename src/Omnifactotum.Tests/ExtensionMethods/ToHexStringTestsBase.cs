using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture]
internal abstract class ToHexStringTestsBase
{
    [Test]
    public void TestToHexStringWhenResultWouldBeTooLongThenThrows([Values] bool upperCase)
        => Assert.That(() => ExecuteToHexString(new byte[46_997], new string('/', 46_021), upperCase), Throws.TypeOf<OverflowException>());

    [Test]
    [TestCase(new byte[0], "")]
    [TestCase(new byte[] { 0xA9 }, "a9")]
    [TestCase(new byte[] { 0xA9, 0x7E }, "a97e")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        "031425364758697a8b9cadbecfd0e1f2")]
    public void TestToHexStringWhenValidArgumentsAndDefaultOptionalArgumentsThenSucceeds(byte[] bytes, string expectedValue)
        => Assert.That(() => ExecuteToHexStringWithDefaultOptionalParameters(bytes), Is.EqualTo(expectedValue));

    [Test]
    //// No items
    [TestCase(new byte[0], null, false, "")]
    [TestCase(new byte[0], null, true, "")]
    [TestCase(new byte[0], "", false, "")]
    [TestCase(new byte[0], "", true, "")]
    [TestCase(new byte[0], ":", false, "")]
    [TestCase(new byte[0], ":", true, "")]
    [TestCase(new byte[0], "0145cfe513704dfe8e774c2683311b83", false, "")]
    [TestCase(new byte[0], "0145cfe513704dfe8e774c2683311b83", true, "")]
    //// Single item
    [TestCase(new byte[] { 0xA9 }, null, false, "a9")]
    [TestCase(new byte[] { 0xA9 }, null, true, "A9")]
    [TestCase(new byte[] { 0xA9 }, "", false, "a9")]
    [TestCase(new byte[] { 0xA9 }, "", true, "A9")]
    [TestCase(new byte[] { 0xA9 }, ":", false, "a9")]
    [TestCase(new byte[] { 0xA9 }, ":", true, "A9")]
    [TestCase(new byte[] { 0xA9 }, "ea944d306f4646fbab654f76b4717e5f", false, "a9")]
    [TestCase(new byte[] { 0xA9 }, "ea944d306f4646fbab654f76b4717e5f", true, "A9")]
    //// Two items
    [TestCase(new byte[] { 0xA9, 0x7E }, null, false, "a97e")]
    [TestCase(new byte[] { 0xA9, 0x7E }, null, true, "A97E")]
    [TestCase(new byte[] { 0xA9, 0x7E }, "", false, "a97e")]
    [TestCase(new byte[] { 0xA9, 0x7E }, "", true, "A97E")]
    [TestCase(new byte[] { 0xA9, 0x7E }, ":", false, "a9:7e")]
    [TestCase(new byte[] { 0xA9, 0x7E }, ":", true, "A9:7E")]
    [TestCase(new byte[] { 0xA9, 0x7E }, "^*%", false, "a9^*%7e")]
    [TestCase(new byte[] { 0xA9, 0x7E }, "^*%", true, "A9^*%7E")]
    //// 16 items with various hex digits
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        null,
        false,
        "031425364758697a8b9cadbecfd0e1f2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        null,
        true,
        "031425364758697A8B9CADBECFD0E1F2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        "",
        false,
        "031425364758697a8b9cadbecfd0e1f2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        "",
        true,
        "031425364758697A8B9CADBECFD0E1F2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        ",",
        false,
        "03,14,25,36,47,58,69,7a,8b,9c,ad,be,cf,d0,e1,f2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        ",",
        true,
        "03,14,25,36,47,58,69,7A,8B,9C,AD,BE,CF,D0,E1,F2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        ":",
        false,
        "03:14:25:36:47:58:69:7a:8b:9c:ad:be:cf:d0:e1:f2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        ":",
        true,
        "03:14:25:36:47:58:69:7A:8B:9C:AD:BE:CF:D0:E1:F2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        "(/!)",
        false,
        "03(/!)14(/!)25(/!)36(/!)47(/!)58(/!)69(/!)7a(/!)8b(/!)9c(/!)ad(/!)be(/!)cf(/!)d0(/!)e1(/!)f2")]
    [TestCase(
        new byte[] { 0x03, 0x14, 0x25, 0x36, 0x47, 0x58, 0x69, 0x7A, 0x8B, 0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2 },
        "(/!)",
        true,
        "03(/!)14(/!)25(/!)36(/!)47(/!)58(/!)69(/!)7A(/!)8B(/!)9C(/!)AD(/!)BE(/!)CF(/!)D0(/!)E1(/!)F2")]
    public void TestToHexStringWhenValidArgumentsThenSucceeds(byte[] bytes, string? separator, bool upperCase, string expectedValue)
        => Assert.That(() => ExecuteToHexString(bytes, separator, upperCase), Is.EqualTo(expectedValue));

    protected abstract string ExecuteToHexString(byte[] bytes, string? separator, bool upperCase);

    protected abstract string ExecuteToHexStringWithDefaultOptionalParameters(byte[] bytes);
}