using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Omnifactotum.Tests
{
    [TestFixture(TestOf = typeof(TemplatedStringResolver))]
    internal sealed class TemplatedStringResolverTests
    {
        [Test]
        public void TestConstructionWhenArgumentIsNullThenThrows()
            => Assert.That(() => new TemplatedStringResolver(null!), Throws.ArgumentNullException);

        [Test]
        public void TestConstructionWhenInvalidVariableNameThenThrows()
            => Assert.That(
                () => new TemplatedStringResolver(
                    new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        { "ValidKey1", "ValidValue1" },
                        { "Key{0}", "Value1" },
                        { "ValidKey2", "ValidValue2" },
                        { "{K", "Value2" },
                        { "ValidKey3", "ValidValue3" },
                        { "K}", "Value3" }
                    }),
                Throws.ArgumentException
                    .With
                    .Property(nameof(ArgumentException.ParamName))
                    .EqualTo("templateVariables")
                    .And
                    .Message.StartsWith(@"The following variable names are invalid: ""{K"", ""K}"", ""Key{0}""."));

        [Test]
        public void TestConstructionWhenValidArgumentThenSucceeds()
        {
            var testeeWithEmptyTemplateVariables = new TemplatedStringResolver(new Dictionary<string, string>());
            Assert.That(() => testeeWithEmptyTemplateVariables.TemplateVariables, Is.Empty);

            var testeeWithSomeTemplateVariables = new TemplatedStringResolver(
                new Dictionary<string, string>
                {
                    { "K1", "V1" },
                    { "K2", "V2" }
                });

            Assert.That(
                () => testeeWithSomeTemplateVariables.TemplateVariables,
                Is.EqualTo(
                    new Dictionary<string, string>
                    {
                        { "K1", "V1" },
                        { "K2", "V2" }
                    }));
        }

        [Test]
        public void TestResolveWhenArgumentIsNullThenThrows()
        {
            var testee = new TemplatedStringResolver(new Dictionary<string, string>());
            Assert.That(() => testee.Resolve(null!), Throws.ArgumentNullException);
        }

        [Test]
        [TestCase(
            "{Last}, {First}",
            @"Error at index 8: the injected variable ""First"" is not defined.")]
        [TestCase(
            "{Last}, {}",
            @"Error at index 8: the injected variable """" is not defined.")]
        [TestCase(
            "Dear {{Last}",
            @"Error at index 11: unexpected token ""}"".")]
        [TestCase(
            "Hello {Last}}",
            @"Error at index 12: unexpected token ""}"".")]
        [TestCase(
            "Dear {Last",
            @"Error at index 5: unexpected token ""{"".")]
        [TestCase(
            "Dear Last}",
            @"Error at index 9: unexpected token ""}"".")]
        [TestCase(
            "Dear {Last{}",
            @"Error at index 5: unexpected token ""{"".")]
        public void TestResolveWhenDefaultOptionsAndInvalidArgumentThenThrows(string template, string expectedExceptionMessage)
        {
            var testee = new TemplatedStringResolver(new Dictionary<string, string> { { "Last", "Doe" } });

            Assert.That(() => testee.Resolve(template), Throws.TypeOf<TemplatedStringResolverException>().With.Message.EqualTo(expectedExceptionMessage));

            //// ReSharper disable once RedundantArgumentDefaultValue
            Assert.That(
                () => testee.Resolve(template, TemplatedStringResolverOptions.None),
                Throws.TypeOf<TemplatedStringResolverException>().With.Message.EqualTo(expectedExceptionMessage));
        }

        [Test]
        [TestCase("", "")]
        [TestCase(
            "{Last}, {First}",
            "Doe, John")]
        [TestCase(
            "{{{Last}, {First}}}",
            "{Doe, John}")]
        [TestCase(
            "{Last}}}, {{{First}",
            "Doe}, {John")]
        [TestCase(
            "{Last}, {First} [{}]",
            "Doe, John [(empty)]")]
        [TestCase(
            "Dear {First} {Last}, please use {{ and }} for expressions.",
            "Dear John Doe, please use { and } for expressions.")]
        public void TestResolveWhenDefaultOptionsAndValidArgumentThenSucceeds(string template, string expectedResult)
        {
            var testee = new TemplatedStringResolver(
                new Dictionary<string, string>
                {
                    { "First", "John" },
                    { "Last", "Doe" },
                    { "", "(empty)" }
                });

            Assert.That(() => testee.Resolve(template), Is.EqualTo(expectedResult));

            //// ReSharper disable once RedundantArgumentDefaultValue
            Assert.That(() => testee.Resolve(template, TemplatedStringResolverOptions.None), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(
            "{First} {Last} ({Age} years old)",
            "John\x0020\x0020(17 years old)")]
        [TestCase(
            "{First} ({} years old)",
            "John (\x0020years old)")]
        public void TestResolveWhenTolerateUndefinedVariablesOptionsAndValidArgumentThenSucceeds(string template, string expectedResult)
        {
            var testee = new TemplatedStringResolver(
                new Dictionary<string, string>
                {
                    { "First", "John" },
                    { "Age", "17" }
                });

            Assert.That(() => testee.Resolve(template, TemplatedStringResolverOptions.TolerateUndefinedVariables), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(
            "{First} {Last}, {Age years old",
            "John Doe, {Age years old")]
        [TestCase(
            "{First} {Last}, Age} years old",
            "John Doe, Age} years old")]
        public void TestResolveWhenTolerateUnexpectedTokensOptionsAndValidArgumentThenSucceeds(string template, string expectedResult)
        {
            var testee = new TemplatedStringResolver(
                new Dictionary<string, string>
                {
                    { "First", "John" },
                    { "Last", "Doe" },
                    { "Age", "17" }
                });

            Assert.That(() => testee.Resolve(template, TemplatedStringResolverOptions.TolerateUnexpectedTokens), Is.EqualTo(expectedResult));
        }
    }
}