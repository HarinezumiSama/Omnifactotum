using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(SyncValueContainer<>))]
    internal sealed class SyncValueContainerForValueTypeTests : SyncValueContainerTestsBase<int>
    {
        public SyncValueContainerForValueTypeTests()
            : base(1, int.MaxValue)
        {
            // Nothing to do
        }
    }
}