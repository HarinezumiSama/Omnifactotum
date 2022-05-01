#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    // TODO: Temporarily commented out due to the issue in RIDER: https://youtrack.jetbrains.com/issue/RIDER-77015
    //// [TestFixture(TestOf = typeof(RecursiveProcessingContext))]
    [TestFixture(TestOf = typeof(RecursiveProcessingContext<>))]
    internal sealed class RecursiveProcessingContextTests
    {
        [Test]
        public void TestConstructionAndCreationForReferenceTypes()
        {
            InternalTestConstructionAndCreationForReferenceType<object>();
            InternalTestConstructionAndCreationForReferenceType<string>();
        }

        [Test]
        public void TestConstructionAndCreationForValueTypes()
        {
            InternalTestConstructionAndCreationForValueType<bool>();
            InternalTestConstructionAndCreationForValueType<int>();
            InternalTestConstructionAndCreationForValueType<DateTime>();
        }

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private static void InternalTestConstructionAndCreationForReferenceType<T>()
            where T : class
        {
            AssertForReferenceType(new RecursiveProcessingContext<T>(), typeof(ByReferenceEqualityComparer<T>));
            AssertForReferenceType(new RecursiveProcessingContext<T>(new CustomEqualityComparer<T>()), typeof(CustomEqualityComparer<T>));

            AssertForReferenceType(RecursiveProcessingContext.Create<T>(), typeof(ByReferenceEqualityComparer<T>));
            AssertForReferenceType(RecursiveProcessingContext.Create<T>(new CustomEqualityComparer<T>()), typeof(CustomEqualityComparer<T>));
        }

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private static void InternalTestConstructionAndCreationForValueType<T>()
            where T : struct
        {
            AssertForValueType(new RecursiveProcessingContext<T>());
            AssertForValueType(new RecursiveProcessingContext<T>(new CustomEqualityComparer<T>()));
            AssertForValueType(new RecursiveProcessingContext<T?>());
            AssertForValueType(new RecursiveProcessingContext<T?>(new CustomEqualityComparer<T?>()));

            AssertForValueType(RecursiveProcessingContext.Create<T>());
            AssertForValueType(RecursiveProcessingContext.Create<T>(new CustomEqualityComparer<T>()));
            AssertForValueType(RecursiveProcessingContext.Create<T?>());
            AssertForValueType(RecursiveProcessingContext.Create<T?>(new CustomEqualityComparer<T?>()));
        }

        private static void AssertForReferenceType<T>(RecursiveProcessingContext<T> testee, Type expectedComparerType)
            where T : class
        {
            Assert.That(testee, Is.Not.Null);
            Assert.That(expectedComparerType, Is.Not.Null);

            Assert.That(
                () => testee.ItemsBeingProcessed,
                Is.TypeOf<HashSet<T>>() & Has.Property(nameof(HashSet<T>.Comparer)).TypeOf(expectedComparerType));
        }

        private static void AssertForValueType<T>(RecursiveProcessingContext<T> testee)
        {
            Assert.That(testee, Is.Not.Null);
            Assert.That(() => testee.ItemsBeingProcessed, Is.Null);
        }

        private sealed class CustomEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T? x, T? y) => throw new NotSupportedException();

            public int GetHashCode(T? obj) => throw new NotSupportedException();
        }
    }
}