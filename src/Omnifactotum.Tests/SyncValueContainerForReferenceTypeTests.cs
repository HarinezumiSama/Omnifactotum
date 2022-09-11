using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(SyncValueContainer<>))]
    internal sealed class SyncValueContainerForReferenceTypeTests : SyncValueContainerForReferenceTypeTestsBase<string>
    {
        public SyncValueContainerForReferenceTypeTests()
            : base("Some value", "Another value")
        {
            // Nothing to do
        }

        protected override string ValueThreadSafetyInitialValue => "Something else";
    }
}