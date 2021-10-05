using System;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    internal abstract class ValueContainerHelperTestsBase<T>
        where T : IEquatable<T>
    {
        private readonly T[] _values;

        protected ValueContainerHelperTestsBase(T value, T anotherValue)
        {
            Assert.That(value, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(value));

            _values = new[] { value, anotherValue, default(T) };
        }

        [Test]
        public void TestCreateWithValue()
        {
            foreach (var value in _values)
            {
                var container = ValueContainer.Create(value);

                Assert.That(
                    container.Value,
                    typeof(T).IsValueType ? Is.EqualTo(value) : Is.SameAs(value));
            }
        }
    }
}