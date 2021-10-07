#nullable enable

using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumNullableDateTimeOffsetExtensions))]
    internal sealed class OmnifactotumNullableDateTimeOffsetExtensionsTests
    {
        private static readonly DateTimeOffset TestValue = new(2001, 2, 3, 7, 8, 9, 456, new TimeSpan(2, 30, 0));

        [Test]
        public void TestToFixedString()
        {
            TestSingleCase(TestValue, "2001-02-03 07:08:09 UTC+02:30");
            TestSingleCase(null, "null");

            static void TestSingleCase(DateTimeOffset? value, string expectedResult)
            {
                var actualResult = value.ToFixedString();
                Assert.That(actualResult, Is.EqualTo(expectedResult));
            }
        }

        [Test]
        public void TestToFixedStringWithMilliseconds()
        {
            TestSingleCase(TestValue, "2001-02-03 07:08:09.456 UTC+02:30");
            TestSingleCase(null, "null");

            static void TestSingleCase(DateTimeOffset? value, string expectedResult)
            {
                var actualResult = value.ToFixedStringWithMilliseconds();
                Assert.That(actualResult, Is.EqualTo(expectedResult));
            }
        }

        [Test]
        public void TestToPreciseFixedString()
        {
            TestSingleCase(TestValue, "2001-02-03 07:08:09.4560000 UTC+02:30");
            TestSingleCase(null, "null");

            static void TestSingleCase(DateTimeOffset? value, string expectedResult)
            {
                var actualResult = value.ToPreciseFixedString();
                Assert.That(actualResult, Is.EqualTo(expectedResult));
            }
        }
    }
}