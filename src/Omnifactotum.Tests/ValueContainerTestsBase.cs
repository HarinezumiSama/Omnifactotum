using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.Abstractions;
using Omnifactotum.NUnit;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests
{
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
                typeof(T).IsValueType ? Is.EqualTo(default(T)) : Is.Null);
        }

        [Test]
        public void TestConstructionWithValue()
        {
            foreach (var value in _values)
            {
                var container = new ValueContainer<T>(value);
                Assert.That(container.Value, GetEqualityConstraint(value));
            }
        }

        [Test]
        public void TestValue()
        {
            var container = new ValueContainer<T>(_value);
            Assert.That(container.Value, GetEqualityConstraint(_value));

            container.Value = _anotherValue;
            Assert.That(container.Value, GetEqualityConstraint(_anotherValue));
        }

        [Test]
        public void TestEquality()
        {
            var container1 = new ValueContainer<T>(_value);
            var container2 = new ValueContainer<T>(_value);
            var containerAnother = new ValueContainer<T>(_anotherValue);
            var containerDefault = new ValueContainer<T>();

            NUnitFactotum.AssertEquality(container1, container1, AssertEqualityExpectation.EqualAndMayBeSame);
            NUnitFactotum.AssertEquality(container1, container2, AssertEqualityExpectation.EqualAndCannotBeSame);
            NUnitFactotum.AssertEquality(container1, containerAnother, AssertEqualityExpectation.NotEqual);
            NUnitFactotum.AssertEquality(container1, containerDefault, AssertEqualityExpectation.NotEqual);
            NUnitFactotum.AssertEquality(container1, null, AssertEqualityExpectation.NotEqual);
        }

        [Test]
        public void TestToString()
        {
            foreach (var value in _values)
            {
                var container = new ValueContainer<T>(value);

                Assert.That(
                    container.ToString(),
                    Is.EqualTo(AsInvariant($@"{{ {nameof(ValueContainer<T>.Value)} = {value.ToStringSafelyInvariant()} }}")));
            }
        }

        private static IResolveConstraint GetEqualityConstraint(T value)
            => typeof(T).IsValueType ? Is.EqualTo(value) : Is.SameAs(value);
    }
}