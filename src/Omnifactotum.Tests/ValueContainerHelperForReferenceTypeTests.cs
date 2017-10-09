using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class ValueContainerHelperForReferenceTypeTests : ValueContainerHelperTestsBase<string>
    {
        public ValueContainerHelperForReferenceTypeTests()
            : base("Some value", "Another value")
        {
            // Nothing to do
        }
    }
}