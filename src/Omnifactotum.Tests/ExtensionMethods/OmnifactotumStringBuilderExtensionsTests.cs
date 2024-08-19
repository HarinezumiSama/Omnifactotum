using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using NUnit.Framework;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumStringBuilderExtensions))]
internal sealed class OmnifactotumStringBuilderExtensionsTests
{
    private const int DefaultMinimumSecuredPartLength = OmnifactotumStringBuilderExtensions.DefaultMinimumSecuredPartLength;
    private const int DefaultLoggedPartLength = OmnifactotumStringBuilderExtensions.DefaultLoggedPartLength;

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("e0921ae61cad495a9c3af0e700e8686c")]
    public void TestAppendUIStringWhenInvalidArgumentThenThrows(string? value)
        => Assert.That(
            () => default(StringBuilder)!.AppendUIString(value),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("stringBuilder"));

    [Test]
    [TestCase(null, "null")]
    [TestCase("", @"""""")]
    [TestCase("fc708089-0BCc-4891-a566-695e14393910", @"""fc708089-0BCc-4891-a566-695e14393910""")]
    [TestCase(@" A ""B"" 'C'-`d`/«3»", @""" A """"B"""" 'C'-`d`/«3»""")]
    public void TestAppendUIStringWhenValidArgumentsThenSucceeds(string? value, string expectedResult)
        => ExecuteTestCase(builder => builder.AppendUIString(value), expectedResult);

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("7ba08709d7e8408a9e9b275ad718e651")]
    [SuppressMessage("ReSharper", "ArgumentsStyleLiteral")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public void TestAppendSecuredUIStringWhenInvalidMandatoryArgumentThenThrows(string? value)
    {
        Assert.That(
            () => default(StringBuilder)!.AppendSecuredUIString(value),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("stringBuilder"));

        Assert.That(
            () => default(StringBuilder)!.AppendSecuredUIString(
                value: value,
                loggedPartLength: DefaultLoggedPartLength,
                minimumSecuredPartLength: DefaultMinimumSecuredPartLength),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("stringBuilder"));

        Assert.That(
            () => default(StringBuilder)!.AppendSecuredUIString(
                value: value,
                loggedPartLength: 1,
                minimumSecuredPartLength: 3),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("stringBuilder"));
    }

    [Test]
    [TestCase(null, "null")]
    [TestCase("", "{ Length = 0 }")]
    [TestCase("b054ab0a11eb9889c8aB693", "{ Length = 23 }")]
    [TestCase("4C15538a053f4b89a1961A5e", @"""4C15...1A5e""")]
    [TestCase("a9251868d527450384d88c3a39de1958", @"""a925...1958""")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public void TestAppendSecuredUIStringWhenDefaultOptionalParametersThenSucceeds(string? value, string expectedResult)
    {
        ExecuteTestCase(builder => builder.AppendSecuredUIString(value), expectedResult);

        ExecuteTestCase(
            builder => builder.AppendSecuredUIString(
                value: value,
                loggedPartLength: DefaultLoggedPartLength,
                minimumSecuredPartLength: DefaultMinimumSecuredPartLength),
            expectedResult);
    }

    [Test]
    [TestCase(null, 1, 1, "null")]
    [TestCase("", 1, 1, "{ Length = 0 }")]
    [TestCase("bQ3", 1, 1, @"""b...3""")]
    [TestCase("bQ3", 2, 1, "{ Length = 3 }")]
    [TestCase("bQ3", 1, 2, "{ Length = 3 }")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 1, 1, @"""4...e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 2, 1, @"""4C...5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 3, 1, @"""4C""""...A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 4, 1, @"""4C""""5...1A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 11, 1, @"""4C""""5538a053...b""""9a1961A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 11, 2, @"""4C""""5538a053...b""""9a1961A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 12, 1, "{ Length = 24 }")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 3, 18, @"""4C""""...A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 3, 19, "{ Length = 24 }")]
    [TestCase("a9251868d527450384d88c3a39de1958", 1, 1, @"""a...8""")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public void TestAppendSecuredUIStringWhenValidSpecifiedOptionalParametersThenSucceeds(
        string? value,
        int loggedPartLength,
        int minimumSecuredPartLength,
        string expectedResult)
        => ExecuteTestCase(
            builder => builder.AppendSecuredUIString(
                value: value,
                loggedPartLength: loggedPartLength,
                minimumSecuredPartLength: minimumSecuredPartLength),
            expectedResult);

    [Test]
    [TestCase(null, 0, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase(null, -1, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase(null, int.MinValue, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("", 0, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("", -1, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("", int.MinValue, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("Value1", 0, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("Value1", -1, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("Value1", int.MinValue, DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase(null, DefaultLoggedPartLength, 0, "minimumSecuredPartLength")]
    [TestCase(null, DefaultLoggedPartLength, -1, "minimumSecuredPartLength")]
    [TestCase(null, DefaultLoggedPartLength, int.MinValue, "minimumSecuredPartLength")]
    [TestCase("", DefaultLoggedPartLength, 0, "minimumSecuredPartLength")]
    [TestCase("", DefaultLoggedPartLength, -1, "minimumSecuredPartLength")]
    [TestCase("", DefaultLoggedPartLength, int.MinValue, "minimumSecuredPartLength")]
    [TestCase("Value2", DefaultLoggedPartLength, 0, "minimumSecuredPartLength")]
    [TestCase("Value2", DefaultLoggedPartLength, -1, "minimumSecuredPartLength")]
    [TestCase("Value2", DefaultLoggedPartLength, int.MinValue, "minimumSecuredPartLength")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public void TestAppendSecuredUIStringWhenInvalidSpecifiedOptionalParametersThenThrows(
        string? value,
        int loggedPartLength,
        int minimumSecuredPartLength,
        string erroneousParameterName)
    {
        Assert.That(
            () => new StringBuilder()
                .AppendSecuredUIString(
                    value: value,
                    loggedPartLength: loggedPartLength,
                    minimumSecuredPartLength: minimumSecuredPartLength),
            Throws.TypeOf<ArgumentOutOfRangeException>().With.Property(nameof(ArgumentException.ParamName)).EqualTo(erroneousParameterName));

        Assert.That(
            () => new StringBuilder("3080dbb5a66b45c8a6de278073ed043e")
                .AppendSecuredUIString(
                    value: value,
                    loggedPartLength: loggedPartLength,
                    minimumSecuredPartLength: minimumSecuredPartLength),
            Throws.TypeOf<ArgumentOutOfRangeException>().With.Property(nameof(ArgumentException.ParamName)).EqualTo(erroneousParameterName));
    }

    private static void ExecuteTestCase(Action<StringBuilder> action, string expectedResult)
    {
        const string ExistingData = "672d6d2b6dd44b7280fddcc64ece92cd";

        Assert.That(
            () =>
            {
                var stringBuilder = new StringBuilder();
                action(stringBuilder);
                return stringBuilder.ToString();
            },
            Is.EqualTo(expectedResult));

        Assert.That(
            () =>
            {
                var stringBuilder = new StringBuilder(ExistingData);
                action(stringBuilder);
                return stringBuilder.ToString();
            },
            Is.EqualTo(ExistingData + expectedResult));
    }
}