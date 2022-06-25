#nullable enable

using System;
using System.Collections.Generic;
using NUnit.Framework;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(KeyValuePair))]
    internal sealed class KeyValuePairTests : KeyValuePairTestsBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var type = typeof(KeyValuePair);
            if (type.Assembly != typeof(Factotum).Assembly)
            {
                Assert.Ignore(AsInvariant($@"Skipping the test for the built-in class {type.AssemblyQualifiedName.ToUIString()}."));
            }
        }

        protected override KeyValuePair<TKey, TValue> CreateTestee<TKey, TValue>(TKey key, TValue value) => KeyValuePair.Create(key, value);
    }
}