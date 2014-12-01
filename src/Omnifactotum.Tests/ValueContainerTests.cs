using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    [TestFixture(typeof(int), 1, int.MaxValue)]
    [TestFixture(typeof(string), "Some value", "Another value")]
    public sealed class ValueContainerTests<T>
        where T : IEquatable<T>
    {
        #region Constants and Fields

        private readonly T _value;
        private readonly T _anotherValue;
        private readonly T[] _values;

        #endregion

        #region Constructors

        public ValueContainerTests(T value, T anotherValue)
        {
            Assert.That(value, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(value));

            _value = value;
            _anotherValue = anotherValue;
            _values = new[] { value, anotherValue, default(T) };
        }

        #endregion

        #region Tests

        [Test]
        public void TestSupportedInterfaces()
        {
            Assert.That(typeof(IValueContainer<T>).IsAssignableFrom(typeof(ValueContainer<T>)), Is.True);
        }

        [Test]
        public void TestPropertyAccess()
        {
            NUnitHelper.For<ValueContainer<T>>.AssertReadableWritable(
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

        #endregion
    }
}