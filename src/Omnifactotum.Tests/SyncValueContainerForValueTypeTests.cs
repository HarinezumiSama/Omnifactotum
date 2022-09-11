using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(SyncValueContainer<>))]
    internal sealed class SyncValueContainerForValueTypeTests : SyncValueContainerForValueTypeTestsBase<int>
    {
        public SyncValueContainerForValueTypeTests()
            : base(1, int.MaxValue)
        {
            // Nothing to do
        }

        protected override int ValueThreadSafetyInitialValue => int.MinValue;
    }
}