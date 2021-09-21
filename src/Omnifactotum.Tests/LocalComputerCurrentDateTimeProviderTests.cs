#nullable enable

using System;
using System.Threading;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(LocalComputerCurrentDateTimeProvider))]
    internal sealed class LocalComputerCurrentDateTimeProviderTests
    {
        private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(10);

        [Test]
        public void TestGetUtcTime()
        {
            var testee = CreateTestee();

            var actualValue1 = testee.GetUtcTime();
            Assert.That(actualValue1.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(actualValue1, Is.LessThanOrEqualTo(DateTime.UtcNow));

            Thread.Sleep(WaitInterval);

            var actualValue2 = testee.GetUtcTime();
            Assert.That(actualValue2.Kind, Is.EqualTo(DateTimeKind.Utc));
            Assert.That(actualValue2, Is.LessThanOrEqualTo(DateTime.UtcNow));

            Assert.That(actualValue2, Is.GreaterThan(actualValue1));
        }

        private static LocalComputerCurrentDateTimeProvider CreateTestee() => new LocalComputerCurrentDateTimeProvider();
    }
}