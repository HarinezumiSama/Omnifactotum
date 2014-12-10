using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;

namespace Omnifactotum.Tests.Extensions
{
    //// ReSharper disable AssignNullToNotNullAttribute - Intentionally for tests
    [TestFixture]
    public sealed class OmnifactotumCustomAttributeProviderExtensionsTests
    {
        #region Tests

        [Test]
        public void TestGetCustomAttributeArray()
        {
            var attributes = typeof(SingleAttributeAppliedTestClass)
                .GetCustomAttributeArray<SerializableAttribute>(true);

            Assert.That(attributes.Length, Is.EqualTo(1));
            Assert.That(attributes.Single(), Is.TypeOf<SerializableAttribute>());

            var noAttributes = typeof(NoAttributesAppliedTestClass).GetCustomAttributeArray<Attribute>(true);
            Assert.That(noAttributes, Is.Not.Null);
            Assert.That(noAttributes, Is.Empty);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestGetCustomAttributeArrayNegative(bool inherit)
        {
            Assert.That(
                () => ((Type)null).GetCustomAttributeArray<SerializableAttribute>(inherit),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCase(typeof(SingleAttributeAppliedTestClass))]
        [TestCase(typeof(MultipleAttributesAppliedTestClass))]
        public void TestGetSingleCustomAttribute(Type type)
        {
            var attribute = type.GetSingleCustomAttribute<SerializableAttribute>(true);
            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute, Is.TypeOf(typeof(SerializableAttribute)));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestGetSingleCustomAttributeNegative(bool inherit)
        {
            Assert.That(
                () => ((Type)null).GetSingleCustomAttribute<Attribute>(inherit),
                Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => typeof(NoAttributesAppliedTestClass).GetSingleCustomAttribute<Attribute>(inherit),
                Throws.InvalidOperationException);

            Assert.That(
                () => typeof(MultipleAttributesAppliedTestClass).GetSingleCustomAttribute<Attribute>(inherit),
                Throws.InvalidOperationException);
        }

        [Test]
        [TestCase(typeof(SingleAttributeAppliedTestClass), true)]
        [TestCase(typeof(MultipleAttributesAppliedTestClass), true)]
        [TestCase(typeof(NoAttributesAppliedTestClass), false)]
        public void TestGetSingleOrDefaultCustomAttribute(Type type, bool found)
        {
            var attribute = type.GetSingleOrDefaultCustomAttribute<SerializableAttribute>(true);
            if (found)
            {
                Assert.That(attribute, Is.Not.Null);
                Assert.That(attribute, Is.TypeOf(typeof(SerializableAttribute)));
            }
            else
            {
                Assert.That(attribute, Is.Null);
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestGetSingleOrDefaultCustomAttributeNegative(bool inherit)
        {
            Assert.That(
                () => ((Type)null).GetSingleOrDefaultCustomAttribute<Attribute>(inherit),
                Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => typeof(MultipleAttributesAppliedTestClass).GetSingleOrDefaultCustomAttribute<Attribute>(inherit),
                Throws.InvalidOperationException);
        }

        #endregion

        #region Nested Classes

        private sealed class NoAttributesAppliedTestClass
        {
        }

        [Serializable]
        private sealed class SingleAttributeAppliedTestClass
        {
        }

        [Serializable]
        [UsedImplicitly]
        private sealed class MultipleAttributesAppliedTestClass
        {
        }

        #endregion
    }
}