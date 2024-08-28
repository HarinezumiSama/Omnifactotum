using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumNullableCharExtensions))]
internal sealed class OmnifactotumNullableCharExtensionsTests
{
    [Test]
    [TestCase(null, "null")]
    [TestCase('\x0000', "'\0'")]
    [TestCase('\x0001', "'\x0001'")]
    [TestCase('\x001F', "'\x001F'")]
    [TestCase('\x0020', "'\x0020'")]
    [TestCase('4', "'4'")]
    [TestCase('.', "'.'")]
    [TestCase('a', "'a'")]
    [TestCase('Q', "'Q'")]
    [TestCase('Ώ', "'Ώ'")]
    [TestCase('β', "'β'")]
    [TestCase('Ы', "'Ы'")]
    [TestCase('ъ', "'ъ'")]
    [TestCase('/', "'/'")]
    [TestCase('≠', "'≠'")]
    [TestCase('`', "'`'")]
    [TestCase('"', "'\"'")]
    [TestCase('«', "'«'")]
    [TestCase('»', "'»'")]
    [TestCase('\'', "''''")]
    public void TestToUIString(char? value, string expectedResult) => Assert.That(() => value.ToUIString(), Is.EqualTo(expectedResult));
}