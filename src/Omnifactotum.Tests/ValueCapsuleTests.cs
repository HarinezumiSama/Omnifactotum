using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ValueCapsule<>))]
    internal sealed class ValueCapsuleTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("ab81a1846da744d1a38284a57713564e")]
        public void TestConstruction(string? value)
        {
            var capsule = new StringCapsule(value);
            Assert.That(capsule.Value, Is.EqualTo(value));
        }

        [Test]
        public void TestValuePropertyIsReadOnly()
        {
            var valuePropertyInfo = typeof(ValueCapsule<>).GetProperty(
                nameof(ValueCapsule<object>.Value),
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(valuePropertyInfo, Is.Not.Null);

            Assert.That(() => valuePropertyInfo!.GetGetMethod(false), Is.Not.Null);
            Assert.That(() => valuePropertyInfo!.GetSetMethod(true), Is.Null);
        }

        private sealed class StringCapsule : ValueCapsule<string?>
        {
            public StringCapsule([CanBeNull] string? value)
                : base(value)
            {
                // Nothing to do
            }
        }
    }
}