using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    //// ReSharper disable ExpressionIsAlwaysNull - Intentionally for unit tests

    [TestFixture]
    public sealed class OmnifactotumGenericObjectExtensionsTests
    {
        [Test]
        public void TestEnsureNotNullForReferenceType()
        {
            var emptyString = string.Empty;
            Assert.That(() => emptyString.EnsureNotNull(), Is.SameAs(emptyString));

            var someObject = new object();
            Assert.That(() => someObject.EnsureNotNull(), Is.SameAs(someObject));

            object nullObject = null;
            Assert.That(() => nullObject.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestEnsureNotNullForNullable()
        {
            int? someValue = 42;
            Assert.That(() => someValue.EnsureNotNull(), Is.EqualTo(someValue.Value));

            int? nullValue = null;
            Assert.That(() => nullValue.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCase(null, "null")]
        [TestCase(42, "42")]
        public void TestToUIString(int? value, string expectedResult)
        {
            var actualResult = value.ToUIString();
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }
    }
}