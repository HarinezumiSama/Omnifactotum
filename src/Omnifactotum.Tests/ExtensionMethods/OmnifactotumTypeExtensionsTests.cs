using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumTypeExtensions))]
    internal sealed class OmnifactotumTypeExtensionsTests
    {
        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestGetManifestResourceStreamWhenNullTypeIsPassedThenThrows()
            => Assert.That(() => ((Type)null).GetManifestResourceStream("ValidName"), Throws.ArgumentNullException);

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void TestGetManifestResourceStreamWhenInvalidNameIsPassedThenThrows(string name)
            => Assert.That(() => GetType().GetManifestResourceStream(name), Throws.ArgumentException);

        [Test]
        public void TestGetManifestResourceStreamNonExistentResourceNameIsPassedThenReturnsNull()
        {
            using (var stream = GetType().GetManifestResourceStream("NonExistentResourceName"))
            {
                Assert.That(stream, Is.Null);
            }
        }

        [Test]
        public void TestGetManifestResourceStreamWhenValidResourceNameIsPassedThenReturnsNonNullStream()
        {
            using (var stream =
                GetType().GetManifestResourceStream("OmnifactotumTypeExtensionsTests.TestResource.txt"))
            {
                Assert.That(stream, Is.Not.Null);

                using (var reader = new StreamReader(stream))
                {
                    var actualValue = reader.ReadToEnd();
                    Assert.That(actualValue, Is.EqualTo("Test"));
                }
            }
        }

        [Test]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void TestIsNullableWhenNullArgumentIsPassedThenThrows()
            => Assert.That(() => ((Type)null).IsNullable(), Throws.ArgumentNullException);

        [Test]
        [TestCase(typeof(bool), false)]
        [TestCase(typeof(bool?), true)]
        [TestCase(typeof(string), false)]
        [TestCase(typeof(object), false)]
        [TestCase(typeof(Action), false)]
        [TestCase(typeof(IDisposable), false)]
        public void TestIsNullableWhenValidArgumentIsPassedThenSucceeds(Type type, bool expectedResult)
            => Assert.That(type.IsNullable(), Is.EqualTo(expectedResult));
    }
}