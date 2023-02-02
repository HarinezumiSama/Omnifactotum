using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(TemplatedStringResolver))]
internal sealed class TemplatedStringResolverTests
{
    [Test]
    public void TestDefaultTemplateVariableNameComparer()
        => Assert.That(() => TemplatedStringResolver.DefaultTemplateVariableNameComparer, Is.SameAs(StringComparer.Ordinal));

    [Test]
    public void TestGetVariableNamesWhenInvalidArgumentThenThrows()
    {
        Assert.That(() => TemplatedStringResolver.GetVariableNames(null!), Throws.ArgumentNullException);
        Assert.That(() => TemplatedStringResolver.GetVariableNames(null!, StringComparer.InvariantCulture), Throws.ArgumentNullException);
        Assert.That(() => TemplatedStringResolver.GetVariableNames(null!, StringComparer.InvariantCulture, true), Throws.ArgumentNullException);
    }

    [Test]
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
    public void TestGetVariableNamesWhenDefaultNameComparerAndNotTolerateUnexpectedTokensAndInvalidArgumentsThenThrows(
        string templatedString,
        string expectedExceptionMessage)
    {
        IResolveConstraint CreateConstraint() => Throws.TypeOf<TemplatedStringResolverException>().With.Message.EqualTo(expectedExceptionMessage);

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString),
            CreateConstraint());

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, TemplatedStringResolver.DefaultTemplateVariableNameComparer),
            CreateConstraint());

        Assert.That(
            //// ReSharper disable once RedundantArgumentDefaultValue
            () => TemplatedStringResolver.GetVariableNames(templatedString, TemplatedStringResolver.DefaultTemplateVariableNameComparer, false),
            CreateConstraint());

        Assert.That(
            //// ReSharper disable once RedundantArgumentDefaultValue
            () => TemplatedStringResolver.GetVariableNames(templatedString, StringComparer.Ordinal, false),
            CreateConstraint());
    }

    [Test]
    [TestCase("Hello World", new string[0])]
    [TestCase("Find keyword '{keyWord}'", new[] { "keyWord" })]
    [TestCase("Hello {First} {Last} and bye {Last}, {First}", new[] { "First", "Last" })]
    [TestCase("Find keywords '{keyWord}', '{KeyWord}', '{}', '{Keyword}', '{KEYWORD}'", new[] { "", "keyWord", "KeyWord", "Keyword", "KEYWORD" })]
    public void TestGetVariableNamesWhenDefaultNameComparerAndValidArgumentsThenSucceeds(string templatedString, string[] expectedResult)
    {
        IEqualityComparer<string> expectedResultComparer = StringComparer.Ordinal;

        Assert.That(() => TemplatedStringResolver.GetVariableNames(templatedString), Is.EquivalentTo(expectedResult).Using(expectedResultComparer));

        //// ReSharper disable RedundantArgumentDefaultValue
        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, null, false),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));
        //// ReSharper restore RedundantArgumentDefaultValue

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, TemplatedStringResolver.DefaultTemplateVariableNameComparer),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));

        Assert.That(
            //// ReSharper disable once RedundantArgumentDefaultValue
            () => TemplatedStringResolver.GetVariableNames(templatedString, TemplatedStringResolver.DefaultTemplateVariableNameComparer, false),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));

        Assert.That(
            //// ReSharper disable once RedundantArgumentDefaultValue
            () => TemplatedStringResolver.GetVariableNames(templatedString, StringComparer.Ordinal, false),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));
    }

    [Test]
    [TestCase("Hello World {", new string[0])]
    [TestCase("Find {keyword '{keyWord}'", new[] { "keyWord" })]
    [TestCase("Hello {First} {Last} and bye} {Last}, {First}", new[] { "First", "Last" })]
    public void TestGetVariableNamesWhenDefaultNameComparerAndTolerateUnexpectedTokensAndValidArgumentsThenSucceeds(
        string templatedString,
        string[] expectedResult)
    {
        IEqualityComparer<string> expectedResultComparer = StringComparer.Ordinal;

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, tolerateUnexpectedTokens: true),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, null, true),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, TemplatedStringResolver.DefaultTemplateVariableNameComparer, true),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, StringComparer.Ordinal, true),
            Is.EquivalentTo(expectedResult).Using(expectedResultComparer));
    }

    [Test]
    [TestCase("Hello World", new string[0])]
    [TestCase("Find keyword '{keyWord}'", new[] { "keyword" })]
    [TestCase("Hello {First} {Last} and bye {Last}, {First}", new[] { "first", "last" })]
    [TestCase("Find keywords '{keyWord}', '{KeyWord}', '{}', '{Keyword}', '{KEYWORD}'", new[] { "", "keyword" })]
    public void TestGetVariableNamesWhenOrdinalIgnoreCaseNameComparerAndValidArgumentsThenSucceeds(string templatedString, string[] expectedResult)
    {
        IEqualityComparer<string> variableNameComparer = StringComparer.OrdinalIgnoreCase;

        Assert.That(
            () => TemplatedStringResolver.GetVariableNames(templatedString, variableNameComparer),
            Is.EquivalentTo(expectedResult).Using(variableNameComparer));
    }

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
    public void TestConstructionWhenValidArgumentsThenSucceeds()
    {
        var testeeWithEmptyTemplateVariables = new TemplatedStringResolver(new Dictionary<string, string>());
        Assert.That(() => testeeWithEmptyTemplateVariables.TemplateVariables, Is.Empty);

        Assert.That(
            () => testeeWithEmptyTemplateVariables.TemplateVariableNameComparer,
            Is.SameAs(TemplatedStringResolver.DefaultTemplateVariableNameComparer));

        var testeeWithSomeTemplateVariables = new TemplatedStringResolver(
            new Dictionary<string, string>
            {
                { "K1", "V1" },
                { "K2", "V2" }
            },
            StringComparer.InvariantCultureIgnoreCase);

        Assert.That(
            () => testeeWithSomeTemplateVariables.TemplateVariables,
            Is.EqualTo(
                new Dictionary<string, string>
                {
                    { "K1", "V1" },
                    { "K2", "V2" }
                }));

        Assert.That(
            () => testeeWithSomeTemplateVariables.TemplateVariableNameComparer,
            Is.SameAs(StringComparer.InvariantCultureIgnoreCase));
    }

    [Test]
    public void TestResolveWhenTemplatedStringArgumentIsNullThenThrows()
    {
        var testee1 = new TemplatedStringResolver(new Dictionary<string, string>());
        Assert.That(() => testee1.Resolve(null!), Throws.ArgumentNullException);

        var testee2 = new TemplatedStringResolver(new Dictionary<string, string>(), StringComparer.InvariantCulture);
        Assert.That(() => testee2.Resolve(null!), Throws.ArgumentNullException);
    }

    [Test]
    [TestCase(
        "{Last}, {First}",
        @"Error at index 8: the injected variable ""First"" is not defined.")]
    [TestCase(
        "{Last}/{LAST}",
        @"Error at index 7: the injected variable ""LAST"" is not defined.")]
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
    public void TestResolveWhenDefaultNameComparerAndDefaultOptionsAndInvalidArgumentThenThrows(string templatedString, string expectedExceptionMessage)
    {
        var testee = new TemplatedStringResolver(new Dictionary<string, string> { { "Last", "Doe" } });

        Assert.That(() => testee.Resolve(templatedString), Throws.TypeOf<TemplatedStringResolverException>().With.Message.EqualTo(expectedExceptionMessage));

        Assert.That(
            //// ReSharper disable once RedundantArgumentDefaultValue
            () => testee.Resolve(templatedString, TemplatedStringResolverOptions.None),
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
    public void TestResolveWhenDefaultNameComparerAndDefaultOptionsAndValidArgumentThenSucceeds(string templatedString, string expectedResult)
    {
        var testee = new TemplatedStringResolver(
            new Dictionary<string, string>
            {
                { "First", "John" },
                { "Last", "Doe" },
                { "", "(empty)" }
            });

        Assert.That(() => testee.Resolve(templatedString), Is.EqualTo(expectedResult));

        //// ReSharper disable once RedundantArgumentDefaultValue
        Assert.That(() => testee.Resolve(templatedString, TemplatedStringResolverOptions.None), Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase("", "")]
    [TestCase(
        "{Last}, {First}",
        "Doe, John")]
    [TestCase(
        "{LAST}, {FIRST}",
        "Doe, John")]
    [TestCase(
        "{FiRSt}/{LAsT}",
        "John/Doe")]
    [TestCase(
        "{{{lAST}, {fIrST}}}",
        "{Doe, John}")]
    [TestCase(
        "{LaSt}}}, {{{FirSt}",
        "Doe}, {John")]
    [TestCase(
        "{LasT}, {FirsT} [{}]",
        "Doe, John [(empty)]")]
    [TestCase(
        "Dear {FiRSt} {LaST}, please use {{ and }} for expressions.",
        "Dear John Doe, please use { and } for expressions.")]
    public void TestResolveWhenOrdinalIgnoreCaseComparerAndDefaultOptionsAndValidArgumentThenSucceeds(string templatedString, string expectedResult)
    {
        var testee = new TemplatedStringResolver(
            new Dictionary<string, string>
            {
                { "First", "John" },
                { "Last", "Doe" },
                { "", "(empty)" }
            },
            StringComparer.OrdinalIgnoreCase);

        Assert.That(() => testee.Resolve(templatedString), Is.EqualTo(expectedResult));

        //// ReSharper disable once RedundantArgumentDefaultValue
        Assert.That(() => testee.Resolve(templatedString, TemplatedStringResolverOptions.None), Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase(
        "{First} {Last} ({Age} years old)",
        "John\x0020\x0020(17 years old)")]
    [TestCase(
        "{First} ({} years old)",
        "John (\x0020years old)")]
    public void TestResolveWhenDefaultNameComparerAndTolerateUndefinedVariablesOptionsAndValidArgumentThenSucceeds(
        string templatedString,
        string expectedResult)
    {
        var testee = new TemplatedStringResolver(
            new Dictionary<string, string>
            {
                { "First", "John" },
                { "Age", "17" }
            });

        Assert.That(() => testee.Resolve(templatedString, TemplatedStringResolverOptions.TolerateUndefinedVariables), Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase(
        "{First} {Last}, {Age years old",
        "John Doe, {Age years old")]
    [TestCase(
        "{First} {Last}, Age} years old",
        "John Doe, Age} years old")]
    public void TestResolveWhenDefaultNameComparerAndTolerateUnexpectedTokensOptionsAndValidArgumentThenSucceeds(string templatedString, string expectedResult)
    {
        var testee = new TemplatedStringResolver(
            new Dictionary<string, string>
            {
                { "First", "John" },
                { "Last", "Doe" },
                { "Age", "17" }
            });

        Assert.That(() => testee.Resolve(templatedString, TemplatedStringResolverOptions.TolerateUnexpectedTokens), Is.EqualTo(expectedResult));
    }
}