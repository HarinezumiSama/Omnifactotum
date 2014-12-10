﻿using System;
using System.Linq;
using NUnit.Framework;

namespace Omnifactotum.Tests.Extensions
{
    //// ReSharper disable ExpressionIsAlwaysNull - Intentionally for unit tests

    [TestFixture]
    public sealed class OmnifactotumGenericObjectExtensionsTests
    {
        #region Tests

        [Test]
        public void TestEnsureNotNullForReferenceType()
        {
            var emptyString = string.Empty;
            Assert.That(() => emptyString.EnsureNotNull(), Is.SameAs(emptyString));

            var someObject = new object();
            Assert.That(() => someObject.EnsureNotNull(), Is.SameAs(someObject));

            object nullObject = null;
            Assert.That(() => nullObject.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestEnsureNotNullForNullable()
        {
            int? someValue = 42;
            Assert.That(() => someValue.EnsureNotNull(), Is.EqualTo(someValue.Value));

            int? nullValue = null;
            Assert.That(() => nullValue.EnsureNotNull(), Throws.TypeOf<ArgumentNullException>());
        }

        #endregion
    }
}