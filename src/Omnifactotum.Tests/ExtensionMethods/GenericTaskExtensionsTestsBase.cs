using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture]
    internal abstract class GenericTaskExtensionsTestsBase
    {
        private const int SensitiveTestRepeatCount = 25;
        private const int SensitiveTestTimeoutInMilliseconds = 2_000;

        private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(25);

        private static readonly MethodInfo SynchronizationContextSendMethodInfo =
            typeof(SynchronizationContext)
                .GetMethod(nameof(SynchronizationContext.Send), BindingFlags.Instance | BindingFlags.Public)
                .EnsureNotNull();

        private static readonly MethodInfo SynchronizationContextPostMethodInfo =
            typeof(SynchronizationContext)
                .GetMethod(nameof(SynchronizationContext.Post), BindingFlags.Instance | BindingFlags.Public)
                .EnsureNotNull();

        private readonly List<Exception> _unobservedTaskExceptions = new();

        private Mock<SynchronizationContext>? _synchronizationContextMock;
        private SynchronizationContext? _previousSynchronizationContext;

        [OneTimeSetUp]
        public void OneTimeSetUp() => TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;

        [OneTimeTearDown]
        public void OneTimeTearDown() => TaskScheduler.UnobservedTaskException -= OnTaskSchedulerUnobservedTaskException;

        [SetUp]
        public void SetUp()
        {
            _unobservedTaskExceptions.Clear();

            _synchronizationContextMock = new Mock<SynchronizationContext>(MockBehavior.Strict) { CallBase = false };

            _synchronizationContextMock
                .Setup(context => context.Post(It.IsAny<SendOrPostCallback>(), It.IsAny<object?>()))
                .CallBase();

            _synchronizationContextMock
                .Setup(context => context.Send(It.IsAny<SendOrPostCallback>(), It.IsAny<object?>()))
                .CallBase();

            _previousSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(_synchronizationContextMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            SynchronizationContext.SetSynchronizationContext(_previousSynchronizationContext);
            _synchronizationContextMock = null;
        }

        [Test]
        [TestCase(ConfigureAwaitMode.StandardWithTrue, true)]
        [TestCase(ConfigureAwaitMode.StandardWithFalse, false)]
        [TestCase(ConfigureAwaitMode.TesteeExtensionMethod, false)]
        [Timeout(SensitiveTestTimeoutInMilliseconds)]
        [Repeat(SensitiveTestRepeatCount)]
        public void TestConfigureAwaitNoCapturedContextForVoidTask(ConfigureAwaitMode mode, bool isExpectedOnContext)
            => InternalTestConfigureAwaitNoCapturedContext(mode, isExpectedOnContext, OnRunTestCaseForVoidTaskAsync);

        [Test]
        [TestCase(ConfigureAwaitMode.StandardWithTrue, true)]
        [TestCase(ConfigureAwaitMode.StandardWithFalse, false)]
        [TestCase(ConfigureAwaitMode.TesteeExtensionMethod, false)]
        [Timeout(SensitiveTestTimeoutInMilliseconds)]
        [Repeat(SensitiveTestRepeatCount)]
        public void TestConfigureAwaitNoCapturedContextForTaskWithResult(ConfigureAwaitMode mode, bool isExpectedOnContext)
            => InternalTestConfigureAwaitNoCapturedContext(mode, isExpectedOnContext, OnRunTestCaseForTaskWithResultAsync);

        protected static Task CreateNonCompleteTask() => Task.Delay(WaitInterval);

        protected static ConfiguredTaskAwaitable ConfigureAwait(ConfigureAwaitMode mode, Task task)
            => mode switch
            {
                ConfigureAwaitMode.StandardWithTrue => task.ConfigureAwait(true),
                ConfigureAwaitMode.StandardWithFalse => task.ConfigureAwait(false),
                ConfigureAwaitMode.TesteeExtensionMethod => task.ConfigureAwaitNoCapturedContext(),
                _ => throw mode.CreateEnumValueNotImplementedException()
            };

        protected static ConfiguredTaskAwaitable<int> ConfigureAwait(ConfigureAwaitMode mode, Task<int> task)
            => mode switch
            {
                ConfigureAwaitMode.StandardWithTrue => task.ConfigureAwait(true),
                ConfigureAwaitMode.StandardWithFalse => task.ConfigureAwait(false),
                ConfigureAwaitMode.TesteeExtensionMethod => task.ConfigureAwaitNoCapturedContext(),
                _ => throw mode.CreateEnumValueNotImplementedException()
            };

        protected static ConfiguredValueTaskAwaitable ConfigureAwait(ConfigureAwaitMode mode, ValueTask task)
            => mode switch
            {
                ConfigureAwaitMode.StandardWithTrue => task.ConfigureAwait(true),
                ConfigureAwaitMode.StandardWithFalse => task.ConfigureAwait(false),
                ConfigureAwaitMode.TesteeExtensionMethod => task.ConfigureAwaitNoCapturedContext(),
                _ => throw mode.CreateEnumValueNotImplementedException()
            };

        protected static ConfiguredValueTaskAwaitable<int> ConfigureAwait(ConfigureAwaitMode mode, ValueTask<int> task)
            => mode switch
            {
                ConfigureAwaitMode.StandardWithTrue => task.ConfigureAwait(true),
                ConfigureAwaitMode.StandardWithFalse => task.ConfigureAwait(false),
                ConfigureAwaitMode.TesteeExtensionMethod => task.ConfigureAwaitNoCapturedContext(),
                _ => throw mode.CreateEnumValueNotImplementedException()
            };

        protected void AssertSynchronizationContext()
            => Assert.That(() => SynchronizationContext.Current, Is.SameAs(_synchronizationContextMock!.Object));

        protected abstract Task OnRunTestCaseForVoidTaskAsync(ConfigureAwaitMode mode, ValueContainer<int> container);

        protected abstract Task OnRunTestCaseForTaskWithResultAsync(ConfigureAwaitMode mode, ValueContainer<int> container);

        private void InternalTestConfigureAwaitNoCapturedContext(
            ConfigureAwaitMode mode,
            bool isExpectedOnContext,
            Func<ConfigureAwaitMode, ValueContainer<int>, Task> runTestCaseAsync)
        {
            TestContext.WriteLine(
                $@"{nameof(TestContext.CurrentContext.CurrentRepeatCount)} = {TestContext.CurrentContext.CurrentRepeatCount}");

            var callCountValueContainer = ValueContainer.Create(0);
            runTestCaseAsync(mode, callCountValueContainer).Wait();

            Assert.That(callCountValueContainer.Value, Is.EqualTo(1));

            Assert.That(_unobservedTaskExceptions, Is.Empty);

            var invocationCount = _synchronizationContextMock!.Invocations.Count(
                invocation => invocation.Method == SynchronizationContextSendMethodInfo
                    || invocation.Method == SynchronizationContextPostMethodInfo);

            // 2 await-s: one is for `IncrementAsync` and another one is inside `IncrementAsync`
            Assert.That(invocationCount, Is.EqualTo(isExpectedOnContext ? 2 : 0));
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = ".NET version dependent.")]
        private void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Exception is null)
            {
                return;
            }

            _unobservedTaskExceptions.Add(e.Exception);
            e.SetObserved();
        }

        internal enum ConfigureAwaitMode
        {
            StandardWithTrue,
            StandardWithFalse,
            TesteeExtensionMethod
        }
    }
}