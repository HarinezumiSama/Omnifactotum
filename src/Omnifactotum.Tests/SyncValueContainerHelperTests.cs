using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests
{
    //// ReSharper disable AssignNullToNotNullAttribute - for negative test cases

    [TestFixture(typeof(int), 1, int.MaxValue)]
    [TestFixture(typeof(string), "Some value", "Another value")]
    public sealed class SyncValueContainerHelperTests<T>
        where T : IEquatable<T>
    {
        #region Constants and Fields

        private readonly T _value;
        private readonly T[] _values;

        #endregion

        #region Constructors

        public SyncValueContainerHelperTests(T value, T anotherValue)
        {
            Assert.That(value, Is.Not.EqualTo(default(T)));

            _value = value;
            _values = new[] { value, anotherValue, default(T) };
        }

        #endregion

        #region Tests

        [Test]
        public void TestCreateWithValue()
        {
            foreach (var value in _values)
            {
                var container = SyncValueContainer.Create(value);

                Assert.That(container.SyncObject, Is.Not.Null & Is.TypeOf<object>());

                Assert.That(
                    container.Value,
                    typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(value) : Is.SameAs(value));
            }
        }

        [Test]
        public void TestCreateWithValueAndSyncObject()
        {
            foreach (var value in _values)
            {
                var syncObject = new object();
                var container = SyncValueContainer.Create(value, syncObject);

                Assert.That(container.SyncObject, Is.Not.Null & Is.SameAs(syncObject));

                Assert.That(
                    container.Value,
                    typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(value) : Is.SameAs(value));
            }
        }

        [Test]
        public void TestCreateWithValueAndSyncObjectNegative()
        {
            Assert.That(() => SyncValueContainer.Create(_value, null), Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => SyncValueContainer.Create(_value, 123), Throws.TypeOf<ArgumentException>());
        }

        #endregion
    }
}