using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumTimeSpanExtensions))]
    internal sealed class OmnifactotumTimeSpanExtensionsTests
    {
        [Test]
        public void TestMultiply()
        {
            const decimal Coefficient = 1.4m;

            var ts = new TimeSpan(1, 2, 39);
            var result = ts.Multiply(Coefficient);
            Assert.That(result.Ticks, Is.EqualTo((long)(ts.Ticks * Coefficient)));
        }

        [Test]
        public void TestDivide()
        {
            const decimal Coefficient = 1.2m;

            var ts = new TimeSpan(2, 3, 57);
            var result = ts.Divide(Coefficient);
            Assert.That(result.Ticks, Is.EqualTo((long)(ts.Ticks / Coefficient)));
        }

        [Test]
        public void TestInvalidDivide()
        {
            var ts = new TimeSpan(3, 2, 1);
            Assert.That(() => ts.Divide(0m), Throws.ArgumentException);
        }
    }
}