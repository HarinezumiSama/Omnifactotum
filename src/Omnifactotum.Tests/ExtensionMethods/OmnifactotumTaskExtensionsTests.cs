#nullable enable

using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumTaskExtensions))]
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