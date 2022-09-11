using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumValueTaskExtensions))]
    internal sealed class OmnifactotumValueTaskExtensionsTests : GenericTaskExtensionsTestsBase
    {
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
}