using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    //// ReSharper disable ExpressionIsAlwaysNull - Intentionally for unit tests

    [TestFixture]
    internal sealed class OmnifactotumGenericObjectExtensionsTests
    {
        private const string NullString = null;
        private const object NullObject = null;

        [Test]
        public void TestEnsureNotNullForReferenceTypeSucceeds()
        {
            var emptyString = string.Empty;
            Assert.That(() => emptyString.EnsureNotNull(), Is.SameAs(emptyString));

            var someObject = new object();
            Assert.That(() => someObject.EnsureNotNull(), Is.SameAs(someObject));

            object nullObject = null;
            Assert.That(() => nullObject.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestEnsureNotNullForNullableSucceeds()
        {
            int? someValue = 42;
            Assert.That(() => someValue.EnsureNotNull(), Is.EqualTo(someValue.Value));

            int? nullValue = null;
            Assert.That(() => nullValue.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCase(null, "null")]
        [TestCase(42, "42")]
        public void TestToUIStringSucceeds(int? value, string expectedResult)
        {
            var actualResult = value.ToUIString();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public void TestAsArraySucceeds()
        {
            const int IntValue = 17;
            const string StringValue = "eaacda5096aa41048c86cdbccc27ed03";
            var obj = new object();

            Assert.That(() => IntValue.AsArray(), Is.TypeOf<int[]>().And.EqualTo(new[] { IntValue }));
            Assert.That(() => StringValue.AsArray(), Is.TypeOf<string[]>().And.EqualTo(new[] { StringValue }));
            Assert.That(() => obj.AsArray(), Is.TypeOf<object[]>().And.EqualTo(new[] { obj }));
            Assert.That(() => NullString.AsArray(), Is.TypeOf<string[]>().And.EqualTo(new[] { NullString }));
            Assert.That(() => NullObject.AsArray(), Is.TypeOf<object[]>().And.EqualTo(new[] { NullObject }));
        }

        [Test]
        public void TestAsListSucceeds()
        {
            const int IntValue = 13;
            const string StringValue = "3037df31af4d426b8edc4469bdf0744c";
            var obj = new object();

            Assert.That(() => IntValue.AsList(), Is.TypeOf<List<int>>().And.EqualTo(new[] { IntValue }));
            Assert.That(() => StringValue.AsList(), Is.TypeOf<List<string>>().And.EqualTo(new[] { StringValue }));
            Assert.That(() => obj.AsList(), Is.TypeOf<List<object>>().And.EqualTo(new[] { obj }));
            Assert.That(() => NullString.AsList(), Is.TypeOf<List<string>>().And.EqualTo(new[] { NullString }));
            Assert.That(() => NullObject.AsList(), Is.TypeOf<List<object>>().And.EqualTo(new[] { NullObject }));
        }

        [Test]
        public void TestAsCollectionSucceeds()
        {
            const int IntValue = 29;
            const string StringValue = "f00136299c4249e5b5ed8d3eb18bbfb4";
            var obj = new object();

            Assert.That(
                () => IntValue.AsCollection(),
                Is.InstanceOf<IEnumerable<int>>().And.EqualTo(new[] { IntValue }));

            Assert.That(
                () => StringValue.AsCollection(),
                Is.InstanceOf<IEnumerable<string>>().And.EqualTo(new[] { StringValue }));

            Assert.That(() => obj.AsCollection(), Is.InstanceOf<IEnumerable<object>>().And.EqualTo(new[] { obj }));

            Assert.That(
                () => NullString.AsCollection(),
                Is.InstanceOf<IEnumerable<string>>().And.EqualTo(new[] { NullString }));

            Assert.That(
                () => NullObject.AsCollection(),
                Is.InstanceOf<IEnumerable<object>>().And.EqualTo(new[] { NullObject }));
        }

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestAvoidNullWhenDefaultValueProviderIsNullThenThrows()
            => Assert.That(() => new object().AvoidNull(null), Throws.ArgumentNullException);

        [Test]
        public void TestAvoidNullWhenNullValueIsPassedAndDefaultValueProviderReturnsNullThenThrows()
            => Assert.That(() => ((TestClass)null).AvoidNull(() => null), Throws.InvalidOperationException);

        [Test]
        public void TestAvoidNullWhenNonNullValueIsPassedThenSucceedsAndReturnsPassedValue()
        {
            var input = new TestClass();
            Assert.That(() => input.AvoidNull(() => null), Is.SameAs(input));
            Assert.That(() => input.AvoidNull(() => new TestClass()), Is.SameAs(input));
        }

        [Test]
        public void TestAvoidNullWhenNullValueIsPassedThenSucceedsAndReturnsValueProvidedByDefaultValueProvider()
        {
            var output = new TestClass();
            Assert.That(() => ((TestClass)null).AvoidNull(() => output), Is.SameAs(output));
        }

        [Test]
        public void TestGetHashCodeSafelyWithDefaultNullValueHashCode()
        {
            const int IntValue = 17;
            Assert.That(() => IntValue.GetHashCodeSafely(), Is.EqualTo(IntValue.GetHashCode()));

            const string StringValue = "a9fd0a6ce1824e9596b2705611754182";
            Assert.That(() => StringValue.GetHashCodeSafely(), Is.EqualTo(StringValue.GetHashCode()));

            Assert.That(() => ((int?)null).GetHashCodeSafely(), Is.EqualTo(0));
            Assert.That(() => ((string)null).GetHashCodeSafely(), Is.EqualTo(0));
            Assert.That(() => ((object)null).GetHashCodeSafely(), Is.EqualTo(0));
            Assert.That(() => ((TestClass)null).GetHashCodeSafely(), Is.EqualTo(0));
        }

        [Test]
        public void TestGetHashCodeSafelyWithSpecifiedNullValueHashCode()
        {
            const int NullValueHashCode = 1021;

            const int IntValue = 19;
            Assert.That(() => IntValue.GetHashCodeSafely(NullValueHashCode), Is.EqualTo(IntValue.GetHashCode()));

            const string StringValue = "488066fdd8764ba99dedfc6457751c6e";
            Assert.That(() => StringValue.GetHashCodeSafely(NullValueHashCode), Is.EqualTo(StringValue.GetHashCode()));

            Assert.That(() => ((int?)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
            Assert.That(() => ((string)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
            Assert.That(() => ((object)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
            Assert.That(() => ((TestClass)null).GetHashCodeSafely(NullValueHashCode), Is.EqualTo(NullValueHashCode));
        }

        private sealed class TestClass
        {
        }
    }
}