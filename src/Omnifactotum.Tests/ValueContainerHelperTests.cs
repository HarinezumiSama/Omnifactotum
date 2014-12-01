using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests
{
    [TestFixture(typeof(int), 1, int.MaxValue)]
    [TestFixture(typeof(string), "Some value", "Another value")]
    public sealed class ValueContainerHelperTests<T>
        where T : IEquatable<T>
    {
        #region Constants and Fields

        private readonly T[] _values;

        #endregion

        #region Constructors

        public ValueContainerHelperTests(T value, T anotherValue)
        {
            Assert.That(value, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(default(T)));
            Assert.That(anotherValue, Is.Not.EqualTo(value));

            _values = new[] { value, anotherValue, default(T) };
        }

        #endregion

        #region Tests

        [Test]
        public void TestCreateWithValue()
        {
            foreach (var value in _values)
            {
                var container = ValueContainer.Create(value);

                Assert.That(
                    container.Value,
                    typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(value) : Is.SameAs(value));
            }
        }

        #endregion
    }
}