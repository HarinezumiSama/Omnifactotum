using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumNullableTimeSpanExtensions))]
    internal sealed class OmnifactotumNullableTimeSpanExtensionsTests: OmnifactotumTimeSpanExtensionsTestsBase
    {
        [Test]
        [TestCaseSource(nameof(GetToFixedStringMethodsTestCases), new object?[] { true })]
        public void TestToFixedStringMethods(
            TimeSpan? value,
            string expectedFixedString,
            string expectedFixedStringWithMilliseconds,
            string expectedPreciseFixedString)
        {
            Assert.That(() => value.ToFixedString(), Is.EqualTo(expectedFixedString));
            Assert.That(() => value.ToFixedStringWithMilliseconds(), Is.EqualTo(expectedFixedStringWithMilliseconds));
            Assert.That(() => value.ToPreciseFixedString(), Is.EqualTo(expectedPreciseFixedString));
        }
    }
}