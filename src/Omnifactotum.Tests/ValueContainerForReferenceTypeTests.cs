using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ValueContainer<>))]
    internal sealed class ValueContainerForReferenceTypeTests : ValueContainerTestsBase<string>
    {
        public ValueContainerForReferenceTypeTests()
            : base("Some value", "Another value")
        {
            // Nothing to do
        }
    }
}