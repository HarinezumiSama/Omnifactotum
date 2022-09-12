using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumCollectionHashCodeHelper))]
internal sealed class OmnifactotumCollectionHashCodeHelperTests
{
    [Test]
    [TestCase(null, 0)]
    [TestCase(new object?[0], 0)]
    [TestCase(new object?[] { 17 }, 17)]
    [TestCase(new object?[] { 17, 0 }, 6749)]
    [TestCase(new object?[] { 17, null }, 6749)]
    [TestCase(new object?[] { 17, 0, 0 }, 2679353)]
    [TestCase(new object?[] { 17, null, 0 }, 2679353)]
    [TestCase(new object?[] { 0, 17 }, 17)]
    [TestCase(new object?[] { 0, 17, 0 }, 6749)]
    [TestCase(new object?[] { 0, 17, null }, 6749)]
    [TestCase(new object?[] { null, 17 }, 17)]
    [TestCase(new object?[] { 17, 13 }, 6736)]
    [TestCase(new object?[] { 17, 13, 42 }, 2674234)]
    [TestCase(new object?[] { 17, 13, int.MaxValue }, 2144809455)]
    public void TestComputeCollectionHashCodeSucceeds(
        object[] input,
        int expectedResult)
        => Assert.That(input.ComputeCollectionHashCode, Is.EqualTo(expectedResult));
}