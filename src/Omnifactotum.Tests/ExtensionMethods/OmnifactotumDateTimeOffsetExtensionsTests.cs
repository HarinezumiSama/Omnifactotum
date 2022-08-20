#nullable enable

using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumDateTimeOffsetExtensions))]
    internal sealed class OmnifactotumDateTimeOffsetExtensionsTests
    {
        [Test]
        public void TestToFixedString()
        {
            var value = new DateTimeOffset(2001, 2, 3, 7, 8, 9, 456, new TimeSpan(2, 30, 0));
            Assert.That(() => value.ToFixedString(), Is.EqualTo("2001-02-03 07:08:09 UTC+02:30"));
        }

        [Test]
        public void TestToFixedStringWithMilliseconds()
        {
            var value = new DateTimeOffset(2001, 2, 3, 7, 8, 9, 456, new TimeSpan(2, 30, 0));
            Assert.That(() => value.ToFixedStringWithMilliseconds(), Is.EqualTo("2001-02-03 07:08:09.456 UTC+02:30"));
        }

        [Test]
        public void TestToPreciseFixedString()
        {
            var value = new DateTimeOffset(2001, 2, 3, 7, 8, 9, 456, new TimeSpan(2, 30, 0));
            Assert.That(() => value.ToPreciseFixedString(), Is.EqualTo("2001-02-03 07:08:09.4560000 UTC+02:30"));
        }
    }
}