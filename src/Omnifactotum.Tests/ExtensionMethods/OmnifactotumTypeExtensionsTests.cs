using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
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

    [Test]
    public void TestGetCollectionElementTypeOrDefaultWhenInvalidArgumentThenThrows()
        => Assert.That(() => ((Type?)null)!.GetCollectionElementTypeOrDefault(), Throws.ArgumentNullException);

    [Test]
    [TestCase(typeof(object), null)]
    [TestCase(typeof(int), null)]
    [TestCase(typeof(int*), null)]
    [TestCase(typeof(string), typeof(char))]
    [TestCase(typeof(Array), typeof(object))]
    [TestCase(typeof(IEnumerable), typeof(object))]
    [TestCase(typeof(string[]), typeof(string))]
    [TestCase(typeof(char[]), typeof(char))]
    [TestCase(typeof(IEnumerable<char>), typeof(char))]
    [TestCase(typeof(ICollection<char>), typeof(char))]
    [TestCase(typeof(Collection<char>), typeof(char))]
    [TestCase(typeof(ReadOnlyCollection<char>), typeof(char))]
    [TestCase(typeof(IList<char>), typeof(char))]
    [TestCase(typeof(List<char>), typeof(char))]
    [TestCase(typeof(int[]), typeof(int))]
    [TestCase(typeof(IEnumerable<int>), typeof(int))]
    [TestCase(typeof(ICollection<int>), typeof(int))]
    [TestCase(typeof(Collection<int>), typeof(int))]
    [TestCase(typeof(ReadOnlyCollection<int>), typeof(int))]
    [TestCase(typeof(IList<int>), typeof(int))]
    [TestCase(typeof(List<int>), typeof(int))]
    [TestCase(typeof(IDictionary<int, string>), typeof(KeyValuePair<int, string>))]
    [TestCase(typeof(Dictionary<int, string>), typeof(KeyValuePair<int, string>))]
    [TestCase(typeof(HashSet<Type>), typeof(Type))]
    [TestCase(typeof(ISet<Type>), typeof(Type))]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestGetCollectionElementTypeOrDefaultWhenValidArgumentThenSucceeds(Type input, Type? expectedResult)
    {
        Assert.That(() => input.GetCollectionElementTypeOrDefault(), Is.EqualTo(expectedResult));

#pragma warning disable CS0618
        Assert.That(() => input.GetCollectionElementType(), Is.EqualTo(expectedResult));
#pragma warning restore CS0618
    }
}