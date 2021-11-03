#if !NET40
#nullable enable

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumTaskExtensions))]
    internal sealed class OmnifactotumTaskExtensionsTests : GenericTaskExtensionsTestsBase
    {
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
}
#endif