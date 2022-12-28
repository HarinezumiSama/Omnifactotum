using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests;

// TODO: Temporarily commented out due to the issue in RIDER: https://youtrack.jetbrains.com/issue/RIDER-77015
//// [TestFixture(TestOf = typeof(OmnifactotumSetExtensions))]
[TestFixture(TestOf = typeof(ReadOnlySet<>))]
internal sealed class ReadOnlySetTests
{
    private const int Value1 = 1;
    private const int Value2 = 2;
    private const int Value3 = 3;
    private const int Value17 = 17;
    private const int ValueExtra = 42;

    [Test]
    public void TestInvalidConstruction()
    {
        Assert.That(
            () => ((ISet<int>?)null)!.AsReadOnly(),
            Throws.TypeOf<ArgumentNullException>().With.Property(nameof(ArgumentException.ParamName)).EqualTo("set"));

        Assert.That(
            () => new ReadOnlySet<int>(null!),
            Throws.TypeOf<ArgumentNullException>().With.Property(nameof(ArgumentException.ParamName)).EqualTo("set"));
    }

    [Test]
    [TestCaseSource(typeof(ConstructionCases))]
    public void TestConstruction(Func<ISet<int>, ReadOnlySet<int>> getReadOnlySet)
    {
        var set = CreateSet();

        Assert.That(getReadOnlySet, Is.Not.Null);
        var count = set.Count;

        var readOnlySet = getReadOnlySet(set);
        Assert.That(readOnlySet, Is.Not.SameAs(set));

        Assert.That(((ICollection<int>)readOnlySet).IsReadOnly, Is.True);
        Assert.That(set.Count, Is.EqualTo(count));
        Assert.That(readOnlySet.Count, Is.EqualTo(count));
        Assert.That(readOnlySet, Is.EquivalentTo(set));
#if NET5_0_OR_GREATER
        Assert.That(readOnlySet, Is.InstanceOf<IReadOnlySet<int>>());
#endif
    }

    [Test]
    public void TestChangeTrackingScenario()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);
        Assert.That(readOnlySet.Count, Is.EqualTo(set.Count));
        Assert.That(readOnlySet, Is.EquivalentTo(set));

        Assert.That(readOnlySet.Contains(ValueExtra), Is.False);

        set.Add(ValueExtra);
        Assert.That(readOnlySet.Contains(ValueExtra), Is.True);
        Assert.That(readOnlySet.Count, Is.EqualTo(set.Count));
        Assert.That(readOnlySet, Is.EquivalentTo(set));

        Assert.That(readOnlySet.Contains(Value2), Is.True);
        set.Remove(Value2);
        Assert.That(readOnlySet.Contains(Value2), Is.False);
        Assert.That(readOnlySet.Count, Is.EqualTo(set.Count));
        Assert.That(readOnlySet, Is.EquivalentTo(set));

        set.Clear();
        Assert.That(readOnlySet.Count, Is.EqualTo(0));
        Assert.That(readOnlySet.Any(), Is.False);
    }

    [Test]
    public void TestReadOnly()
    {
        var set = CreateSet();

        var count = set.Count;
        var readOnlySet = new ReadOnlySet<int>(set);
        Assert.That(((ICollection<int>)readOnlySet).IsReadOnly, Is.True);

        var collection = (ICollection<int>)readOnlySet;
        Assert.That(collection.IsReadOnly, Is.True);

        void AssertNoChanges()
        {
            Assert.That(readOnlySet.Count, Is.EqualTo(count));
            CollectionAssert.AreEquivalent(set, readOnlySet);

            Assert.That(collection.Count, Is.EqualTo(count));
            CollectionAssert.AreEquivalent(set, collection);
        }

        AssertNoChanges();

        Assert.That(
            () => ((ISet<int>)readOnlySet).Add(ValueExtra),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        Assert.That(
            () => ((ISet<int>)readOnlySet).Remove(Value1),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        Assert.That(
            () => ((ISet<int>)readOnlySet).ExceptWith(Value1.AsCollection()),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        Assert.That(
            () => ((ISet<int>)readOnlySet).IntersectWith(Value1.AsCollection()),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        Assert.That(
            () => ((ISet<int>)readOnlySet).SymmetricExceptWith(Value1.AsCollection()),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        Assert.That(
            () => ((ISet<int>)readOnlySet).UnionWith(ValueExtra.AsCollection()),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        //// As collection

        Assert.That(
            () => collection.Add(ValueExtra),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        Assert.That(
            collection.Clear,
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();

        Assert.That(
            () => collection.Remove(Value1),
            Throws.TypeOf<NotSupportedException>());

        AssertNoChanges();
    }

    [Test]
    public void TestGetEnumerator()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        var values = readOnlySet.AsEnumerable().ToList();
        Assert.That(values.Count, Is.EqualTo(set.Count));
        Assert.That(values, Is.EquivalentTo(set));
    }

    [Test]
    public void TestContains()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        Assert.That(readOnlySet.Contains(Value1), Is.True);
        Assert.That(readOnlySet.Contains(Value2), Is.True);
        Assert.That(readOnlySet.Contains(Value3), Is.True);
        Assert.That(readOnlySet.Contains(Value17), Is.True);

        Assert.That(readOnlySet.Contains(ValueExtra), Is.False);
    }

    [Test]
    public void TestCollectionContains()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);
        var collection = (ICollection<int>)readOnlySet;

        Assert.That(collection.Contains(Value1), Is.True);
        Assert.That(collection.Contains(Value2), Is.True);
        Assert.That(collection.Contains(Value3), Is.True);
        Assert.That(collection.Contains(Value17), Is.True);

        Assert.That(collection.Contains(ValueExtra), Is.False);
    }

    [Test]
    public void TestCollectionCopyTo()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);
        var collection = (ICollection<int>)readOnlySet;

        var array = new int[collection.Count];
        collection.CopyTo(array, 0);

        Assert.That(array, Is.EquivalentTo(set));
    }

    [Test]
    public void TestIsProperSubsetOf()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        Assert.That(
            readOnlySet.IsProperSubsetOf(new[] { Value1, Value2, Value3, Value17, ValueExtra }),
            Is.True);

        Assert.That(
            readOnlySet.IsProperSubsetOf(new[] { Value1, Value2, Value17, ValueExtra }),
            Is.False);

        Assert.That(
            readOnlySet.IsProperSubsetOf(set.ToArray()),
            Is.False);
    }

    [Test]
    public void TestIsProperSupersetOf()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        Assert.That(
            readOnlySet.IsProperSupersetOf(new[] { Value1, Value2, Value3 }),
            Is.True);

        Assert.That(
            readOnlySet.IsProperSupersetOf(new[] { Value1, Value2, Value3, ValueExtra }),
            Is.False);

        Assert.That(
            readOnlySet.IsProperSupersetOf(set.ToArray()),
            Is.False);
    }

    [Test]
    public void TestIsSubsetOf()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        Assert.That(
            readOnlySet.IsSubsetOf(new[] { Value1, Value2, Value3, Value17, ValueExtra }),
            Is.True);

        Assert.That(
            readOnlySet.IsSubsetOf(new[] { Value1, Value2, Value17, ValueExtra }),
            Is.False);

        Assert.That(
            readOnlySet.IsSubsetOf(set.ToArray()),
            Is.True);
    }

    [Test]
    public void TestIsSupersetOf()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        Assert.That(
            readOnlySet.IsSupersetOf(new[] { Value1, Value2, Value3 }),
            Is.True);

        Assert.That(
            readOnlySet.IsSupersetOf(new[] { Value1, Value2, Value3, ValueExtra }),
            Is.False);

        Assert.That(
            readOnlySet.IsSupersetOf(set.ToArray()),
            Is.True);
    }

    [Test]
    public void TestOverlaps()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        Assert.That(readOnlySet.Overlaps(new[] { Value1 }), Is.True);
        Assert.That(readOnlySet.Overlaps(new[] { Value17, Value1 }), Is.True);
        Assert.That(readOnlySet.Overlaps(new[] { Value3, ValueExtra }), Is.True);

        Assert.That(readOnlySet.Overlaps(new[] { ValueExtra }), Is.False);
    }

    [Test]
    public void TestSetEquals()
    {
        var set = CreateSet();

        var readOnlySet = new ReadOnlySet<int>(set);

        Assert.That(readOnlySet.SetEquals(set.ToArray()), Is.True);
        Assert.That(readOnlySet.SetEquals(set.ToArray().Concat(ValueExtra.AsCollection())), Is.False);
    }

    private static HashSet<int> CreateSet() => new() { Value1, Value2, Value3, Value17 };

    private sealed class ConstructionCases : TestCasesBase
    {
        protected override IEnumerable<TestCaseData> GetCases()
        {
            yield return new TestCaseData(new Func<ISet<int>, ReadOnlySet<int>>(obj => obj.AsReadOnly()))
                .SetDescription("Implicit creation");

            yield return new TestCaseData(new Func<ISet<int>, ReadOnlySet<int>>(obj => new ReadOnlySet<int>(obj)))
                .SetDescription("Explicit creation");
        }
    }
}