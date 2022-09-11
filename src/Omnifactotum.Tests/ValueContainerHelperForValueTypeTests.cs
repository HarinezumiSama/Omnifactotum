using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ValueContainer))]
    internal sealed class ValueContainerHelperForValueTypeTests : ValueContainerHelperTestsBase<int>
    {
        public ValueContainerHelperForValueTypeTests()
            : base(1, int.MaxValue)
        {
            // Nothing to do
        }

        public override void TestConstructionWithValueIncludingDefaultValue() => Assert.Ignore();

        public override void TestEqualityIncludingDefaultValue() => Assert.Ignore();

        public override void TestToStringIncludingDefaultValue() => Assert.Ignore();
    }
}