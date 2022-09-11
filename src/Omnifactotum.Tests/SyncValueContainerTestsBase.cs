using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using NUnit.Framework;
using Omnifactotum.NUnit;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests
{
    [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
    internal abstract class SyncValueContainerTestsBase<TValue> : ValueContainerBaseTestsBase<SyncValueContainer<TValue>, TValue>
        where TValue : IEquatable<TValue>
    {
        protected SyncValueContainerTestsBase(TValue value, TValue anotherValue)
            : base(value, anotherValue)
        {
            // Nothing to do
        }

        protected abstract TValue ValueThreadSafetyInitialValue { get; }

        [OneTimeSetUp]
        public void OneTimeSetUp() => Assert.That(new[] { Value, AnotherValue, ValueThreadSafetyInitialValue }, Is.Unique);

        [Test]
        public override void TestPropertyAccess()
        {
            base.TestPropertyAccess();
            NUnitFactotum.For<SyncValueContainer<TValue>>.AssertReadableWritable(obj => obj.Value, PropertyAccessMode.ReadWrite);
            NUnitFactotum.For<SyncValueContainer<TValue>>.AssertReadableWritable(obj => obj.SyncObject, PropertyAccessMode.ReadOnly);
        }

        [Test]
        public void TestConstructionWithValueAndSyncObject()
        {
            foreach (var value in Values)
            {
                var syncObject = new object();
                var container = CreateContainer(value, syncObject);

                Assert.That(container.SyncObject, Is.Not.Null & Is.SameAs(syncObject));
                Assert.That(container.Value, CreateValueEqualityConstraint(value));
            }
        }

        [Test]
        public void TestConstructionWithValueAndSyncObjectNegative()
        {
            Assert.That(() => CreateContainer(Value, null!), Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => CreateContainer(Value, 123),
                Throws.TypeOf<ArgumentException>().With.Message.EqualTo("The synchronization object cannot be a value type object."));
        }

        [Test]
        public void TestValueThreadSafety()
        {
            const int ConditionWaitTimeoutInSeconds = 1;

            var container = new SyncValueContainer<TValue>(ValueThreadSafetyInitialValue);
            Assert.That(container.Value, CreateValueEqualityConstraint(ValueThreadSafetyInitialValue));

            var canAnotherThreadChangeValue = false;
            var isAnotherThreadEntered = false;
            var isAnotherThreadExited = false;

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            void ExecuteAnotherThread()
            {
                isAnotherThreadEntered = true;

                //// ReSharper disable once AccessToModifiedClosure :: Seems to be false alarm
                //// ReSharper disable once LoopVariableIsNeverChangedInsideLoop :: Changed in another thread
                while (!canAnotherThreadChangeValue)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        isAnotherThreadExited = true;
                        return;
                    }

                    Thread.Sleep(1);
                }

                container.Value = AnotherValue;

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

                Assert.That(compiledCondition(), () => AsInvariant($@"Condition has not been met: {condition}"));
            }

            var thread = new Thread(ExecuteAnotherThread)
            {
                IsBackground = true,
                Name = AsInvariant($@"{nameof(TestValueThreadSafety)}_{nameof(ExecuteAnotherThread)}")
            };

            try
            {
                try
                {
                    thread.Start();
                    WaitForCondition(false, () => isAnotherThreadEntered);
                    WaitForCondition(false, () => !isAnotherThreadExited);
                    Assert.That(container.Value, CreateValueEqualityConstraint(ValueThreadSafetyInitialValue));

                    lock (container.SyncObject)
                    {
                        WaitForCondition(false, () => !isAnotherThreadExited);

                        // ReSharper disable once RedundantAssignment :: False detection
                        canAnotherThreadChangeValue = true;

                        WaitForCondition(true, () => !isAnotherThreadExited);

                        container.Value = Value;
                        Assert.That(container.Value, CreateValueEqualityConstraint(Value));

                        WaitForCondition(true, () => !isAnotherThreadExited);
                    }

                    WaitForCondition(false, () => isAnotherThreadExited);
                    Assert.That(container.Value, CreateValueEqualityConstraint(AnotherValue));
                }
                finally
                {
                    cancellationTokenSource.Cancel();

                    if (!thread.Join(TimeSpan.FromSeconds(ConditionWaitTimeoutInSeconds)))
                    {
                        Assert.Inconclusive(
                            AsInvariant($@"Failed to gracefully finish the thread {thread.Name.ToUIString()} (ID: {thread.ManagedThreadId:N0})."));
                    }
                }
            }
            catch (OperationCanceledException ex)
                when (ex.CancellationToken == cancellationToken)
            {
                Assert.Fail(AsInvariant($@"Cancellation occurred before thread exited."));
            }
        }

        protected override SyncValueContainer<TValue> CreateContainer(TValue value) => new(value);

        protected virtual SyncValueContainer<TValue> CreateContainer(TValue value, object syncObject) => new(value, syncObject);

        protected sealed override TValue GetContainerValue(SyncValueContainer<TValue> container) => container.Value;

        protected sealed override void SetContainerValue(SyncValueContainer<TValue> container, TValue value) => container.Value = value;

        protected sealed override void AssertConstructionWithValueTestCase(SyncValueContainer<TValue> container, TValue value)
        {
            base.AssertConstructionWithValueTestCase(container, value);
            Assert.That(container.SyncObject, Is.Not.Null & Is.TypeOf<object>());
        }
    }
}