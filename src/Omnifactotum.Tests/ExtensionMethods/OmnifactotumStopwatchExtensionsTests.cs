using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumStopwatchExtensions))]
internal sealed class OmnifactotumStopwatchExtensionsTests
{
    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(10);

    [Test]
    public void TestGetStoppedElapsedWhenInvalidArgumentThenThrows()
        => Assert.That(() => default(Stopwatch)!.GetStoppedElapsed(), Throws.ArgumentNullException);

    [Test]
    public void TestGetStoppedElapsedWhenValidArgumentThenSucceeds()
    {
        var stopwatch = new Stopwatch();

        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatch.GetStoppedElapsed(), Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatch.IsRunning, Is.False);

        Thread.Sleep(WaitInterval);

        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatch.GetStoppedElapsed(), Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatch.IsRunning, Is.False);

        stopwatch.Start();
        Assert.That(() => stopwatch.IsRunning, Is.True);

        Thread.Sleep(WaitInterval);

        var stoppedElapsed1 = stopwatch.GetStoppedElapsed();
        Assert.That(() => stoppedElapsed1, Is.GreaterThanOrEqualTo(WaitInterval));

        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatch.GetStoppedElapsed(), Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(stoppedElapsed1));

        Thread.Sleep(WaitInterval);

        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatch.GetStoppedElapsed(), Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(stoppedElapsed1));

        stopwatch.Start();

        Thread.Sleep(WaitInterval);

        var stoppedElapsed2 = stopwatch.GetStoppedElapsed();
        Assert.That(() => stoppedElapsed2, Is.GreaterThanOrEqualTo(WaitInterval));

        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(stoppedElapsed2));
        Assert.That(() => stopwatch.GetStoppedElapsed(), Is.EqualTo(stoppedElapsed2));
        Assert.That(() => stopwatch.IsRunning, Is.False);
        Assert.That(() => stopwatch.Elapsed, Is.EqualTo(stoppedElapsed2));
    }
}