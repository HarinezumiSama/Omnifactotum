using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumCharExtensions))]
internal sealed class OmnifactotumCharExtensionsTests
{
    [Test]
    [TestCase('\x0000', "'\x0000'")]
    [TestCase('\x0020', "'\x0020'")]
    [TestCase('4', "'4'")]
    [TestCase('.', "'.'")]
    [TestCase('a', "'a'")]
    [TestCase('Q', "'Q'")]
    [TestCase('/', "'/'")]
    [TestCase('`', "'`'")]
    [TestCase('"', "'\"'")]
    [TestCase('\'', "''''")]
    public void TestToUIString(char value, string expectedResult) => Assert.That(() => value.ToUIString(), Is.EqualTo(expectedResult));
}