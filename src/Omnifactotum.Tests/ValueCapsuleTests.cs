using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class ValueCapsuleTests
    {
        [Test]
        public void TestConstruction()
        {
            const string Value = "74eb9ddd3c6d496e9f84ceeb765412cd";
            var capsule = new StringCapsule(Value);
            Assert.That(capsule.Value, Is.EqualTo(Value));
        }

        [Test]
        public void TestValuePropertyIsReadOnly()
        {
            var valuePropertyInfo = typeof(ValueCapsule<>).GetProperty(
                nameof(ValueCapsule<object>.Value),
                BindingFlags.Instance | BindingFlags.Public);

            Assert.That(valuePropertyInfo, Is.Not.Null);

            Assert.That(valuePropertyInfo.GetGetMethod(false), Is.Not.Null);
            Assert.That(valuePropertyInfo.GetSetMethod(true), Is.Null);
        }

        private sealed class StringCapsule : ValueCapsule<string>
        {
            public StringCapsule([CanBeNull] string value)
                : base(value)
            {
                // Nothing to do
            }
        }
    }
}