using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests
{
    internal abstract class SyncValueContainerTestsBase<T>
        where T : IEquatable<T>
    {
        private readonly T _value;
        private readonly T _anotherValue;
        private readonly T[] _values;

        protected SyncValueContainerTestsBase(T value, T anotherValue)
        {
            Assert.That(value, Is.Not.EqualTo(default(T)));

            _value = value;
            _anotherValue = anotherValue;
            _values = new[] { value, anotherValue, default(T) };
        }

        [Test]
        public void TestSupportedInterfaces()
        {
            Assert.That(typeof(IValueContainer<T>).IsAssignableFrom(typeof(SyncValueContainer<T>)), Is.True);
        }

        [Test]
        public void TestPropertyAccess()
        {
            NUnitFactotum.For<SyncValueContainer<T>>.AssertReadableWritable(
                obj => obj.SyncObject,
                PropertyAccessMode.ReadOnly);

            NUnitFactotum.For<SyncValueContainer<T>>.AssertReadableWritable(
                obj => obj.Value,
                PropertyAccessMode.ReadWrite);
        }

        [Test]
        public void TestConstructionDefault()
        {
            var container = new SyncValueContainer<T>();

            Assert.That(container.SyncObject, Is.Not.Null & Is.TypeOf<object>());
            Assert.That(container.Value, GetDefaultValueConstraint());
        }

        [Test]
        public void TestConstructionWithValue()
        {
            foreach (var value in _values)
            {
                var container = new SyncValueContainer<T>(value);

                Assert.That(container.SyncObject, Is.Not.Null & Is.TypeOf<object>());
                Assert.That(container.Value, GetEqualityConstraint(value));
            }
        }

        [Test]
        public void TestConstructionWithValueAndSyncObject()
        {
            foreach (var value in _values)
            {
                var syncObject = new object();
                var container = new SyncValueContainer<T>(value, syncObject);

                Assert.That(container.SyncObject, Is.Not.Null & Is.SameAs(syncObject));
                Assert.That(container.Value, GetEqualityConstraint(value));
            }
        }

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute", Justification = "Negative test case.")]
        public void TestConstructionWithValueAndSyncObjectNegative()
        {
            Assert.That(() => new SyncValueContainer<T>(_value, null), Throws.TypeOf<ArgumentNullException>());

            Assert.That(() => new SyncValueContainer<T>(_value, 123), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void TestValue()
        {
            var container = new SyncValueContainer<T>(_value);
            Assert.That(container.Value, GetEqualityConstraint(_value));

            container.Value = _anotherValue;
            Assert.That(container.Value, GetEqualityConstraint(_anotherValue));
        }

        [Test]
        public void TestValueThreadSafety()
        {
            const int ConditionWaitTimeoutInSeconds = 1;

            var container = new SyncValueContainer<T>();
            Assert.That(container.Value, GetDefaultValueConstraint());

            var canAnotherThreadChangeValue = false;
            var isAnotherThreadEntered = false;
            var isAnotherThreadExited = false;

            void ExecuteAnotherThread()
            {
                isAnotherThreadEntered = true;

                while (!canAnotherThreadChangeValue)
                {
                    Thread.Sleep(1);
                }

                container.Value = _anotherValue;

                isAnotherThreadExited = true;
            }

            void WaitForCondition(bool alwaysWait, Expression<Func<bool>> condition)
            {
                var compiledCondition = condition.Compile();

                var stopwatch = Stopwatch.StartNew();
                while (alwaysWait || !compiledCondition())
                {
                    if (stopwatch.Elapsed.TotalSeconds > ConditionWaitTimeoutInSeconds)
                    {
                        break;
                    }

                    Thread.Sleep(1);
                }

                Assert.That(compiledCondition(), $@"Condition has not been met: {condition}");
            }

            var thread = new Thread(ExecuteAnotherThread)
            {
                IsBackground = true,
                Name = $@"{nameof(TestValueThreadSafety)}_{nameof(ExecuteAnotherThread)}"
            };
            try
            {
                thread.Start();
                WaitForCondition(false, () => isAnotherThreadEntered);
                WaitForCondition(false, () => !isAnotherThreadExited);
                Assert.That(container.Value, GetDefaultValueConstraint());

                lock (container.SyncObject)
                {
                    WaitForCondition(false, () => !isAnotherThreadExited);
                    canAnotherThreadChangeValue = true;
                    WaitForCondition(true, () => !isAnotherThreadExited);

                    container.Value = _value;
                    Assert.That(container.Value, GetEqualityConstraint(_value));

                    WaitForCondition(true, () => !isAnotherThreadExited);
                }

                WaitForCondition(false, () => isAnotherThreadExited);
                Assert.That(container.Value, GetEqualityConstraint(_anotherValue));
            }
            finally
            {
                try
                {
                    if (thread.IsAlive)
                    {
                        thread.Abort();
                    }
                }
                catch (Exception)
                {
                    // Nothing to do
                }
            }
        }

        [Test]
        public void TestEquality()
        {
            var container1 = new SyncValueContainer<T>(_value);
            var container2 = new SyncValueContainer<T>(_value);
            var containerAnother = new SyncValueContainer<T>(_anotherValue);
            var containerDefault = new SyncValueContainer<T>();

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
                var container = new SyncValueContainer<T>(value);
                Assert.That(
                    container.ToString(),
                    Is.EqualTo($@"{{ {nameof(SyncValueContainer<T>.Value)} = {value.ToStringSafelyInvariant()} }}"));
            }
        }

        private static IResolveConstraint GetEqualityConstraint(T value)
            => typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(value) : Is.SameAs(value);

        private static IResolveConstraint GetDefaultValueConstraint()
            => typeof(T).IsValueType ? (IResolveConstraint)Is.EqualTo(default(T)) : Is.Null;
    }
}