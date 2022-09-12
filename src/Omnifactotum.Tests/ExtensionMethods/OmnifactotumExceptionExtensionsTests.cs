using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

#if NETFRAMEWORK
using System.Threading;
#endif

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

#if NETFRAMEWORK
        [Test]
        public void TestIsFatalWhenThreadAbortExceptionIsPassedThenReturnsTrue()
        {
            var currentThread = Thread.CurrentThread;
            var stateInfo = new object();

            try
            {
                currentThread.Abort(stateInfo);
            }
            catch (ThreadAbortException ex)
                when (ReferenceEquals(ex.ExceptionState, stateInfo))
            {
                Thread.ResetAbort();

                Assert.That(() => ex.IsFatal(), Is.True);
            }
        }

#endif

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
}