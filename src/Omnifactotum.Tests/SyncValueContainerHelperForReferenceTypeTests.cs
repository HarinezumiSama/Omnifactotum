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

        protected override string ValueThreadSafetyInitialValue => "Something else";

        public override void TestConstructionWithValueIncludingDefaultValue() => Assert.Ignore();

        public override void TestEqualityIncludingDefaultValue() => Assert.Ignore();

        public override void TestToStringIncludingDefaultValue() => Assert.Ignore();
    }
}