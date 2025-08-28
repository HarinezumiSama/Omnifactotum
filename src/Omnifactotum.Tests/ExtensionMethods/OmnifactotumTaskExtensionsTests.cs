using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumTaskExtensions))]
[NonParallelizable]
internal sealed class OmnifactotumTaskExtensionsTests : GenericTaskExtensionsTestsBase
{
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(7)]
    [TestCase(17)]
    public async Task TestAwaitAllAsyncForVoidTaskAsync(int count)
    {
        var flags = new int[count];

        async Task ExecuteActionAsync(int index)
        {
            await Task.Delay(0);
            flags[index]++;
        }

        await Enumerable
            .Range(0, count)
            .Select(ExecuteActionAsync)
            .AwaitAllAsync();

        Assert.That(flags, Is.EqualTo(Enumerable.Repeat(1, count)));
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(7)]
    [TestCase(17)]
    public async Task TestAwaitAllAsyncForTaskWithResultAsync(int count)
    {
        async Task<int> ExecuteActionAsync(int index)
        {
            await Task.Delay(0);
            return -index;
        }

        var actualResult = await Enumerable
            .Range(0, count)
            .Select(ExecuteActionAsync)
            .AwaitAllAsync();

        Assert.That(actualResult, Is.EqualTo(Enumerable.Range(0, count).Select(i => -i)));
    }

#if NET5_0_OR_GREATER
    [Test]
    public void TestEnsureNotNullAsyncForReferenceType()
    {
        const string ExpectedValue = "320434148ba94220927556f406b3acc0";

        Assert.That(
            async () => await default(Task<string?>)!.EnsureNotNullAsync(),
            Throws.ArgumentNullException.With.Message.EqualTo(
                "The following expression is null: { default(Task<string?>) }. (Parameter 'resultTask')"));

        Assert.That(
            async () => await Task.FromResult<string?>(null).EnsureNotNullAsync(),
            Throws.ArgumentNullException.With.Message.EqualTo(
                "The following expression is null: { await Task.FromResult<string?>(null) }. (Parameter 'value')"));

        Assert.That(
            async () => await Task.FromResult<string?>(ExpectedValue).EnsureNotNullAsync(),
            Is.EqualTo(ExpectedValue));
    }

    [Test]
    public void TestEnsureNotNullAsyncForValueType()
    {
        const int ExpectedValue = 17;

        Assert.That(
            async () => await default(Task<int?>)!.EnsureNotNullAsync(),
            Throws.ArgumentNullException.With.Message.EqualTo(
                "The following expression is null: { default(Task<int?>) }. (Parameter 'resultTask')"));

        Assert.That(
            async () => await Task.FromResult<int?>(null).EnsureNotNullAsync(),
            Throws.ArgumentNullException.With.Message.EqualTo(
                "The following expression is null: { await Task.FromResult<int?>(null) }. (Parameter 'value')"));

        Assert.That(
            async () => await Task.FromResult<int?>(ExpectedValue).EnsureNotNullAsync(),
            Is.EqualTo(ExpectedValue));
    }
#endif

    protected override Task OnRunTestCaseForVoidTaskAsync(ConfigureAwaitMode mode, ValueContainer<int> container)
    {
        async Task IncrementAsync()
        {
            await ConfigureAwait(mode, CreateNonCompleteTask());
            container.Value++;
        }

        async Task ExecuteAsync()
        {
            AssertSynchronizationContext();

            var taskAwaitable = ConfigureAwait(mode, IncrementAsync());
            await taskAwaitable;

            Assert.That(taskAwaitable, Is.TypeOf<ConfiguredTaskAwaitable>());
        }

        return ExecuteAsync();
    }

    protected override Task OnRunTestCaseForTaskWithResultAsync(ConfigureAwaitMode mode, ValueContainer<int> container)
    {
        async Task<int> IncrementAsync()
        {
            await ConfigureAwait(mode, CreateNonCompleteTask());
            return container.Value++;
        }

        async Task ExecuteAsync()
        {
            AssertSynchronizationContext();

            var taskAwaitable = ConfigureAwait(mode, IncrementAsync());
            await taskAwaitable;

            Assert.That(taskAwaitable, Is.TypeOf<ConfiguredTaskAwaitable<int>>());
        }

        return ExecuteAsync();
    }
}