using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(SyncValueContainer))]
    internal sealed class SyncValueContainerHelperForValueTypeTests : SyncValueContainerHelperTestsBase<int>
    {
        public SyncValueContainerHelperForValueTypeTests()
            : base(1, int.MaxValue)
        {
            // Nothing to do
        }

        protected override int ValueThreadSafetyInitialValue => int.MinValue;

        public override void TestConstructionWithValueIncludingDefaultValue() => Assert.Ignore();

        public override void TestEqualityIncludingDefaultValue() => Assert.Ignore();

        public override void TestToStringIncludingDefaultValue() => Assert.Ignore();
    }
}