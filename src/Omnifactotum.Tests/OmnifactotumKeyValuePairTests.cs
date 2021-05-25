using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(OmnifactotumKeyValuePair))]
    internal sealed class OmnifactotumKeyValuePairTests : KeyValuePairTestsBase
    {
        protected override KeyValuePair<TKey, TValue> CreateTestee<TKey, TValue>(TKey key, TValue value)
            => OmnifactotumKeyValuePair.Create(key, value);
    }
}