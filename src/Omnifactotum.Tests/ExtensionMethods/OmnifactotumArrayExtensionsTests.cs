using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.ExtensionMethods
{
    //// ReSharper disable AssignNullToNotNullAttribute - Intentionally for tests

    [TestFixture]
    internal sealed class OmnifactotumArrayExtensionsTests
    {
        private static readonly string[] NullArray = null;
        private static readonly byte[] NullByteArray = null;

        [Test]
        public void TestCopyNull()
        {
            var copy = NullArray.Copy();
            Assert.That(copy, Is.Null);
        }

        [Test]
        public void TestCopyNonNull()
        {
            var array = new[] { new CopyableObject { Value = 1 }, new CopyableObject { Value = 2 } };
            var copy = array.Copy().AssertNotNull();

            Assert.That(copy, Is.Not.Null);
            Assert.That(copy, Is.Not.SameAs(array));
            Assert.That(copy.Length, Is.EqualTo(array.Length));
            for (var index = 0; index < array.Length; index++)
            {
                Assert.That(copy[index], Is.SameAs(array[index]));
            }
        }

        [Test]
        public void TestInitializeWithOldValueNegative()
        {
            Assert.That(() => NullArray.Initialize((s, i) => s), Throws.TypeOf<ArgumentNullException>());

            var nonNullArray = new[] { "foo", "bar" }.AssertNotNull();

            Assert.That(
                () => nonNullArray.Initialize((Func<string, int, string>)null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestInitializeWithOldValue()
        {
            var array = new[] { "foo", "bar" }.AssertNotNull();
            array.Initialize((s, i) => s + "-" + (i + 1).ToString(CultureInfo.InvariantCulture));
            Assert.That(array[0], Is.EqualTo("foo-1"));
            Assert.That(array[1], Is.EqualTo("bar-2"));
        }

        [Test]
        public void TestInitializeNegative()
        {
            Assert.That(
                () => NullArray.Initialize(i => i.ToString(CultureInfo.InvariantCulture)),
                Throws.TypeOf<ArgumentNullException>());

            var nonNullArray = new[] { "foo", "bar" }.AssertNotNull();

            Assert.That(
                () => nonNullArray.Initialize((Func<int, string>)null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestInitialize()
        {
            var array = new[] { "foo", "bar" }.AssertNotNull();
            array.Initialize(i => (i + 1).ToString(CultureInfo.InvariantCulture));
            Assert.That(array[0], Is.EqualTo("1"));
            Assert.That(array[1], Is.EqualTo("2"));
        }

        [Test]
        public void TestAvoidNull()
        {
            var avoided = NullArray.AvoidNull();
            Assert.That(avoided, Is.Not.Null);
            Assert.That(avoided.Length, Is.EqualTo(0));

            var array = new[] { "foo", "bar" }.AssertNotNull();
            var shouldBeSame = array.AvoidNull();
            Assert.That(shouldBeSame, Is.SameAs(array));
        }

        [Test]
        public void TestAsReadOnlyNegative()
        {
            Assert.That(() => NullArray.AsReadOnly(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestAsReadOnly()
        {
            var array = new[] { "foo", "bar" }.AssertNotNull();
            var readOnly = array.AsReadOnly();

            Assert.That(readOnly, Is.Not.Null);
            Assert.That(() => ((ICollection<string>)readOnly).Clear(), Throws.TypeOf<NotSupportedException>());
            Assert.That(readOnly, Is.EqualTo(array));

            array[0] = "not foo";
            Assert.That(readOnly, Is.EqualTo(array));
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void TestToHexStringWithCaseNegative(bool useUpperCase)
        {
            Assert.That(() => NullByteArray.ToHexString(useUpperCase), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCase(false, "01c8")]
        [TestCase(true, "01C8")]
        public void TestToHexStringWithCase(bool useUpperCase, string expectedValue)
        {
            Assert.That(expectedValue.Any(char.IsLetter), Is.True);

            var array = new byte[] { 1, 200 };
            var hexString = array.ToHexString(useUpperCase);
            Assert.That(hexString, Is.EqualTo(expectedValue));
        }

        [Test]
        public void TestToHexStringNegative()
        {
            Assert.That(() => NullByteArray.ToHexString(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestToHexString()
        {
            const string ExpectedValue = "01c8";
            Assert.That(ExpectedValue.Any(char.IsLetter), Is.True);

            var array = new byte[] { 1, 200 };
            var hexString = array.ToHexString();
            Assert.That(hexString, Is.EqualTo(ExpectedValue));
        }

        private sealed class CopyableObject : ICloneable
        {
            public int Value
            {
                private get;
                set;
            }

            [UsedImplicitly]
            public CopyableObject Copy()
            {
                return new CopyableObject { Value = Value };
            }

            public object Clone()
            {
                return Copy();
            }
        }
    }
}