using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    /// <summary>
    ///     Contains tests for <see cref="ReadOnlySet{T}"/> class.
    /// </summary>
    [TestFixture]
    public sealed class ReadOnlySetTests
    {
        #region Constants and Fields

        private const int Value1 = 1;
        private const int Value2 = 2;
        private const int Value3 = 3;
        private const int Value17 = 17;
        private const int ValueExtra = 42;

        private HashSet<int> _set;

        #endregion

        #region SetUp/TearDown

        [SetUp]
        public void SetUp()
        {
            _set = new HashSet<int> { Value1, Value2, Value3, Value17 };
        }

        [TearDown]
        public void TearDown()
        {
            _set = null;
        }

        #endregion

        #region Tests

        [Test]
        [Category(TestCategory.Negative)]
        public void TestInvalidConstruction()
        {
            Assert.That(
                () => ((ISet<int>)null).AsReadOnly(),
                Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => new ReadOnlySet<int>(null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ConstructionCases))]
        [Category(TestCategory.Positive)]
        public void TestConstruction(Func<ISet<int>, ReadOnlySet<int>> getReadOnlySet)
        {
            Assert.That(getReadOnlySet, Is.Not.Null);
            var count = _set.Count;

            var readOnlySet = getReadOnlySet(_set);
            Assert.That(readOnlySet, Is.Not.SameAs(_set));

            Assert.That(readOnlySet.IsReadOnly, Is.True);
            Assert.That(_set.Count, Is.EqualTo(count));
            Assert.That(readOnlySet.Count, Is.EqualTo(count));
            Assert.That(readOnlySet, Is.EquivalentTo(_set));
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestChangeTrackingScenario()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);
            Assert.That(readOnlySet.Count, Is.EqualTo(_set.Count));
            Assert.That(readOnlySet, Is.EquivalentTo(_set));

            Assert.That(readOnlySet.Contains(ValueExtra), Is.False);

            _set.Add(ValueExtra);
            Assert.That(readOnlySet.Contains(ValueExtra), Is.True);
            Assert.That(readOnlySet.Count, Is.EqualTo(_set.Count));
            Assert.That(readOnlySet, Is.EquivalentTo(_set));

            Assert.That(readOnlySet.Contains(Value2), Is.True);
            _set.Remove(Value2);
            Assert.That(readOnlySet.Contains(Value2), Is.False);
            Assert.That(readOnlySet.Count, Is.EqualTo(_set.Count));
            Assert.That(readOnlySet, Is.EquivalentTo(_set));

            _set.Clear();
            Assert.That(readOnlySet.Count, Is.EqualTo(0));
            Assert.That(readOnlySet.Any(), Is.False);
        }

        [Test]
        [Category(TestCategory.Negative)]
        public void TestReadOnly()
        {
            var count = _set.Count;
            var readOnlySet = new ReadOnlySet<int>(_set);
            Assert.That(readOnlySet.IsReadOnly, Is.True);

            var collection = (ICollection<int>)readOnlySet;
            Assert.That(collection.IsReadOnly, Is.True);

            Action assertNoChanges =
                () =>
                {
                    Assert.That(readOnlySet.Count, Is.EqualTo(count));
                    CollectionAssert.AreEquivalent(_set, readOnlySet);

                    Assert.That(collection.Count, Is.EqualTo(count));
                    CollectionAssert.AreEquivalent(_set, collection);
                };

            assertNoChanges();

            Assert.That(
                () => ((ISet<int>)readOnlySet).Add(ValueExtra),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            Assert.That(
                () => ((ISet<int>)readOnlySet).Remove(Value1),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            Assert.That(
                () => ((ISet<int>)readOnlySet).ExceptWith(Value1.AsCollection()),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            Assert.That(
                () => ((ISet<int>)readOnlySet).IntersectWith(Value1.AsCollection()),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            Assert.That(
                () => ((ISet<int>)readOnlySet).SymmetricExceptWith(Value1.AsCollection()),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            Assert.That(
                () => ((ISet<int>)readOnlySet).UnionWith(ValueExtra.AsCollection()),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            //// As collection

            Assert.That(
                () => collection.Add(ValueExtra),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            Assert.That(
                () => collection.Clear(),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();

            Assert.That(
                () => collection.Remove(Value1),
                Throws.TypeOf<NotSupportedException>());
            assertNoChanges();
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestGetEnumerator()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            var values = readOnlySet.AsEnumerable().ToList();
            Assert.That(values.Count, Is.EqualTo(_set.Count));
            Assert.That(values, Is.EquivalentTo(_set));
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestContains()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            Assert.That(readOnlySet.Contains(Value1), Is.True);
            Assert.That(readOnlySet.Contains(Value2), Is.True);
            Assert.That(readOnlySet.Contains(Value3), Is.True);
            Assert.That(readOnlySet.Contains(Value17), Is.True);

            Assert.That(readOnlySet.Contains(ValueExtra), Is.False);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestCollectionContains()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);
            var collection = (ICollection<int>)readOnlySet;

            Assert.That(collection.Contains(Value1), Is.True);
            Assert.That(collection.Contains(Value2), Is.True);
            Assert.That(collection.Contains(Value3), Is.True);
            Assert.That(collection.Contains(Value17), Is.True);

            Assert.That(collection.Contains(ValueExtra), Is.False);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestCollectionCopyTo()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);
            var collection = (ICollection<int>)readOnlySet;

            var array = new int[collection.Count];
            collection.CopyTo(array, 0);

            Assert.That(array, Is.EquivalentTo(_set));
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestIsProperSubsetOf()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            Assert.That(
                readOnlySet.IsProperSubsetOf(new[] { Value1, Value2, Value3, Value17, ValueExtra }),
                Is.True);

            Assert.That(
                readOnlySet.IsProperSubsetOf(new[] { Value1, Value2, Value17, ValueExtra }),
                Is.False);

            Assert.That(
                readOnlySet.IsProperSubsetOf(_set.ToArray()),
                Is.False);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestIsProperSupersetOf()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            Assert.That(
                readOnlySet.IsProperSupersetOf(new[] { Value1, Value2, Value3 }),
                Is.True);

            Assert.That(
                readOnlySet.IsProperSupersetOf(new[] { Value1, Value2, Value3, ValueExtra }),
                Is.False);

            Assert.That(
                readOnlySet.IsProperSupersetOf(_set.ToArray()),
                Is.False);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestIsSubsetOf()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            Assert.That(
                readOnlySet.IsSubsetOf(new[] { Value1, Value2, Value3, Value17, ValueExtra }),
                Is.True);

            Assert.That(
                readOnlySet.IsSubsetOf(new[] { Value1, Value2, Value17, ValueExtra }),
                Is.False);

            Assert.That(
                readOnlySet.IsSubsetOf(_set.ToArray()),
                Is.True);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestIsSupersetOf()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            Assert.That(
                readOnlySet.IsSupersetOf(new[] { Value1, Value2, Value3 }),
                Is.True);

            Assert.That(
                readOnlySet.IsSupersetOf(new[] { Value1, Value2, Value3, ValueExtra }),
                Is.False);

            Assert.That(
                readOnlySet.IsSupersetOf(_set.ToArray()),
                Is.True);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestOverlaps()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            Assert.That(readOnlySet.Overlaps(new[] { Value1 }), Is.True);
            Assert.That(readOnlySet.Overlaps(new[] { Value17, Value1 }), Is.True);
            Assert.That(readOnlySet.Overlaps(new[] { Value3, ValueExtra }), Is.True);

            Assert.That(readOnlySet.Overlaps(new[] { ValueExtra }), Is.False);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestSetEquals()
        {
            var readOnlySet = new ReadOnlySet<int>(_set);

            Assert.That(readOnlySet.SetEquals(_set.ToArray()), Is.True);
            Assert.That(readOnlySet.SetEquals(_set.ToArray().Concat(ValueExtra.AsCollection())), Is.False);
        }

        #endregion

        #region Nested Types: Test Cases

        internal sealed class ConstructionCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(new Func<ISet<int>, ReadOnlySet<int>>(obj => obj.AsReadOnly()))
                    .SetName("Implicit creation");

                yield return new TestCaseData(new Func<ISet<int>, ReadOnlySet<int>>(obj => new ReadOnlySet<int>(obj)))
                    .SetName("Explicit creation");
            }
        }

        #endregion
    }
}