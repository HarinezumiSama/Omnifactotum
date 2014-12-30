using System;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
    public sealed class OmnifactotumDateTimeExtensionsTests
    {
        #region Tests

        [Test]
        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Unspecified)]
        [TestCase(DateTimeKind.Utc)]
        public void TestToFixedString(DateTimeKind kind)
        {
            var dateTime = new DateTime(2001, 2, 3, 7, 8, 9, 123);

            var actualResult = dateTime.ToFixedString();
            Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09"));
        }

        [Test]
        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Unspecified)]
        [TestCase(DateTimeKind.Utc)]
        public void TestToPreciseFixedString(DateTimeKind kind)
        {
            var dateTime = new DateTime(2001, 2, 3, 7, 8, 9, 123);

            var actualResult = dateTime.ToPreciseFixedString();
            Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09.1230000"));
        }

        #endregion
    }
}