using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class SyncValueContainerForReferenceTypeTests : SyncValueContainerTestsBase<string>
    {
        public SyncValueContainerForReferenceTypeTests()
            : base("Some value", "Another value")
        {
            // Nothing to do
        }
    }
}