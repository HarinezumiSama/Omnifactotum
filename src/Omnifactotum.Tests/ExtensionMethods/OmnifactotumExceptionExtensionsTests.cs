using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumExceptionExtensions))]
internal sealed class OmnifactotumExceptionExtensionsTests
{
    [Test]
    public void TestIsFatalWhenNullArgumentIsPassedThenReturnsFalse()
    {
        const Exception? NullException = null;

        Assert.That(() => NullException.IsFatal(), Is.False);
    }

    [Test]
    [TestCase(typeof(OperationCanceledException), true)]
    [TestCase(typeof(TaskCanceledException), true)]
    [TestCase(typeof(OutOfMemoryException), true)]
    [TestCase(typeof(StackOverflowException), true)]
    [TestCase(typeof(IOException), false)]
    [TestCase(typeof(InvalidOperationException), false)]
    [TestCase(typeof(Exception), false)]
    [TestCase(typeof(ArgumentException), false)]
    [TestCase(typeof(SystemException), false)]
    public void TestIsFatalWhenExceptionIsPassedThenSucceeds(Type exceptionType, bool expectedResult)
    {
        var exception = (Exception)Activator.CreateInstance(exceptionType)!;
        Assert.That(exception, Is.Not.Null);

        Assert.That(() => exception.IsFatal(), Is.EqualTo(expectedResult));
    }

    [Test]
    public void TestIsOriginatedFrom()
    {
        Assert.That(() => new Exception().IsOriginatedFrom<Exception>(), Is.True);

        Assert.That(() => new InvalidOperationException().IsOriginatedFrom<Exception>(), Is.True);
        Assert.That(() => new InvalidOperationException().IsOriginatedFrom<InvalidOperationException>(), Is.True);
        Assert.That(() => new InvalidOperationException().IsOriginatedFrom<SystemException>(), Is.True);

        Assert.That(() => new InvalidOperationException().IsOriginatedFrom<IOException>(), Is.False);
        Assert.That(() => new SystemException().IsOriginatedFrom<InvalidOperationException>(), Is.False);

        Assert.That(
            () => new TaskCanceledException().IsOriginatedFrom<TimeoutException>(),
            Is.False);

        Assert.That(
            () => new TaskCanceledException("", new IOException()).IsOriginatedFrom<TimeoutException>(),
            Is.False);

        Assert.That(
            () => new TaskCanceledException("", new TimeoutException()).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException("", new RegexMatchTimeoutException()).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException("", new TimeoutException()).IsOriginatedFrom<RegexMatchTimeoutException>(),
            Is.False);

        Assert.That(
            () => new TaskCanceledException("", new IOException("", new TimeoutException())).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException("", new IOException("", new RegexMatchTimeoutException())).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException("", new TimeoutException("", new IOException())).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException("", new RegexMatchTimeoutException("", new IOException())).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException("", new IOException("", new SystemException())).IsOriginatedFrom<TimeoutException>(),
            Is.False);
    }
}