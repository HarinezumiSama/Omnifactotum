using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumValueTaskExtensions))]
[NonParallelizable]
internal sealed class OmnifactotumValueTaskExtensionsTests : GenericTaskExtensionsTestsBase
{
#if NET5_0_OR_GREATER
    [Test]
    public void TestEnsureNotNullAsyncForReferenceType()
    {
        const string ExpectedValue = "99ee00e562ea47669d01f3580262d811";

        Assert.That(
            async () => await ValueTask.FromResult<string?>(null).EnsureNotNullAsync(),
            Throws.ArgumentNullException.With.Message.EqualTo(
                "The following expression is null: { await ValueTask.FromResult<string?>(null) }. (Parameter 'value')"));

        Assert.That(
            async () => await ValueTask.FromResult<string?>(ExpectedValue).EnsureNotNullAsync(),
            Is.EqualTo(ExpectedValue));
    }

    [Test]
    public void TestEnsureNotNullAsyncForValueType()
    {
        const int ExpectedValue = -17;

        Assert.That(
            async () => await ValueTask.FromResult<int?>(null).EnsureNotNullAsync(),
            Throws.ArgumentNullException.With.Message.EqualTo(
                "The following expression is null: { await ValueTask.FromResult<int?>(null) }. (Parameter 'value')"));

        Assert.That(
            async () => await ValueTask.FromResult<int?>(ExpectedValue).EnsureNotNullAsync(),
            Is.EqualTo(ExpectedValue));
    }
#endif

    protected override Task OnRunTestCaseForVoidTaskAsync(ConfigureAwaitMode mode, ValueContainer<int> container)
    {
        async ValueTask IncrementAsync()
        {
            await ConfigureAwait(mode, CreateNonCompleteTask());
            container.Value++;
        }

        async Task ExecuteAsync()
        {
            AssertSynchronizationContext();

            var taskAwaitable = ConfigureAwait(mode, IncrementAsync());
            await taskAwaitable;

            Assert.That(taskAwaitable, Is.TypeOf<ConfiguredValueTaskAwaitable>());
        }

        return ExecuteAsync();
    }

    protected override Task OnRunTestCaseForTaskWithResultAsync(ConfigureAwaitMode mode, ValueContainer<int> container)
    {
        async ValueTask<int> IncrementAsync()
        {
            await ConfigureAwait(mode, CreateNonCompleteTask());
            return container.Value++;
        }

        async Task ExecuteAsync()
        {
            AssertSynchronizationContext();

            var taskAwaitable = ConfigureAwait(mode, IncrementAsync());
            await taskAwaitable;

            Assert.That(taskAwaitable, Is.TypeOf<ConfiguredValueTaskAwaitable<int>>());
        }

        return ExecuteAsync();
    }
}