using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(ValueContainer<>))]
    internal sealed class ValueContainerForValueTypeTests : ValueContainerTestsBase<int>
    {
        public ValueContainerForValueTypeTests()
            : base(1, int.MaxValue)
        {
            // Nothing to do
        }
    }
}