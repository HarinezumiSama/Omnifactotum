using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods
{
    [TestFixture(TestOf = typeof(OmnifactotumSecureStringExtensions))]
    internal sealed class OmnifactotumSecureStringExtensionsTests
    {
        [Test]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("\u0000", false)]
        [TestCase("\u0020", false)]
        [TestCase("\t", false)]
        [TestCase("\r", false)]
        [TestCase("\n", false)]
        [TestCase(@"luNCh/обеД/ランチ/dÉJEuner/午餐", false)]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void TestIsNullOrEmpty(string? plainText, bool expectedResult)
        {
            using var secureString = CreateSecureString(plainText);
            if (plainText is null)
            {
                Assert.That(secureString, Is.Null);
            }

            Assert.That(() => secureString.IsNullOrEmpty(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("\u0020")]
        [TestCase("\t")]
        [TestCase("\r")]
        [TestCase("\n")]
        [TestCase(@"diNNEr/уЖин/晩ごはん/dÎner/晚餐")]
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public void TestToPlainText(string? plainText)
        {
            using var secureString = CreateSecureString(plainText);
            if (plainText is null)
            {
                Assert.That(secureString, Is.Null);
            }

            var actualResult = secureString.ToPlainText();
            Assert.That(actualResult, Is.EqualTo(plainText));
            if (plainText is null)
            {
                Assert.That(actualResult, Is.Null);
            }
        }

        private static SecureString? CreateSecureString(string? plainText)
        {
            switch (plainText)
            {
                case null:
                    return null;

                default:
                    var secureString = new SecureString();
                    foreach (var ch in plainText)
                    {
                        secureString.AppendChar(ch);
                    }

                    return secureString;
            }
        }
    }
}