using System;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class AsyncFactotumTests
    {
        private const int ExpectedResult = 9;

        private static readonly TimeSpan TaskWaitTime = TimeSpan.FromSeconds(5);

        [Test]
        public void TestComputeAsyncWith8ArgumentsWhenUnderlyingMethodSucceeds()
        {
            var task = AsyncFactotum.ComputeAsync(Compute8, 1, 2, 3, 4, 5, 6, 7, 8);
            Assert.That(task.Result, Is.EqualTo(ExpectedResult));
            Assert.That(task.Exception, Is.Null);
        }

        [Test]
        public void TestExecuteAsyncWith8ArgumentsWhenUnderlyingMethodSucceeds()
        {
            var task = AsyncFactotum.ExecuteAsync(Execute8, 1, 2, 3, 4, 5, 6, 7, 8);
            if (!task.Wait(TaskWaitTime))
            {
                Assert.Fail("Supposed to run pretty much fast.");
            }

            Assert.That(task.Exception, Is.Null);
        }

        private static int Compute8(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8)
        {
            Assert.That(arg1, Is.EqualTo(1));
            Assert.That(arg2, Is.EqualTo(2));
            Assert.That(arg3, Is.EqualTo(3));
            Assert.That(arg4, Is.EqualTo(4));
            Assert.That(arg5, Is.EqualTo(5));
            Assert.That(arg6, Is.EqualTo(6));
            Assert.That(arg7, Is.EqualTo(7));
            Assert.That(arg8, Is.EqualTo(8));

            Assert.That(new[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, Is.Unique);

            return ExpectedResult;
        }

        private static void Execute8(int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7, int arg8)
        {
            Assert.That(arg1, Is.EqualTo(1));
            Assert.That(arg2, Is.EqualTo(2));
            Assert.That(arg3, Is.EqualTo(3));
            Assert.That(arg4, Is.EqualTo(4));
            Assert.That(arg5, Is.EqualTo(5));
            Assert.That(arg6, Is.EqualTo(6));
            Assert.That(arg7, Is.EqualTo(7));
            Assert.That(arg8, Is.EqualTo(8));

            Assert.That(new[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, Is.Unique);
        }
    }
}