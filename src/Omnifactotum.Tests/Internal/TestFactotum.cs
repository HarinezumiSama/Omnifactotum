using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.Internal;

internal static class TestFactotum
{
    public static void ReportThreadPoolInformation(string? suffix = null, [CallerMemberName] string callerName = null!)
    {
        var marker = new[] { callerName, suffix }.Where(s => !s.IsNullOrWhiteSpace()).Join("\x0020:\x0020");

        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
        ThreadPool.GetAvailableThreads(out var availableWorkerThreads, out var availableCompletionPortThreads);

        var parts = new[]
        {
            $"{nameof(ThreadPool)}.{nameof(ThreadPool.ThreadCount)} = {ThreadPool.ThreadCount}",
            $"{nameof(ThreadPool)}.{nameof(ThreadPool.CompletedWorkItemCount)} = {ThreadPool.CompletedWorkItemCount}",
            $"{nameof(ThreadPool)}.{nameof(ThreadPool.PendingWorkItemCount)} = {ThreadPool.PendingWorkItemCount}",
            $"{nameof(minWorkerThreads)} = {minWorkerThreads}",
            $"{nameof(minCompletionPortThreads)} = {minCompletionPortThreads}",
            $"{nameof(maxWorkerThreads)} = {maxWorkerThreads}",
            $"{nameof(maxCompletionPortThreads)} = {maxCompletionPortThreads}",
            $"{nameof(availableWorkerThreads)} = {availableWorkerThreads}",
            $"{nameof(availableCompletionPortThreads)} = {availableCompletionPortThreads}"
        };

        TestContext.WriteLine($@"[{marker}] {parts.Join(",\x0020")}");

        TestContext.WriteLine();

        TestContext.WriteLine(
            $@"[{marker}] {nameof(SynchronizationContext)}.{nameof(SynchronizationContext.Current)} = {
                SynchronizationContext.Current.GetShortObjectReferenceDescription()}");
    }

    public static void ReportCurrentRepeatCount(int? repeatIndex = null, [CallerArgumentExpression(nameof(repeatIndex))] string? repeatIndexExpression = null)
    {
        var testContext = TestContext.CurrentContext.AssertNotNull();

        var message = $"[{testContext.Test.Name}] {nameof(testContext.CurrentRepeatCount)} = {testContext.CurrentRepeatCount}";

        if (repeatIndex is { } repeatIndexValue)
        {
            var name = repeatIndexExpression.IsNullOrWhiteSpace() ? nameof(repeatIndex) : repeatIndexExpression;
            message += $", {name} = {repeatIndexValue}";
        }

        TestContext.WriteLine(message);
    }

    public static void AdjustThreadPoolSettingsForHigherLoad([CallerMemberName] string callerName = null!)
    {
        const int RequiredMinWorkerThreads = sbyte.MaxValue;

        var fullCallerName = $@"{callerName} > {nameof(AdjustThreadPoolSettingsForHigherLoad)}";

        ReportThreadPoolInformation("BEFORE", fullCallerName);
        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.SetMinThreads(Math.Max(minWorkerThreads, RequiredMinWorkerThreads), minCompletionPortThreads);
        ReportThreadPoolInformation("AFTER", fullCallerName);
    }
}