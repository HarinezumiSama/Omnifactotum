using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ValueContainer))]
    internal sealed class ValueContainerHelperForReferenceTypeTests : ValueContainerHelperTestsBase<string>
    {
        public ValueContainerHelperForReferenceTypeTests()
            : base("Some value", "Another value")
        {
            // Nothing to do
        }

        public override void TestConstructionWithValueIncludingDefaultValue() => Assert.Ignore();

        public override void TestEqualityIncludingDefaultValue() => Assert.Ignore();

        public override void TestToStringIncludingDefaultValue() => Assert.Ignore();
    }
}