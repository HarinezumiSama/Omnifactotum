using System;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
    public sealed class OmnifactotumDateTimeOffsetExtensionsTests
    {
        #region Tests

        [Test]
        public void TestToFixedString()
        {
            var dateTimeOffset = new DateTimeOffset(2001, 2, 3, 7, 8, 9, 123, new TimeSpan(2, 30, 0));

            var actualResult = dateTimeOffset.ToFixedString();
            Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09 UTC+02:30"));
        }

        [Test]
        public void TestToPreciseFixedString()
        {
            var dateTimeOffset = new DateTimeOffset(2001, 2, 3, 7, 8, 9, 123, new TimeSpan(2, 30, 0));

            var actualResult = dateTimeOffset.ToPreciseFixedString();
            Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09.1230000 UTC+02:30"));
        }

        #endregion
    }
}