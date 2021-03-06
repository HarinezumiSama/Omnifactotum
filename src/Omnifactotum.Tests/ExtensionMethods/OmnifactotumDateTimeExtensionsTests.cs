﻿using System;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumDateTimeExtensions))]
    internal sealed class OmnifactotumDateTimeExtensionsTests
    {
        [Test]
        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Unspecified)]
        [TestCase(DateTimeKind.Utc)]
        public void TestToFixedString(DateTimeKind kind)
        {
            var value = new DateTime(2001, 2, 3, 7, 8, 9, 123);

            var actualResult = value.ToFixedString();
            Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09"));
        }

        [Test]
        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Unspecified)]
        [TestCase(DateTimeKind.Utc)]
        public void TestToPreciseFixedString(DateTimeKind kind)
        {
            var value = new DateTime(2001, 2, 3, 7, 8, 9, 123);

            var actualResult = value.ToPreciseFixedString();
            Assert.That(actualResult, Is.EqualTo("2001-02-03 07:08:09.1230000"));
        }
    }
}