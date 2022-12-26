using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumImmutableArrayExtensions))]
internal sealed class OmnifactotumImmutableArrayExtensionsTests
{
    [Test]
    [SuppressMessage("ReSharper", "ArrangeDefaultValueWhenTypeNotEvident")]
    public void TestAvoidDefaultSucceeds()
    {
        ExecuteTestCase(default(ImmutableArray<int>), Array.Empty<int>());
        ExecuteTestCase(default(ImmutableArray<string>), Array.Empty<string>());

        ExecuteTestCase(ImmutableArray.Create(-13, 17), new[] { -13, 17 });
        ExecuteTestCase(ImmutableArray.Create("Hello", "people"), new[] { "Hello", "people" });

        static void ExecuteTestCase<T>(ImmutableArray<T> input, T[] expectedResult)
        {
            expectedResult.AssertNotNull();
            Assert.That(() => input.AvoidDefault(), Has.Property(nameof(ImmutableArray<object>.IsDefault)).False & Is.EqualTo(expectedResult));
        }
    }
}