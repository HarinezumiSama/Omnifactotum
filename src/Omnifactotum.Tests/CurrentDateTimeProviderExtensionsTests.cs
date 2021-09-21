#nullable enable

using System;
using Moq;
using NUnit.Framework;
using Omnifactotum.Abstractions;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(CurrentDateTimeProviderExtensions))]
    internal sealed class CurrentDateTimeProviderExtensionsTests
    {
        [Test]
        [Category(TestCategory.Positive)]
        public void TestGetLocalTimeWhenValidArgumentsThenSucceeds()
        {
            var currentTime = new DateTime(2021, 9, 20, 19, 10, 36, DateTimeKind.Utc);

            var currentDateTimeProviderMock = new Mock<ICurrentDateTimeProvider>(MockBehavior.Strict);
            currentDateTimeProviderMock.Setup(provider => provider.GetUtcTime()).Returns(currentTime);

            var actualValue = currentDateTimeProviderMock.Object.GetLocalTime();
            Assert.That(actualValue.Kind, Is.EqualTo(DateTimeKind.Local));
            Assert.That(actualValue, Is.EqualTo(currentTime.ToLocalTime()));
        }

        [Test]
        [Category(TestCategory.Negative)]
        public void TestGetLocalTimeWhenInvalidArgumentThenThrows()
            => Assert.That(() => default(ICurrentDateTimeProvider)!.GetLocalTime(), Throws.ArgumentNullException);

        [Test]
        [Category(TestCategory.Negative)]
        public void TestGetLocalTimeWhenProviderReturnsNonUtcThenThrows(
            [Values(DateTimeKind.Unspecified, DateTimeKind.Local)] DateTimeKind dateTimeKind)
        {
            var currentTime = new DateTime(2021, 9, 20, 19, 27, 13, dateTimeKind);

            var currentDateTimeProviderMock = new Mock<ICurrentDateTimeProvider>(MockBehavior.Strict);
            currentDateTimeProviderMock.Setup(provider => provider.GetUtcTime()).Returns(currentTime);

            Assert.That(() => currentDateTimeProviderMock.Object.GetLocalTime(), Throws.InvalidOperationException);
        }
    }
}