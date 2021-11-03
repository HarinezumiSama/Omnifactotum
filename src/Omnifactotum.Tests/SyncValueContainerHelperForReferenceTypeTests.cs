using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(SyncValueContainer))]
    internal sealed class SyncValueContainerHelperForReferenceTypeTests : SyncValueContainerHelperTestsBase<string>
    {
        public SyncValueContainerHelperForReferenceTypeTests()
            : base("Some value", "Another value")
        {
            // Nothing to do
        }
    }
}