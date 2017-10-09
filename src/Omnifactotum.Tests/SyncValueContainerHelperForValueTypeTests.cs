using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class SyncValueContainerHelperForValueTypeTests : SyncValueContainerHelperTestsBase<int>
    {
        public SyncValueContainerHelperForValueTypeTests()
            : base(1, int.MaxValue)
        {
            // Nothing to do
        }
    }
}