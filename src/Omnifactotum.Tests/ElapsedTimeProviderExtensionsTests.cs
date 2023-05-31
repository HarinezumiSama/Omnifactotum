using System;
using System.Collections.Generic;
using System.Threading;
using Moq;
using NUnit.Framework;
using Omnifactotum.Abstractions;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(ElapsedTimeProviderExtensions))]
internal sealed class ElapsedTimeProviderExtensionsTests
{
    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(10);

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

    [Test]
    public void TestGetStoppedElapsedWhenInvalidArgumentThenThrows()
        => Assert.That(() => default(IElapsedTimeProvider)!.GetStoppedElapsed(), Throws.ArgumentNullException);

    [Test]
    public void TestGetStoppedElapsedWhenValidArgumentThenSucceeds()
    {
        var stopwatchElapsedTimeProvider = new StopwatchElapsedTimeProvider();

        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatchElapsedTimeProvider.GetStoppedElapsed(), Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);

        Thread.Sleep(WaitInterval);

        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatchElapsedTimeProvider.GetStoppedElapsed(), Is.EqualTo(TimeSpan.Zero));
        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);

        stopwatchElapsedTimeProvider.Start();
        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.True);

        Thread.Sleep(WaitInterval);

        var stoppedElapsed1 = stopwatchElapsedTimeProvider.GetStoppedElapsed();
        Assert.That(() => stoppedElapsed1, Is.GreaterThanOrEqualTo(WaitInterval));

        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatchElapsedTimeProvider.GetStoppedElapsed(), Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(stoppedElapsed1));

        Thread.Sleep(WaitInterval);

        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatchElapsedTimeProvider.GetStoppedElapsed(), Is.EqualTo(stoppedElapsed1));
        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(stoppedElapsed1));

        stopwatchElapsedTimeProvider.Start();

        Thread.Sleep(WaitInterval);

        var stoppedElapsed2 = stopwatchElapsedTimeProvider.GetStoppedElapsed();
        Assert.That(() => stoppedElapsed2, Is.GreaterThanOrEqualTo(WaitInterval));

        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(stoppedElapsed2));
        Assert.That(() => stopwatchElapsedTimeProvider.GetStoppedElapsed(), Is.EqualTo(stoppedElapsed2));
        Assert.That(() => stopwatchElapsedTimeProvider.IsRunning, Is.False);
        Assert.That(() => stopwatchElapsedTimeProvider.Elapsed, Is.EqualTo(stoppedElapsed2));
    }

    private enum CalledMethod
    {
        Reset,
        Start
    }
}