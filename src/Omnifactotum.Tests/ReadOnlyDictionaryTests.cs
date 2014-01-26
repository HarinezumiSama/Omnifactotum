using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    /// <summary>
    ///     Contains tests for <see cref="ReadOnlyDictionary{TKey,TValue}"/> class.
    /// </summary>
    [TestFixture]
    public sealed class ReadOnlyDictionaryTests
    {
        #region Constants and Fields

        private const int Key1 = 1;
        private const string Value1 = "One";

        private const int Key2 = 2;
        private const string Value2 = "Two";

        private const int Key3 = 3;
        private const string Value3 = "Three";

        private const int Key17 = 17;
        private const string Value17 = "Seventeen";

        private const int KeyExtra = 42;
        private const string ValueExtra = "Forty two";

        private Dictionary<int, string> _dictionary;

        #endregion

        #region SetUp/TearDown

        [SetUp]
        public void SetUp()
        {
            _dictionary = new Dictionary<int, string>
            {
                { Key1, Value1 },
                { Key2, Value2 },
                { Key3, Value3 },
                { Key17, Value17 }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _dictionary = null;
        }

        #endregion

        #region Tests

        [Test]
        [Category(TestCategory.Negative)]
        public void TestInvalidConstruction()
        {
            Assert.That(() => ((IDictionary<int, string>)null).AsReadOnly(), Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => new ReadOnlyDictionary<int, string>(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ConstructionCases))]
        [Category(TestCategory.Positive)]
        public void TestConstruction(
            Func<IDictionary<int, string>, ReadOnlyDictionary<int, string>> getReadOnlyDictionary)
        {
            var count = _dictionary.Count;

            var rod = getReadOnlyDictionary.AssertNotNull()(_dictionary);
            Assert.That(rod, Is.Not.Null);
            Assert.That(rod, Is.Not.SameAs(_dictionary));

            Assert.That(_dictionary.Count, Is.EqualTo(count));

            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rod.Keys.Count, Is.EqualTo(count));
            Assert.That(rod.Values.Count, Is.EqualTo(count));

            Assert.That(rod, Is.EquivalentTo(_dictionary));
            Assert.That(rod.Keys, Is.EquivalentTo(_dictionary.Keys));
            Assert.That(rod.Values, Is.EquivalentTo(_dictionary.Values));

            Assert.That(rod.Keys.IsReadOnly, Is.True);
            Assert.That(rod.Values.IsReadOnly, Is.True);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestTrackingChanges()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);
            Assert.That(rod.Count, Is.EqualTo(_dictionary.Count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            _dictionary.Add(KeyExtra, ValueExtra);
            Assert.That(rod.ContainsKey(KeyExtra), Is.True);
            Assert.That(rod[KeyExtra], Is.EqualTo(ValueExtra));
            Assert.That(rod.Count, Is.EqualTo(_dictionary.Count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            const string NewValueExtra = ValueExtra + "_New";
            Assert.That(rod.ContainsKey(KeyExtra), Is.True);
            Assert.That(rod[KeyExtra], Is.EqualTo(ValueExtra));
            _dictionary[KeyExtra] = NewValueExtra;
            Assert.That(rod.ContainsKey(KeyExtra), Is.True);
            Assert.That(rod[KeyExtra], Is.EqualTo(NewValueExtra));
            Assert.That(rod.Count, Is.EqualTo(_dictionary.Count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            Assert.That(rod.ContainsKey(Key2), Is.True);
            _dictionary.Remove(Key2);
            Assert.That(rod.ContainsKey(Key2), Is.False);
            Assert.That(rod.Count, Is.EqualTo(_dictionary.Count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            _dictionary.Clear();
            Assert.That(rod.Count, Is.EqualTo(0));
            Assert.That(rod.Any(), Is.False);
        }

        [Test]
        [Category(TestCategory.Negative)]
        public void TestReadOnly()
        {
            var count = _dictionary.Count;

            var rod = new ReadOnlyDictionary<int, string>(_dictionary);

            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));
            Assert.That(rod.IsReadOnly, Is.True);

            Assert.That(
                () => ((IDictionary<int, string>)rod).Add(KeyExtra, ValueExtra),
                Throws.TypeOf<NotSupportedException>());
            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            Assert.That(() => ((IDictionary<int, string>)rod).Remove(Key1), Throws.TypeOf<NotSupportedException>());
            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            Assert.That(() => rod[KeyExtra] = ValueExtra, Throws.TypeOf<NotSupportedException>());
            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            Assert.That(() => rod[Key1] = ValueExtra, Throws.TypeOf<NotSupportedException>());
            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rod, Is.EquivalentTo(_dictionary));

            //// As collection

            var rodCollection = (ICollection<KeyValuePair<int, string>>)rod;

            Assert.That(rodCollection.Count, Is.EqualTo(count));
            Assert.That(rodCollection, Is.EquivalentTo(_dictionary));
            Assert.That(rodCollection.IsReadOnly, Is.True);

            Assert.That(
                () => rodCollection.Add(new KeyValuePair<int, string>(KeyExtra, ValueExtra)),
                Throws.TypeOf<NotSupportedException>());
            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rodCollection.Count, Is.EqualTo(count));
            Assert.That(rodCollection, Is.EquivalentTo(_dictionary));

            Assert.That(() => rodCollection.Clear(), Throws.TypeOf<NotSupportedException>());
            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rodCollection.Count, Is.EqualTo(count));
            Assert.That(rodCollection, Is.EquivalentTo(_dictionary));

            Assert.That(
                () => rodCollection.Remove(new KeyValuePair<int, string>(Key1, Value1)),
                Throws.TypeOf<NotSupportedException>());
            Assert.That(rod.Count, Is.EqualTo(count));
            Assert.That(rodCollection.Count, Is.EqualTo(count));
            Assert.That(rodCollection, Is.EquivalentTo(_dictionary));

            //// Keys

            Assert.That(() => rod.Keys.Add(KeyExtra), Throws.TypeOf<NotSupportedException>());
            Assert.That(() => rod.Keys.Clear(), Throws.TypeOf<NotSupportedException>());

            //// Values

            Assert.That(() => rod.Values.Add(ValueExtra), Throws.TypeOf<NotSupportedException>());
            Assert.That(() => rod.Values.Clear(), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestGetEnumerator()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);

            var pairs = rod.AsEnumerable().ToList();
            Assert.That(pairs.Count, Is.EqualTo(_dictionary.Count));
            Assert.That(pairs, Is.EquivalentTo(_dictionary));
        }

        [Test]
        [Category(TestCategory.Positive)]
        [Category(TestCategory.Negative)]
        public void TestIndexer()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);

            Assert.That(rod[Key1], Is.EqualTo(Value1));
            Assert.That(rod[Key2], Is.EqualTo(Value2));
            Assert.That(rod[Key3], Is.EqualTo(Value3));
            Assert.That(rod[Key17], Is.EqualTo(Value17));

            Assert.That(() => rod[KeyExtra], Throws.TypeOf<KeyNotFoundException>());
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestContainsKey()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);

            Assert.That(rod.ContainsKey(Key1), Is.True);
            Assert.That(rod.ContainsKey(Key2), Is.True);
            Assert.That(rod.ContainsKey(Key3), Is.True);
            Assert.That(rod.ContainsKey(Key17), Is.True);

            Assert.That(rod.ContainsKey(KeyExtra), Is.False);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestTryGetValue()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);

            string resultValue1;
            Assert.That(rod.TryGetValue(Key1, out resultValue1), Is.True);
            Assert.That(resultValue1, Is.EqualTo(Value1));

            string resultValue2;
            Assert.That(rod.TryGetValue(Key2, out resultValue2), Is.True);
            Assert.That(resultValue2, Is.EqualTo(Value2));

            string resultValue3;
            Assert.That(rod.TryGetValue(Key3, out resultValue3), Is.True);
            Assert.That(resultValue3, Is.EqualTo(Value3));

            string resultValue17;
            Assert.That(rod.TryGetValue(Key17, out resultValue17), Is.True);
            Assert.That(resultValue17, Is.EqualTo(Value17));

            string resultValueExtra;
            Assert.That(rod.TryGetValue(KeyExtra, out resultValueExtra), Is.False);
            Assert.That(resultValueExtra, Is.Null);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestCollectionContains()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);
            var rodCollection = (ICollection<KeyValuePair<int, string>>)rod;

            Assert.That(rodCollection.Contains(new KeyValuePair<int, string>(Key1, Value1)), Is.True);
            Assert.That(rodCollection.Contains(new KeyValuePair<int, string>(Key2, Value2)), Is.True);
            Assert.That(rodCollection.Contains(new KeyValuePair<int, string>(Key3, Value3)), Is.True);
            Assert.That(rodCollection.Contains(new KeyValuePair<int, string>(Key17, Value17)), Is.True);

            Assert.That(rodCollection.Contains(new KeyValuePair<int, string>(Key1, Value2)), Is.False);
            Assert.That(rodCollection.Contains(new KeyValuePair<int, string>(KeyExtra, ValueExtra)), Is.False);
        }

        [Test]
        [Category(TestCategory.Positive)]
        public void TestCollectionCopyTo()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);
            var rodCollection = (ICollection<KeyValuePair<int, string>>)rod;

            var array = new KeyValuePair<int, string>[rodCollection.Count];
            rodCollection.CopyTo(array, 0);

            Assert.That(array, Is.EquivalentTo(_dictionary));
        }

        #endregion

        #region Nested Types: Test Cases

        internal sealed class ConstructionCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(
                    new Func<IDictionary<int, string>, ReadOnlyDictionary<int, string>>(
                        obj => obj.AsReadOnly()))
                    .SetName("Implicit creation");

                yield return new TestCaseData(
                    new Func<IDictionary<int, string>, ReadOnlyDictionary<int, string>>(
                        obj => new ReadOnlyDictionary<int, string>(obj)))
                    .SetName("Explicit creation");
            }
        }

        #endregion
    }
}