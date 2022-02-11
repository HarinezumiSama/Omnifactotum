#nullable enable

using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumHashCodeHelper))]
    internal sealed class OmnifactotumHashCodeHelperTests
    {
        [Test]
        [TestCase(17, 13, 6736)]
        [TestCase(int.MaxValue, int.MinValue, -397)]
        [TestCase(int.MaxValue - 1, int.MinValue + 1, -793)]
        public void TestCombineHashCodeValuesSucceeds(int previousHashCode, int nextHashCode, int expectedResult)
            => Assert.That(previousHashCode.CombineHashCodeValues(nextHashCode), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(1234567890123L, 9876543210987UL, 1301382356)]
        [TestCase(9876543210987L, 1234567890123UL, 1705296900)]
        [TestCase(0L, 9876543210987UL, -1881567984)]
        [TestCase(null, 9876543210987UL, -1881567984)]
        [TestCase(9876543210987L, null, 341819856)]
        [TestCase(9876543210987L, 0UL, 341819856)]
        public void TestCombineHashCodesWithTwoGenericArgumentsSucceeds(
            long? previous,
            ulong? next,
            int expectedResult)
            => Assert.That(previous.CombineHashCodes(next), Is.EqualTo(expectedResult));

        [Test]
        [TestCase(17, 9876543210987UL, -1881565363)]
        [TestCase(int.MaxValue, 1234567890123UL, 235207591)]
        [TestCase(17, 0UL, 6749)]
        [TestCase(17, null, 6749)]
        public void TestCombineHashCodesWithPreviousHashCodeAndGenericArgumentSucceeds(
            int previousHashCode,
            ulong? next,
            int expectedResult)
            => Assert.That(previousHashCode.CombineHashCodes(next), Is.EqualTo(expectedResult));
    }
}