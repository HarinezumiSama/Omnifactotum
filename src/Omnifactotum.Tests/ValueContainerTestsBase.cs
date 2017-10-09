using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture(typeof(int), 1, int.MaxValue)]
    [TestFixture(typeof(string), "Some value", "Another value")]
    internal abstract class ValueContainerTestsBase<T>
        where T : IEquatable<T>
    {
        private readonly T _value;
        private readonly T _anotherValue;
        private readonly T[] _values;

        protected ValueContainerTestsBase(T value, T anotherValue)
        {
            Assert.That(value, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(value));

            _value = value;
            _anotherValue = anotherValue;
            _values = new[] { value, anotherValue, default(T) };
        }

        [Test]
        public void TestSupportedInterfaces()
        {
            Assert.That(typeof(IValueContainer<T>).IsAssignableFrom(typeof(ValueContainer<T>)), Is.True);
        }

        [Test]
        public void TestPropertyAccess()
        {
            NUnitFactotum.For<ValueContainer<T>>.AssertReadableWritable(
                obj => obj.Value,
                PropertyAccessMode.ReadWrite);
        }

        [Test]
        public void TestConstructionDefault()
        {
            var container = new ValueContainer<T>();

            Assert.That(
                container.Value,
                typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(default(T)) : Is.Null);
        }

        [Test]
        public void TestConstructionWithValue()
        {
            foreach (var value in _values)
            {
                var container = new ValueContainer<T>(value);

                Assert.That(
                    container.Value,
                    typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(value) : Is.SameAs(value));
            }
        }

        [Test]
        public void TestValue()
        {
            var container = new ValueContainer<T>(_value);

            Assert.That(
                container.Value,
                typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(_value) : Is.SameAs(_value));

            container.Value = _anotherValue;

            Assert.That(
                container.Value,
                typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(_anotherValue) : Is.SameAs(_anotherValue));
        }
    }
}