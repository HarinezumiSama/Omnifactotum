using System;
using System.IO;
using System.Linq;
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
    public void TestIsOriginatedFromWithGenericExceptionParameter()
    {
        const string Message = "f228b8bc1d3a4c76a6f66ebf11a597a1";

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
            () => new TaskCanceledException(Message, new IOException(Message)).IsOriginatedFrom<TimeoutException>(),
            Is.False);

        Assert.That(
            () => new TaskCanceledException(Message, new TimeoutException(Message)).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new RegexMatchTimeoutException(Message)).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new TimeoutException(Message)).IsOriginatedFrom<RegexMatchTimeoutException>(),
            Is.False);

        Assert.That(
            () => new TaskCanceledException(Message, new IOException(Message, new TimeoutException(Message))).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new IOException(Message, new RegexMatchTimeoutException(Message))).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new TimeoutException(Message, new IOException(Message))).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new RegexMatchTimeoutException(Message, new IOException(Message))).IsOriginatedFrom<TimeoutException>(),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new IOException(Message, new SystemException(Message))).IsOriginatedFrom<TimeoutException>(),
            Is.False);

        Assert.That(
            () => new AggregateException(
                    Message,
                    new TaskCanceledException(Message, new TimeoutException(Message)),
                    new AggregateException(Message, new InvalidOperationException(Message), new IOException(Message)))
                .IsOriginatedFrom<IOException>(),
            Is.True);
    }

    [Test]
    public void TestIsOriginatedFromWithExceptionTypeParameter()
    {
        const string Message = "29a8d5673fa542d9b76b15fd7f22519d";

        Assert.That(
            () => new Exception().IsOriginatedFrom(null!),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("originatingExceptionType"));

        Assert.That(
            () => new Exception().IsOriginatedFrom(typeof(string)),
            Throws.ArgumentException
                .With.Property(nameof(ArgumentException.ParamName))
                .EqualTo("originatingExceptionType")
                .With.Property(nameof(Exception.Message))
                .StartsWith(@"Invalid exception type ""System.String""."));

        Assert.That(() => new Exception().IsOriginatedFrom(typeof(Exception)), Is.True);

        Assert.That(() => new InvalidOperationException().IsOriginatedFrom(typeof(Exception)), Is.True);
        Assert.That(() => new InvalidOperationException().IsOriginatedFrom(typeof(InvalidOperationException)), Is.True);
        Assert.That(() => new InvalidOperationException().IsOriginatedFrom(typeof(SystemException)), Is.True);

        Assert.That(() => new InvalidOperationException().IsOriginatedFrom(typeof(IOException)), Is.False);
        Assert.That(() => new SystemException().IsOriginatedFrom(typeof(InvalidOperationException)), Is.False);

        Assert.That(
            () => new TaskCanceledException().IsOriginatedFrom(typeof(TimeoutException)),
            Is.False);

        Assert.That(
            () => new TaskCanceledException(Message, new IOException(Message)).IsOriginatedFrom(typeof(TimeoutException)),
            Is.False);

        Assert.That(
            () => new TaskCanceledException(Message, new TimeoutException(Message)).IsOriginatedFrom(typeof(TimeoutException)),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new RegexMatchTimeoutException(Message)).IsOriginatedFrom(typeof(TimeoutException)),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new TimeoutException(Message)).IsOriginatedFrom(typeof(RegexMatchTimeoutException)),
            Is.False);

        Assert.That(
            () => new TaskCanceledException(Message, new IOException(Message, new TimeoutException(Message))).IsOriginatedFrom(typeof(TimeoutException)),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new IOException(Message, new RegexMatchTimeoutException(Message))).IsOriginatedFrom(
                typeof(TimeoutException)),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new TimeoutException(Message, new IOException(Message))).IsOriginatedFrom(typeof(TimeoutException)),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new RegexMatchTimeoutException(Message, new IOException(Message)))
                .IsOriginatedFrom(typeof(TimeoutException)),
            Is.True);

        Assert.That(
            () => new TaskCanceledException(Message, new IOException(Message, new SystemException(Message))).IsOriginatedFrom(typeof(TimeoutException)),
            Is.False);

        Assert.That(
            () => new AggregateException(
                    Message,
                    new TaskCanceledException(Message, new TimeoutException(Message)),
                    new AggregateException(Message, new InvalidOperationException(Message), new IOException(Message)))
                .IsOriginatedFrom(typeof(IOException)),
            Is.True);
    }

    [Test]
    public void TestEnumerateRecursively()
    {
        Assert.That(() => default(Exception).EnumerateRecursively(), Is.Empty);

        const string Message = "9bddb3d89fe74535b8ba64cad0ec650d";

        var timeoutException = new TimeoutException(Message);
        var taskCanceledException = new TaskCanceledException(Message, timeoutException);

        var invalidOperationException = new InvalidOperationException(Message);
        var ioException = new IOException(Message);
        var aggregateException = new AggregateException(Message, invalidOperationException, ioException);

        var rootException = new AggregateException(Message, taskCanceledException, aggregateException);

        var hierarchicalExceptions = rootException.EnumerateRecursively().ToArray();
        Assert.That(hierarchicalExceptions.Length, Is.EqualTo(6));
        Assert.That(hierarchicalExceptions[0], Is.SameAs(rootException));
        Assert.That(hierarchicalExceptions[1], Is.SameAs(taskCanceledException));
        Assert.That(hierarchicalExceptions[2], Is.SameAs(timeoutException));
        Assert.That(hierarchicalExceptions[3], Is.SameAs(aggregateException));
        Assert.That(hierarchicalExceptions[4], Is.SameAs(invalidOperationException));
        Assert.That(hierarchicalExceptions[5], Is.SameAs(ioException));
    }
}