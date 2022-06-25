#nullable enable

using System;
using System.Threading;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(Lazy))]
    internal sealed class LazyTests
    {
        private const int Value = 13;

        [Test]
        public void TestCreateWithValueFactorySucceeds()
        {
            var lazy = Lazy.Create(() => Value);
            AssertLazyBehavior(lazy);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestCreateWithValueFactoryAndThreadSafetyFlagSucceeds(bool isThreadSafe)
        {
            var lazy = Lazy.Create(() => Value, isThreadSafe);
            AssertLazyBehavior(lazy);
        }

        [Test]
        [TestCase(LazyThreadSafetyMode.None)]
        [TestCase(LazyThreadSafetyMode.ExecutionAndPublication)]
        [TestCase(LazyThreadSafetyMode.PublicationOnly)]
        public void TestCreateWithValueFactoryAndLazyThreadSafetyModeSucceeds(LazyThreadSafetyMode threadSafetyMode)
        {
            var lazy = Lazy.Create(() => Value, threadSafetyMode);
            AssertLazyBehavior(lazy);
        }

        private static void AssertLazyBehavior(Lazy<int> lazy)
        {
            Assert.That(lazy, Is.Not.Null);
            Assert.That(lazy.IsValueCreated, Is.False);
            Assert.That(lazy.Value, Is.EqualTo(Value));
            Assert.That(lazy.IsValueCreated, Is.True);
        }
    }
}