using System;
using System.Globalization;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumArrayExtensions))]
internal sealed class OmnifactotumArrayExtensionsTests
{
    private static readonly string[]? NullArray = null;

    [Test]
    public void TestCopyNull()
    {
        var copy = NullArray.Copy();
        Assert.That(copy, Is.Null);
    }

    [Test]
    public void TestCopyNonNull()
    {
        var array = new[] { new CopyableObject { Value = 1 }, new CopyableObject { Value = 2 } };
        var copy = array.Copy().AssertNotNull();

        Assert.That(copy, Is.Not.Null);
        Assert.That(copy, Is.Not.SameAs(array));
        Assert.That(copy.Length, Is.EqualTo(array.Length));
        for (var index = 0; index < array.Length; index++)
        {
            Assert.That(copy[index], Is.SameAs(array[index]));
        }
    }

    [Test]
    public void TestInitializeWithOldValueNegative()
    {
        Assert.That(() => NullArray!.Initialize((s, _) => s), Throws.TypeOf<ArgumentNullException>());

        var nonNullArray = new[] { "foo", "bar" }.AssertNotNull();

        Assert.That(
            () => nonNullArray.Initialize((Func<string, int, string>)null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void TestInitializeWithOldValue()
    {
        var array = new[] { "foo", "bar" }.AssertNotNull();
        array.Initialize((s, i) => s + "-" + (i + 1).ToString(CultureInfo.InvariantCulture));
        Assert.That(array[0], Is.EqualTo("foo-1"));
        Assert.That(array[1], Is.EqualTo("bar-2"));
    }

    [Test]
    public void TestInitializeNegative()
    {
        Assert.That(
            () => NullArray!.Initialize(i => i.ToString(CultureInfo.InvariantCulture)),
            Throws.TypeOf<ArgumentNullException>());

        var nonNullArray = new[] { "foo", "bar" }.AssertNotNull();

        Assert.That(
            () => nonNullArray.Initialize((Func<int, string>)null!),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void TestInitialize()
    {
        var array = new[] { "foo", "bar" }.AssertNotNull();
        array.Initialize(i => (i + 1).ToString(CultureInfo.InvariantCulture));
        Assert.That(array[0], Is.EqualTo("1"));
        Assert.That(array[1], Is.EqualTo("2"));
    }

    [Test]
    public void TestAvoidNull()
    {
        var avoided = NullArray.AvoidNull();
        Assert.That(avoided, Is.Not.Null);
        Assert.That(avoided.Length, Is.EqualTo(0));

        var array = new[] { "foo", "bar" }.AssertNotNull();
        var shouldBeSame = array.AvoidNull();
        Assert.That(shouldBeSame, Is.SameAs(array));
    }

    private sealed class CopyableObject : ICloneable
    {
        // ReSharper disable once PropertyCanBeMadeInitOnly.Local
        public int Value { private get; set; }

        [UsedImplicitly]
        public CopyableObject Copy() => new() { Value = Value };

        public object Clone() => Copy();
    }
}