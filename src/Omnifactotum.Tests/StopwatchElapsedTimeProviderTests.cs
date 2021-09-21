#nullable enable

using System;
using System.Threading;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(StopwatchElapsedTimeProvider))]
    internal sealed class StopwatchElapsedTimeProviderTests
    {
        private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(10);

        [Test]
        public void TestConstruction()
        {
            var testee = CreateTestee();

            Assert.That(testee.IsRunning, Is.False);
            Assert.That(testee.Elapsed, Is.EqualTo(TimeSpan.Zero));

            Thread.Sleep(WaitInterval);

            Assert.That(testee.IsRunning, Is.False);
            Assert.That(testee.Elapsed, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void TestStartAndStop()
        {
            var testee = CreateTestee();

            testee.Start();

            Assert.That(testee.IsRunning, Is.True);
            Assert.That(testee.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.Zero));

            Thread.Sleep(WaitInterval);
            var elapsedAfterWait1 = testee.Elapsed;

            Assert.That(testee.IsRunning, Is.True);
            Assert.That(elapsedAfterWait1, Is.GreaterThan(TimeSpan.Zero));

            Thread.Sleep(WaitInterval);
            var elapsedAfterWait2 = testee.Elapsed;

            Assert.That(testee.IsRunning, Is.True);
            Assert.That(elapsedAfterWait2, Is.GreaterThan(elapsedAfterWait1));

            Thread.Sleep(WaitInterval);
            testee.Stop();
            var elapsedWhenStopped = testee.Elapsed;

            Assert.That(testee.IsRunning, Is.False);
            Assert.That(testee.Elapsed, Is.EqualTo(elapsedWhenStopped));

            Thread.Sleep(WaitInterval);

            Assert.That(testee.IsRunning, Is.False);
            Assert.That(testee.Elapsed, Is.EqualTo(elapsedWhenStopped));
        }

        [Test]
        public void TestStartAndReset()
        {
            var testee = CreateTestee();

            testee.Start();
            Thread.Sleep(WaitInterval);

            Assert.That(testee.IsRunning, Is.True);
            Assert.That(testee.Elapsed, Is.GreaterThan(TimeSpan.Zero));

            testee.Reset();

            Assert.That(testee.IsRunning, Is.False);
            Assert.That(testee.Elapsed, Is.EqualTo(TimeSpan.Zero));

            Thread.Sleep(WaitInterval);

            Assert.That(testee.IsRunning, Is.False);
            Assert.That(testee.Elapsed, Is.EqualTo(TimeSpan.Zero));
        }

        private static StopwatchElapsedTimeProvider CreateTestee() => new StopwatchElapsedTimeProvider();
    }
}