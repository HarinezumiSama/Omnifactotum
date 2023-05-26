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
using Omnifactotum.Tests.Internal;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture]
[NonParallelizable]
internal abstract class GenericTaskExtensionsTestsBase
{
    private const int SensitiveTestRepeatCount = 25;
    private const int SensitiveTestTimeoutInMilliseconds = 2_000;
    private const int SensitiveTestRetryCount = 3;

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

    public static IEnumerable<TestCaseData> ConfigureAwaitNoCapturedContextTestCases
    {
        get
        {
            for (var repeatIndex = 1; repeatIndex <= SensitiveTestRepeatCount; repeatIndex++)
            {
                yield return new TestCaseData(repeatIndex, ConfigureAwaitMode.StandardWithTrue, true);
                yield return new TestCaseData(repeatIndex, ConfigureAwaitMode.StandardWithFalse, false);
                yield return new TestCaseData(repeatIndex, ConfigureAwaitMode.TesteeExtensionMethod, false);
            }
        }
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TestFactotum.AdjustThreadPoolSettingsForHigherLoad();

        TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => TaskScheduler.UnobservedTaskException -= OnTaskSchedulerUnobservedTaskException;

    [SetUp]
    public void SetUp()
    {
        TestFactotum.ReportThreadPoolInformation("BEFORE");
        TestContext.WriteLine();

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

        TestFactotum.ReportThreadPoolInformation("AFTER");
        TestContext.WriteLine();
    }

    [TearDown]
    public void TearDown()
    {
        TestContext.WriteLine();
        TestFactotum.ReportThreadPoolInformation("BEFORE");

        SynchronizationContext.SetSynchronizationContext(_previousSynchronizationContext);
        _synchronizationContextMock = null;

        TestContext.WriteLine();
        TestFactotum.ReportThreadPoolInformation("AFTER");
    }

    [Test]
    [TestCaseSource(nameof(ConfigureAwaitNoCapturedContextTestCases))]
    [Timeout(SensitiveTestTimeoutInMilliseconds)]
    [Retry(SensitiveTestRetryCount)]
    public void TestConfigureAwaitNoCapturedContextForVoidTask(
        int repeatIndex,
        ConfigureAwaitMode mode,
        bool isExpectedOnContext)
        => InternalTestConfigureAwaitNoCapturedContext(repeatIndex, mode, isExpectedOnContext, OnRunTestCaseForVoidTaskAsync);

    [Test]
    [TestCaseSource(nameof(ConfigureAwaitNoCapturedContextTestCases))]
    [Timeout(SensitiveTestTimeoutInMilliseconds)]
    [Retry(SensitiveTestRetryCount)]
    public void TestConfigureAwaitNoCapturedContextForTaskWithResult(
        int repeatIndex,
        ConfigureAwaitMode mode,
        bool isExpectedOnContext)
        => InternalTestConfigureAwaitNoCapturedContext(repeatIndex, mode, isExpectedOnContext, OnRunTestCaseForTaskWithResultAsync);

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
        int repeatIndex,
        ConfigureAwaitMode mode,
        bool isExpectedOnContext,
        Func<ConfigureAwaitMode, ValueContainer<int>, Task> runTestCaseAsync)
    {
        TestFactotum.ReportCurrentRepeatCount(repeatIndex);

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
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract", Justification = ".NET version dependent.")]
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