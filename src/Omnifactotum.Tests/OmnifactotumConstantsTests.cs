#nullable enable

using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture]
    internal sealed class OmnifactotumConstantsTests
    {
        [Test]
        public void TestComparisonResult()
        {
            Assert.That(() => OmnifactotumConstants.ComparisonResult.Equal, Is.Zero);
            Assert.That(() => OmnifactotumConstants.ComparisonResult.LessThan, Is.LessThan(OmnifactotumConstants.ComparisonResult.Equal));
            Assert.That(() => OmnifactotumConstants.ComparisonResult.GreaterThan, Is.GreaterThan(OmnifactotumConstants.ComparisonResult.Equal));
        }
    }
}