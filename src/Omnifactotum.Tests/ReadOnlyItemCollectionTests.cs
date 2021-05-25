using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ReadOnlyItemCollection<>))]
    internal sealed class ReadOnlyItemCollectionTests
    {
        private const string Item1 = "11e2082d16b64d918dafd8e2e077ab4f";
        private const string Item2 = "1ad469d8c40849bbbd4efca3fe28e3cb";
        private const string Item3 = "b6acaaaae0fe4bf49414c27239e48ef8";

        private const string AbsentItem = "de330e62b2d14bb284a6d8ebcbff4b4d";

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestInvalidConstruction()
            => Assert.That(() => new ReadOnlyItemCollection<int>(null), Throws.TypeOf<ArgumentNullException>());

        [Test]
        public void TestConstruction()
        {
            var testeeContainer = CreateTesteeContainer();
            testeeContainer.EnsureUnchanged();
        }

        [Test]
        public void TestTrackingChanges()
        {
            const string ExtraItem = @"3e4097657a9a4f13a31ecae72ba495a4";

            var testeeContainer = CreateTesteeContainer();
            testeeContainer.EnsureUnchanged();

            testeeContainer.ReferenceCollection.Add(ExtraItem);
            testeeContainer.EnsureMatchesWrappedCollection();
            Assert.That(testeeContainer.ReferenceCollection.Count, Is.EqualTo(testeeContainer.OriginalCount + 1));
            Assert.That(testeeContainer.Testee.Contains(ExtraItem), Is.True);
            Assert.That(testeeContainer.TesteeAsCollection.Contains(ExtraItem), Is.True);
#if !NET40
            Assert.That(testeeContainer.TesteeAsReadOnlyCollection.Contains(ExtraItem), Is.True);
#endif
            testeeContainer.ReferenceCollection.Remove(Item2);
            testeeContainer.EnsureMatchesWrappedCollection();
            Assert.That(testeeContainer.ReferenceCollection.Count, Is.EqualTo(testeeContainer.OriginalCount));
            Assert.That(testeeContainer.Testee.Contains(Item2), Is.False);
            Assert.That(testeeContainer.TesteeAsCollection.Contains(Item2), Is.False);
#if !NET40
            Assert.That(testeeContainer.TesteeAsReadOnlyCollection.Contains(Item2), Is.False);
#endif
            testeeContainer.ReferenceCollection.Clear();
            testeeContainer.EnsureMatchesWrappedCollection();
            Assert.That(testeeContainer.ReferenceCollection.Count, Is.Zero);
            Assert.That(testeeContainer.ReferenceCollection, Is.Empty);
            Assert.That(testeeContainer.Testee.Count, Is.Zero);
            Assert.That(testeeContainer.Testee, Is.Empty);
            Assert.That(testeeContainer.TesteeAsCollection.Count, Is.Zero);
            Assert.That(testeeContainer.TesteeAsCollection, Is.Empty);
#if !NET40
            Assert.That(testeeContainer.TesteeAsReadOnlyCollection.Count, Is.Zero);
            Assert.That(testeeContainer.TesteeAsReadOnlyCollection, Is.Empty);
#endif
        }

        [Test]
        public void TestReadOnlyBehavior()
        {
            var testeeContainer = CreateTesteeContainer();
            testeeContainer.EnsureUnchanged();

            testeeContainer.AssertNotSupportedExceptionAndEnsureUnchanged(() => testeeContainer.TesteeAsCollection.Add(@"An item"));
            testeeContainer.AssertNotSupportedExceptionAndEnsureUnchanged(() => testeeContainer.TesteeAsCollection.Clear());
            testeeContainer.AssertNotSupportedExceptionAndEnsureUnchanged(() => testeeContainer.TesteeAsCollection.Remove(Item1));
        }

        private static TesteeContainer CreateTesteeContainer() => new TesteeContainer();

        private sealed class TesteeContainer
        {
            private readonly string[] _originalItems;

            internal TesteeContainer()
            {
                ReferenceCollection = new List<string> { Item1, Item2, Item3 };
                _originalItems = ReferenceCollection.ToArray();

                Testee = new ReadOnlyItemCollection<string>(ReferenceCollection);
            }

            public int OriginalCount => _originalItems.Length;

            public ICollection<string> ReferenceCollection { get; }

            public ReadOnlyItemCollection<string> Testee { get; }

            public ICollection<string> TesteeAsCollection => Testee;

#if !NET40
            public IReadOnlyCollection<string> TesteeAsReadOnlyCollection => Testee;
#endif

            public void EnsureMatchesWrappedCollection()
            {
                Assert.That(Testee.Count, Is.EqualTo(ReferenceCollection.Count));
                Assert.That(Testee, Is.EquivalentTo(ReferenceCollection));
                Assert.That(Testee.AsEnumerable().ToArray(), Is.EquivalentTo(ReferenceCollection));
                Assert.That(Testee.Contains(AbsentItem), Is.False);
                foreach (var item in ReferenceCollection)
                {
                    Assert.That(Testee.Contains(item), Is.True);
                }
                var testeeItems = new string[Testee.Count];
                Testee.CopyTo(testeeItems, 0);
                Assert.That(testeeItems, Is.EquivalentTo(ReferenceCollection));

                Assert.That(TesteeAsCollection.IsReadOnly, Is.True);
                Assert.That(TesteeAsCollection.Count, Is.EqualTo(ReferenceCollection.Count));
                Assert.That(TesteeAsCollection, Is.EquivalentTo(ReferenceCollection));
                Assert.That(TesteeAsCollection.Contains(AbsentItem), Is.False);
                foreach (var item in ReferenceCollection)
                {
                    Assert.That(TesteeAsCollection.Contains(item), Is.True);
                }
                var testeeAsCollectionItems = new string[TesteeAsCollection.Count];
                TesteeAsCollection.CopyTo(testeeAsCollectionItems, 0);
                Assert.That(testeeAsCollectionItems, Is.EquivalentTo(ReferenceCollection));

#if !NET40
                Assert.That(TesteeAsReadOnlyCollection.Count, Is.EqualTo(ReferenceCollection.Count));
                Assert.That(TesteeAsReadOnlyCollection, Is.EquivalentTo(ReferenceCollection));
                Assert.That(TesteeAsReadOnlyCollection.Contains(AbsentItem), Is.False);
                foreach (var item in ReferenceCollection)
                {
                    Assert.That(TesteeAsReadOnlyCollection.Contains(item), Is.True);
                }
#endif
            }

            public void EnsureUnchanged()
            {
                Assert.That(ReferenceCollection, Is.EquivalentTo(_originalItems));
                EnsureMatchesWrappedCollection();
            }

            public void AssertNotSupportedExceptionAndEnsureUnchanged(TestDelegate code)
            {
                Assert.That(code.AssertNotNull(), Throws.TypeOf<NotSupportedException>());
                EnsureUnchanged();
            }
        }
    }
}