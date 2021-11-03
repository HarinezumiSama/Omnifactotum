using System;
using System.Collections.Generic;

#if !NET40
using System.Collections.ObjectModel;
#endif

using System.Linq;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    //// ReSharper disable AssignNullToNotNullAttribute - for negative test cases

    [TestFixture(TestOf = typeof(ReadOnlyDictionary<,>))]
    internal sealed class ReadOnlyDictionaryTests
    {
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

        [Test]
        public void TestInvalidConstruction()
        {
            Assert.That(() => ((IDictionary<int, string>)null).AsReadOnly(), Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => new ReadOnlyDictionary<int, string>(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCaseSource(typeof(ConstructionCases))]
        public void TestConstruction(Func<IDictionary<int, string>, ReadOnlyDictionary<int, string>> getReadOnlyDictionary)
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

            Assert.That(((ICollection<int>)rod.Keys).IsReadOnly, Is.True);
            Assert.That(((ICollection<string>)rod.Values).IsReadOnly, Is.True);
        }

        [Test]
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
        public void TestReadOnlyBehavior()
        {
            var originalPairs = _dictionary.ToArray();
            var originalCount = _dictionary.Count;

            var rod = new ReadOnlyDictionary<int, string>(_dictionary);
            var rodAsDictionary = (IDictionary<int, string>)rod;
            var rodAsCollection = (ICollection<KeyValuePair<int, string>>)rod;

            void EnsureUnchanged()
            {
                Assert.That(_dictionary, Is.EquivalentTo(originalPairs));

                Assert.That(rodAsDictionary.IsReadOnly, Is.True);
                Assert.That(rodAsCollection.IsReadOnly, Is.True);

                Assert.That(rodAsDictionary.Count, Is.EqualTo(originalCount));
                Assert.That(rodAsDictionary, Is.EquivalentTo(originalPairs));

                Assert.That(rod.Count, Is.EqualTo(originalCount));
                Assert.That(rod, Is.EquivalentTo(originalPairs));

                Assert.That(rodAsCollection.Count, Is.EqualTo(originalCount));
                Assert.That(rodAsCollection, Is.EquivalentTo(originalPairs));
            }

            void AssertNotSupportedAndEnsureUnchanged(TestDelegate code)
            {
                code.AssertNotNull();
                Assert.That(code, Throws.TypeOf<NotSupportedException>());

                EnsureUnchanged();
            }

            EnsureUnchanged();

            //// As IDictionary<TKey, TValue>

            AssertNotSupportedAndEnsureUnchanged(() => rodAsDictionary.Add(KeyExtra, ValueExtra));
            AssertNotSupportedAndEnsureUnchanged(() => rodAsDictionary.Remove(Key1));
            AssertNotSupportedAndEnsureUnchanged(() => rodAsDictionary[KeyExtra] = ValueExtra);
            AssertNotSupportedAndEnsureUnchanged(() => rodAsDictionary[Key1] = ValueExtra);

            //// As ICollection<TKey, TValue>

            AssertNotSupportedAndEnsureUnchanged(() => rodAsCollection.Add(new KeyValuePair<int, string>(KeyExtra, ValueExtra)));
            AssertNotSupportedAndEnsureUnchanged(rodAsCollection.Clear);
            AssertNotSupportedAndEnsureUnchanged(() => rodAsCollection.Remove(new KeyValuePair<int, string>(Key1, Value1)));

            //// Keys

            var rodKeys = (ICollection<int>)rod.Keys;

            AssertNotSupportedAndEnsureUnchanged(() => rodKeys.Add(KeyExtra));
            AssertNotSupportedAndEnsureUnchanged(() => rodKeys.Remove(Key1));
            AssertNotSupportedAndEnsureUnchanged(() => rodKeys.Clear());

            //// Values

            var rodValues = (ICollection<string>)rod.Values;

            AssertNotSupportedAndEnsureUnchanged(() => rodValues.Add(ValueExtra));
            AssertNotSupportedAndEnsureUnchanged(() => rodValues.Remove(Value1));
            AssertNotSupportedAndEnsureUnchanged(() => rodValues.Clear());
        }

        [Test]
        public void TestGetEnumerator()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);

            var pairs = rod.AsEnumerable().ToList();
            Assert.That(pairs.Count, Is.EqualTo(_dictionary.Count));
            Assert.That(pairs, Is.EquivalentTo(_dictionary));
        }

        [Test]
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
        public void TestTryGetValue()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);

            Assert.That(rod.TryGetValue(Key1, out var resultValue1), Is.True);
            Assert.That(resultValue1, Is.EqualTo(Value1));

            Assert.That(rod.TryGetValue(Key2, out var resultValue2), Is.True);
            Assert.That(resultValue2, Is.EqualTo(Value2));

            Assert.That(rod.TryGetValue(Key3, out var resultValue3), Is.True);
            Assert.That(resultValue3, Is.EqualTo(Value3));

            Assert.That(rod.TryGetValue(Key17, out var resultValue17), Is.True);
            Assert.That(resultValue17, Is.EqualTo(Value17));

            Assert.That(rod.TryGetValue(KeyExtra, out var resultValueExtra), Is.False);
            Assert.That(resultValueExtra, Is.Null);
        }

        [Test]
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
        public void TestCollectionCopyTo()
        {
            var rod = new ReadOnlyDictionary<int, string>(_dictionary);
            var rodCollection = (ICollection<KeyValuePair<int, string>>)rod;

            var array = new KeyValuePair<int, string>[rodCollection.Count];
            rodCollection.CopyTo(array, 0);

            Assert.That(array, Is.EquivalentTo(_dictionary));
        }

        private sealed class ConstructionCases : TestCasesBase
        {
            protected override IEnumerable<TestCaseData> GetCases()
            {
                yield return new TestCaseData(
                        new Func<IDictionary<int, string>, ReadOnlyDictionary<int, string>>(
                            obj => obj.AsReadOnly()))
                    .SetDescription("Implicit creation");

                yield return new TestCaseData(
                        new Func<IDictionary<int, string>, ReadOnlyDictionary<int, string>>(
                            obj => new ReadOnlyDictionary<int, string>(obj)))
                    .SetDescription("Explicit creation");
            }
        }
    }
}