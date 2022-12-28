using System;
using System.IO;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumTypeExtensions))]
internal sealed class OmnifactotumTypeExtensionsTests
{
    [Test]
    public void TestGetManifestResourceStreamWhenNullTypeIsPassedThenThrows()
        => Assert.That(() => ((Type?)null)!.GetManifestResourceStream("ValidName"), Throws.ArgumentNullException);

    [Test]
    [TestCase(null)]
    [TestCase("")]
    public void TestGetManifestResourceStreamWhenInvalidNameIsPassedThenThrows(string? name)
        => Assert.That(() => GetType().GetManifestResourceStream(name!), Throws.ArgumentException);

    [Test]
    public void TestGetManifestResourceStreamNonExistentResourceNameIsPassedThenReturnsNull()
    {
        using var stream = GetType().GetManifestResourceStream("NonExistentResourceName");
        Assert.That(stream, Is.Null);
    }

    [Test]
    public void TestGetManifestResourceStreamWhenValidResourceNameIsPassedThenReturnsNonNullStream()
    {
        using var stream = GetType().GetManifestResourceStream("OmnifactotumTypeExtensionsTests.TestResource.txt");
        Assert.That(stream, Is.Not.Null);

        using var reader = new StreamReader(stream!);
        var actualValue = reader.ReadToEnd();
        Assert.That(actualValue, Is.EqualTo("Test"));
    }

    [Test]
    public void TestIsNullableValueTypeWhenNullArgumentIsPassedThenThrows()
    {
        Assert.That(() => ((Type?)null)!.IsNullableValueType(), Throws.ArgumentNullException);

#pragma warning disable CS0618
        Assert.That(() => ((Type?)null)!.IsNullable(), Throws.ArgumentNullException);
#pragma warning restore CS0618
    }

    [Test]
    [TestCase(typeof(void), false)]
    [TestCase(typeof(void*), false)]
    [TestCase(typeof(bool), false)]
    [TestCase(typeof(bool?), true)]
    [TestCase(typeof(int), false)]
    [TestCase(typeof(int*), false)]
    [TestCase(typeof(int?), true)]
    [TestCase(typeof(IntPtr), false)]
    [TestCase(typeof(IntPtr*), false)]
    [TestCase(typeof(IntPtr?), true)]
    [TestCase(typeof(DateTime), false)]
    [TestCase(typeof(DateTime?), true)]
    [TestCase(typeof(DateTimeKind), false)]
    [TestCase(typeof(DateTimeKind?), true)]
    [TestCase(typeof(string), false)]
    [TestCase(typeof(object), false)]
    [TestCase(typeof(Action), false)]
    [TestCase(typeof(IDisposable), false)]
    public void TestIsNullableValueTypeWhenValidArgumentIsPassedThenSucceeds(Type type, bool expectedResult)
    {
        Assert.That(type.IsNullableValueType(), Is.EqualTo(expectedResult));

#pragma warning disable CS0618
        Assert.That(type.IsNullable(), Is.EqualTo(expectedResult));
#pragma warning restore CS0618
    }
}