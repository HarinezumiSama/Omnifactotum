using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumEqualityComparerExtensions))]
    internal sealed class OmnifactotumEqualityComparerExtensionsTests
    {
        [Test]
        [TestCase(null, int.MinValue)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase((int)sbyte.MaxValue, -sbyte.MaxValue)]
        [TestCase(int.MaxValue, -int.MaxValue)]
        public void TestGetHashCodeSafelyWithSpecifiedFallbackValue(int? value, int expectedResult)
        {
            var equalityComparerMock = new Mock<IEqualityComparer<int?>>(MockBehavior.Strict);

            equalityComparerMock
                .Setup(comparer => comparer.GetHashCode(It.IsAny<int?>()!))
                .Returns<int?>(obj => -obj ?? throw new ArgumentNullException(nameof(obj)));

            Assert.That(() => equalityComparerMock.Object.GetHashCodeSafely(value, int.MinValue), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null, 0)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase((int)sbyte.MaxValue, -sbyte.MaxValue)]
        [TestCase(int.MaxValue, -int.MaxValue)]
        public void TestGetHashCodeSafelyWithDefaultFallbackValue(int? value, int expectedResult)
        {
            var equalityComparerMock = new Mock<IEqualityComparer<int?>>(MockBehavior.Strict);

            equalityComparerMock
                .Setup(comparer => comparer.GetHashCode(It.IsAny<int?>()!))
                .Returns<int?>(obj => -obj ?? throw new ArgumentNullException(nameof(obj)));

            Assert.That(() => equalityComparerMock.Object.GetHashCodeSafely(value), Is.EqualTo(expectedResult));
        }
    }
}