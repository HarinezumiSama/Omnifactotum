using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Omnifactotum.Threading;

namespace Omnifactotum.Tests.Threading;

[TestFixture(TestOf = typeof(SemaphoreSlimBasedLock))]
internal sealed class SemaphoreSlimBasedLockTests
{
    private const int WaitIntervalInMilliseconds = 20;

    private const int TimeSensitiveTestRepeatCount = 25;
    private const int TimeSensitiveTestTimeout = WaitIntervalInMilliseconds * 50;

    private static readonly TimeSpan WaitInterval = TimeSpan.FromMilliseconds(WaitIntervalInMilliseconds);
    private static readonly TimeSpan ConditionTimeout = TimeSpan.FromMilliseconds(WaitIntervalInMilliseconds * 10);

    [OneTimeSetUp]
    public void OneTimeSetUp() => AdjustThreadPoolSettingsForHigherLoad();

    [SetUp]
    [SuppressMessage("Interoperability", "CA1416")]
    public void SetUp() => ReportThreadPoolInformation();

    [TearDown]
    public void TearDown() => ReportThreadPoolInformation();

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void TestConstructionWhenInvalidArgumentThenThrows(int count)
        => Assert.That(
            () => CreateTestee(count),
            Throws
                .TypeOf<ArgumentOutOfRangeException>()
                .With
                .Property(nameof(ArgumentException.ParamName))
                .EqualTo("count"));

    [Test]
    public void TestConstructionWhenNoArgumentsThenSucceeds()
    {
        using var testee = CreateTestee();
        Assert.That(testee.Count, Is.EqualTo(1));
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(17)]
    [TestCase(int.MaxValue)]
    public void TestConstructionWhenValidArgumentsThenSucceeds(int count)
    {
        using var testee = CreateTestee(count);
        Assert.That(testee.Count, Is.EqualTo(count));
    }

    [Test]
    [Timeout(TimeSensitiveTestTimeout)]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void TestAcquireWhenSingleAllowed(
        [Values] bool useExplicitCount,
        [Range(1, TimeSensitiveTestRepeatCount)] int repeatIndex)
    {
        using var testee = useExplicitCount ? CreateTestee(1) : CreateTestee();

        var protectedResource = 0;
        var threadState = WorkItemState.NotStarted;

        var thread =
            new Thread(
                () =>
                {
                    threadState = WorkItemState.Started;
                    using (testee.Acquire())
                    {
                        threadState = WorkItemState.EnteredLock;
                        protectedResource++;
                    }

                    threadState = WorkItemState.Finished;
                })
            {
                IsBackground = true,
                Name = MethodBase.GetCurrentMethod().EnsureNotNull().GetQualifiedName()
            };

        using (testee.Acquire())
        {
            thread.Start();
            Thread.Sleep(WaitInterval);
            Assert.That(protectedResource, Is.Zero);
            Assert.That(threadState, Is.EqualTo(WorkItemState.Started));
        }

        Thread.Sleep(WaitInterval);
        Assert.That(protectedResource, Is.EqualTo(1));
        Assert.That(threadState, Is.EqualTo(WorkItemState.Finished));

        using (testee.Acquire())
        {
            Assert.That(protectedResource, Is.EqualTo(1));
        }
    }

    [Test]
    [Timeout(TimeSensitiveTestTimeout)]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void TestAcquireWhenCancelledWhileWaiting(
        [Range(1, TimeSensitiveTestRepeatCount)] int repeatIndex)
    {
        using var testee = CreateTestee();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var protectedResource = 0;
        var threadState = WorkItemState.NotStarted;
        Exception? threadUnhandledException = null;

        var thread =
            new Thread(
                () =>
                {
                    threadState = WorkItemState.Started;
                    try
                    {
                        using (testee.Acquire(cancellationToken))
                        {
                            threadState = WorkItemState.EnteredLock;
                            protectedResource++;
                        }
                    }
                    catch (OperationCanceledException ex)
                        when (ex.CancellationToken == cancellationToken)
                    {
                        threadState = WorkItemState.Cancelled;
                        return;
                    }
                    catch (Exception ex)
                    {
                        threadState = WorkItemState.UnhandledException;
                        threadUnhandledException = ex;
                        return;
                    }

                    threadState = WorkItemState.Finished;
                })
            {
                IsBackground = true,
                Name = MethodBase.GetCurrentMethod().EnsureNotNull().GetQualifiedName()
            };

        using (testee.Acquire())
        {
            thread.Start();
            Thread.Sleep(WaitInterval);
            Assert.That(threadState, Is.EqualTo(WorkItemState.Started));
            Assert.That(protectedResource, Is.Zero);

            cancellationTokenSource.Cancel();
            Thread.Sleep(WaitInterval);
            Assert.That(threadUnhandledException, Is.Null);
            Assert.That(threadState, Is.EqualTo(WorkItemState.Cancelled));
            Assert.That(protectedResource, Is.Zero);
        }

        Thread.Sleep(WaitInterval);
        Assert.That(protectedResource, Is.Zero);
        Assert.That(threadUnhandledException, Is.Null);
        Assert.That(threadState, Is.EqualTo(WorkItemState.Cancelled));

        Assert.That(
            () => testee.Acquire(cancellationToken),
            Throws
                .InstanceOf<OperationCanceledException>()
                .With
                .Property(nameof(OperationCanceledException.CancellationToken))
                .EqualTo(cancellationToken));
    }

    [Test]
    [Timeout(TimeSensitiveTestTimeout)]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public void TestAcquireWhenSpecifiedCountAllowed(
        [Values(2, 7)] int count,
        [Range(1, TimeSensitiveTestRepeatCount)] int repeatIndex)
    {
        var methodName = MethodBase.GetCurrentMethod().EnsureNotNull().GetQualifiedName();

        using var testee = CreateTestee(count);

        var protectedResources = new int[count];

        var threadStates = new WorkItemState[count];
        threadStates.Initialize(WorkItemState.NotStarted);

        var lockHolders = Enumerable
            .Range(0, count)
            .Select(_ => testee.Acquire())
            .ToArray();

        var threads = Enumerable
            .Range(0, count)
            .Select(
                index => new Thread(
                    () =>
                    {
                        threadStates[index] = WorkItemState.Started;

                        // Need to hold the lock forever so that a released lock is not taken by another thread from this list
                        testee.Acquire().EnsureNotNull();
                        threadStates[index] = WorkItemState.EnteredLock;

                        protectedResources[index]++;
                        threadStates[index] = WorkItemState.Finished;
                    })
                {
                    IsBackground = true,
                    Name = $"{methodName}#{index:00}"
                })
            .ToArray();

        threads.DoForEach(thread => thread.Start());

        for (var index = 0; index < count; index++)
        {
            Thread.Sleep(WaitInterval);
            AssertCompleted(index);

            lockHolders[index].Dispose();
        }

        Thread.Sleep(WaitInterval);
        AssertCompleted(count);

        Assert.That(() => threads.Select(thread => thread.Join(0)), Is.EqualTo(Enumerable.Repeat(true, count)));

        void AssertCompleted(int completedCount)
        {
            Assert.That(completedCount, Is.GreaterThanOrEqualTo(0) & Is.LessThanOrEqualTo(count));

            var nonCompletedCount = count - completedCount;

            Assert.That(protectedResources.Count(item => item == 0), Is.EqualTo(nonCompletedCount));
            Assert.That(protectedResources.Count(item => item == 1), Is.EqualTo(completedCount));

            Assert.That(threadStates.Count(state => state == WorkItemState.Started), Is.EqualTo(nonCompletedCount));
            Assert.That(threadStates.Count(state => state == WorkItemState.Finished), Is.EqualTo(completedCount));
        }
    }

    [Test]
    [Timeout(TimeSensitiveTestTimeout)]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task TestAcquireAsyncWhenSingleAllowed(
        [Values] bool useExplicitCount,
        [Range(1, TimeSensitiveTestRepeatCount)] int repeatIndex)
    {
        using var testee = useExplicitCount ? CreateTestee(1) : CreateTestee();

        var protectedResource = 0;
        var taskState = WorkItemState.NotStarted;

        async Task RunAsync()
        {
            taskState = WorkItemState.Started;
            using (await testee.AcquireAsync())
            {
                taskState = WorkItemState.EnteredLock;
                protectedResource++;
            }

            taskState = WorkItemState.Finished;
        }

        Task task;
        using (await testee.AcquireAsync())
        {
            task = Task.Run(async () => await RunAsync().ConfigureAwait(false));
            await WaitForConditionAsync(() => taskState == WorkItemState.Started);

            Assert.That(taskState, Is.EqualTo(WorkItemState.Started));
            Assert.That(protectedResource, Is.Zero);
        }

        await Task.Delay(WaitInterval);
        Assert.That(taskState, Is.EqualTo(WorkItemState.Finished));
        Assert.That(protectedResource, Is.EqualTo(1));
        Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));

        using (await testee.AcquireAsync())
        {
            Assert.That(protectedResource, Is.EqualTo(1));
        }
    }

    [Test]
    [Timeout(TimeSensitiveTestTimeout)]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task TestAcquireAsyncWhenCancelledWhileWaiting(
        [Range(1, TimeSensitiveTestRepeatCount)] int repeatIndex)
    {
        using var testee = CreateTestee();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var protectedResource = 0;
        var taskState = WorkItemState.NotStarted;
        Exception? taskUnhandledException = null;

        async Task RunAsync()
        {
            taskState = WorkItemState.Started;
            try
            {
                using (await testee.AcquireAsync(cancellationToken))
                {
                    taskState = WorkItemState.EnteredLock;
                    protectedResource++;
                }
            }
            catch (OperationCanceledException ex)
                when (ex.CancellationToken == cancellationToken)
            {
                taskState = WorkItemState.Cancelled;
                return;
            }
            catch (Exception ex)
            {
                taskState = WorkItemState.UnhandledException;
                taskUnhandledException = ex;
                return;
            }

            taskState = WorkItemState.Finished;
        }

        Task task;
        using (await testee.AcquireAsync(CancellationToken.None))
        {
            task = Task.Run(async () => await RunAsync().ConfigureAwait(false), CancellationToken.None);
            await WaitForConditionAsync(() => taskState == WorkItemState.Started);

            Assert.That(taskUnhandledException, Is.Null);
            Assert.That(taskState, Is.EqualTo(WorkItemState.Started));
            Assert.That(protectedResource, Is.Zero);

            cancellationTokenSource.Cancel();
            await Task.Delay(WaitInterval, CancellationToken.None);
            Assert.That(taskUnhandledException, Is.Null);
            Assert.That(taskState, Is.EqualTo(WorkItemState.Cancelled));
            Assert.That(protectedResource, Is.Zero);
        }

        await Task.Delay(WaitInterval, CancellationToken.None);
        Assert.That(taskUnhandledException, Is.Null);
        Assert.That(taskState, Is.EqualTo(WorkItemState.Cancelled));
        Assert.That(protectedResource, Is.Zero);
        Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));

        Assert.That(
            async () => await testee.AcquireAsync(cancellationToken),
            Throws
                .InstanceOf<OperationCanceledException>()
                .With
                .Property(nameof(OperationCanceledException.CancellationToken))
                .EqualTo(cancellationToken));
    }

    [Test]
    [Timeout(TimeSensitiveTestTimeout)]
    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task TestAcquireAsyncWhenSpecifiedCountAllowed(
        [Values(2, 7)] int count,
        [Range(1, TimeSensitiveTestRepeatCount)] int repeatIndex)
    {
        using var testee = CreateTestee(count);

        var protectedResources = new int[count];

        var taskStates = new WorkItemState[count];
        taskStates.Initialize(WorkItemState.NotStarted);

        var lockHolders = await Task.WhenAll(Enumerable.Range(0, count).Select(_ => testee.AcquireAsync()));

        async Task RunAsync(int index)
        {
            taskStates[index] = WorkItemState.Started;

            // Need to hold the lock forever so that a released lock is not taken by another task from the list
            await testee.AcquireAsync().EnsureNotNull();
            taskStates[index] = WorkItemState.EnteredLock;

            protectedResources[index]++;
            taskStates[index] = WorkItemState.Finished;
        }

        var tasks = Enumerable.Range(0, count).Select(RunAsync).ToArray();
        await WaitForConditionAsync(() => taskStates.All(state => state == WorkItemState.Started));

        for (var index = 0; index < count; index++)
        {
            Thread.Sleep(WaitInterval);
            AssertCompleted(index);

            lockHolders[index].Dispose();
        }

        Thread.Sleep(WaitInterval);
        AssertCompleted(count);

        Assert.That(() => tasks.Select(task => task.Status), Is.EqualTo(Enumerable.Repeat(TaskStatus.RanToCompletion, count)));

        void AssertCompleted(int completedCount)
        {
            Assert.That(completedCount, Is.GreaterThanOrEqualTo(0) & Is.LessThanOrEqualTo(count));

            var nonCompletedCount = count - completedCount;

            Assert.That(protectedResources.Count(item => item == 0), Is.EqualTo(nonCompletedCount));
            Assert.That(protectedResources.Count(item => item == 1), Is.EqualTo(completedCount));

            Assert.That(taskStates.Count(state => state == WorkItemState.Started), Is.EqualTo(nonCompletedCount));
            Assert.That(taskStates.Count(state => state == WorkItemState.Finished), Is.EqualTo(completedCount));
        }
    }

    private static async Task WaitForConditionAsync(Func<bool> condition)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed <= ConditionTimeout)
        {
            if (condition())
            {
                return;
            }

            await Task.Delay(WaitInterval);
        }

        throw new TimeoutException(
            $@"Failed to transition to the required state within the allotted time ({ConditionTimeout}).");
    }

    private static SemaphoreSlimBasedLock CreateTestee(int count) => new(count);

    private static SemaphoreSlimBasedLock CreateTestee() => new();

    private static void ReportThreadPoolInformation(string? suffix = null, [CallerMemberName] string callerName = null!)
    {
        var marker = new[] { callerName, suffix }.Where(s => !s.IsNullOrWhiteSpace()).Join("\x0020:\x0020");

        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
        ThreadPool.GetAvailableThreads(out var availableWorkerThreads, out var availableCompletionPortThreads);

        var parts = new[]
        {
#if NETCOREAPP3_1_OR_GREATER
            $@"{nameof(ThreadPool)}.{nameof(ThreadPool.ThreadCount)} = {ThreadPool.ThreadCount}",
#endif
            $@"{nameof(minWorkerThreads)} = {minWorkerThreads}",
            $@"{nameof(minCompletionPortThreads)} = {minCompletionPortThreads}",
            $@"{nameof(maxWorkerThreads)} = {maxWorkerThreads}",
            $@"{nameof(maxCompletionPortThreads)} = {maxCompletionPortThreads}",
            $@"{nameof(availableWorkerThreads)} = {availableWorkerThreads}",
            $@"{nameof(availableCompletionPortThreads)} = {availableCompletionPortThreads}"
        };

        TestContext.WriteLine($@"[{marker}] {parts.Join(",\x0020")}");
    }

    private static void AdjustThreadPoolSettingsForHigherLoad([CallerMemberName] string callerName = null!)
    {
        const int RequiredMinWorkerThreads = sbyte.MaxValue;

        var fullCallerName = $@"{callerName} > {nameof(AdjustThreadPoolSettingsForHigherLoad)}";

        ReportThreadPoolInformation("BEFORE", fullCallerName);
        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.SetMinThreads(Math.Max(minWorkerThreads, RequiredMinWorkerThreads), minCompletionPortThreads);
        ReportThreadPoolInformation("AFTER", fullCallerName);
    }

    private enum WorkItemState
    {
        NotStarted,
        Started,
        EnteredLock,
        Cancelled,
        UnhandledException,
        Finished
    }
}