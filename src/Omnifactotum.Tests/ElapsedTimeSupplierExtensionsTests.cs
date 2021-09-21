#nullable enable

using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Omnifactotum.Abstractions;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ElapsedTimeProviderExtensions))]
    internal sealed class ElapsedTimeSupplierExtensionsTests
    {
        [Test]
        public void TestRestart()
        {
            var elapsedTimeProviderMock = new Mock<IElapsedTimeProvider>(MockBehavior.Strict);

            var calledMethods = new List<CalledMethod>();

            elapsedTimeProviderMock
                .Setup(provider => provider.Reset())
                .Callback(() => calledMethods.Add(CalledMethod.Reset));

            elapsedTimeProviderMock
                .Setup(provider => provider.Start())
                .Callback(() => calledMethods.Add(CalledMethod.Start));

            elapsedTimeProviderMock.Object.Restart();

            Assert.That(calledMethods, Is.EqualTo(new[] { CalledMethod.Reset, CalledMethod.Start }));
            elapsedTimeProviderMock.Verify(provider => provider.Reset(), Times.Once);
            elapsedTimeProviderMock.Verify(provider => provider.Start(), Times.Once);
        }

        private enum CalledMethod
        {
            Reset,
            Start
        }
    }
}