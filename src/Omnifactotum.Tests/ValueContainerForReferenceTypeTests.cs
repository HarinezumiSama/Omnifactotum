using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ValueContainer<>))]
    internal sealed class ValueContainerForReferenceTypeTests : ValueContainerForReferenceTypeTestsBase<string>
    {
        public ValueContainerForReferenceTypeTests()
            : base("Some value", "Another value")
        {
            // Nothing to do
        }
    }
}