using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Omnifactotum.Tests.Internal;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(EventHandlerAsyncExtensions))]
[NonParallelizable]
internal sealed class EventHandlerAsyncExtensionsTests
{
    private const int SensitiveTestRepeatCount = 100;
    private const int SensitiveTestTimeoutInMilliseconds = 500;
    private const int SensitiveTestRetryCount = 3;

    [OneTimeSetUp]
    public void OneTimeSetUp() => TestFactotum.AdjustThreadPoolSettingsForHigherLoad();

    [SetUp]
    public void SetUp()
    {
        TestFactotum.ReportThreadPoolInformation();
        TestContext.WriteLine();
    }

    [TearDown]
    public void TearDown()
    {
        TestContext.WriteLine();
        TestFactotum.ReportThreadPoolInformation();
    }

    [Test]
    [Timeout(SensitiveTestTimeoutInMilliseconds)]
    [Retry(SensitiveTestRetryCount)]
    public async Task TestInvokeAsync([Range(1, SensitiveTestRepeatCount)] int repeatIndex)
    {
        TestFactotum.ReportCurrentRepeatCount(repeatIndex);

        const int Value = 17;

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var obj = new TestClass();

        var invocationValues = new List<int>();
        var isCorrectSender = true;
        var isCorrectToken = true;

        await obj.InvokeTestActionAsync(Value, cancellationToken);

        Assert.That(isCorrectSender, Is.True);
        Assert.That(isCorrectToken, Is.True);
        Assert.That(invocationValues, Is.Empty);

        obj.TestAction += ProcessActionAsync;
        obj.TestAction += (sender, value, token) => ProcessActionAsync(sender, -value, token);

        await obj.InvokeTestActionAsync(Value, cancellationToken);

        Assert.That(isCorrectSender, Is.True);
        Assert.That(isCorrectToken, Is.True);
        Assert.That(invocationValues, Is.EqualTo(new[] { Value, -Value }));

        Task ProcessActionAsync(object? sender, int value, CancellationToken token)
        {
            invocationValues.Add(value);

            isCorrectSender &= ReferenceEquals(sender, obj);
            isCorrectToken &= token == cancellationToken;

            return Task.CompletedTask;
        }
    }

    [Test]
    [Timeout(SensitiveTestTimeoutInMilliseconds)]
    [Retry(SensitiveTestRetryCount)]
    public async Task TestInvokeParallelAsync([Range(1, SensitiveTestRepeatCount)] int repeatIndex)
    {
        TestFactotum.ReportCurrentRepeatCount(repeatIndex);

        const int Value = 23;

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var obj = new TestClass();

        var invocationValues = new List<int>();
        var isCorrectSender = true;
        var isCorrectToken = true;

        await obj.InvokeTestActionParallelAsync(Value, cancellationToken);

        Assert.That(isCorrectSender, Is.True);
        Assert.That(isCorrectToken, Is.True);
        Assert.That(invocationValues, Is.Empty);

        var isSecondActionExecuted = false;
        var isThirdActionExecuted = false;

        obj.TestAction +=
            async (sender, value, token) =>
            {
                //// ReSharper disable once LoopVariableIsNeverChangedInsideLoop :: False detection
                while (!isThirdActionExecuted)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Yield();
                }

                await ProcessActionAsync(sender, value, token);
            };

        obj.TestAction +=
            async (sender, value, token) =>
            {
                await ProcessActionAsync(sender, value * 2, token);
                isSecondActionExecuted = true;
            };

        obj.TestAction +=
            async (sender, value, token) =>
            {
                while (!isSecondActionExecuted)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Yield();
                }

                await ProcessActionAsync(sender, value * 3, token);
                isThirdActionExecuted = true;
            };

        await obj.InvokeTestActionParallelAsync(Value, cancellationToken);

        Assert.That(isCorrectSender, Is.True);
        Assert.That(isCorrectToken, Is.True);
        Assert.That(invocationValues, Is.EqualTo(new[] { Value * 2, Value * 3, Value }));

        Task ProcessActionAsync(object? sender, int value, CancellationToken token)
        {
            invocationValues.Add(value);

            isCorrectSender &= ReferenceEquals(sender, obj);
            isCorrectToken &= token == cancellationToken;

            return Task.CompletedTask;
        }
    }

    private sealed class TestClass
    {
        public event EventHandlerAsync<int>? TestAction;

        public Task InvokeTestActionAsync(int value, CancellationToken cancellationToken)
            => TestAction.InvokeAsync(this, value, cancellationToken);

        public Task InvokeTestActionParallelAsync(int value, CancellationToken cancellationToken)
            => TestAction.InvokeParallelAsync(this, value, cancellationToken);
    }
}