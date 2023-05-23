using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Omnifactotum.Tests.ExtensionMethods;

[TestFixture(TestOf = typeof(OmnifactotumStringExtensions))]
internal sealed class OmnifactotumStringExtensionsTests
{
    [Test]
    [TestCase(null, true)]
    [TestCase("", true)]
    [TestCase("\u0000", false)]
    [TestCase("\u0020", false)]
    [TestCase("\t", false)]
    [TestCase("\r", false)]
    [TestCase("\n", false)]
    [TestCase("\u0020\t\r\n", false)]
    [TestCase("A", false)]
    [TestCase("\u0020A\u0020", false)]
    public void TestIsNullOrEmpty(string? value, bool expectedResult)
        => Assert.That(value.IsNullOrEmpty, Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, true)]
    [TestCase("", true)]
    [TestCase("\u0000", false)]
    [TestCase("\u0020", true)]
    [TestCase("\t", true)]
    [TestCase("\r", true)]
    [TestCase("\n", true)]
    [TestCase("\u0020\t\r\n", true)]
    [TestCase("A", false)]
    [TestCase("\u0020A\u0020", false)]
    public void TestIsNullOrWhiteSpace(string? value, bool expectedResult)
        => Assert.That(value.IsNullOrWhiteSpace, Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, null)]
    [TestCase("", null)]
    [TestCase(" ", null)]
    [TestCase("\n", null)]
    [TestCase("T", null)]
    [TestCase("1", true)]
    [TestCase("42", true)]
    [TestCase(" 1 \n ", true)]
    [TestCase("true", true)]
    [TestCase("  tRue ", true)]
    [TestCase("0", false)]
    [TestCase(" \n 0 \r ", false)]
    [TestCase("false", false)]
    [TestCase(" FALse ", false)]
    public void TestToNullableBooleanAndToBoolean(string? value, bool? expectedResult)
    {
        Assert.That(value.ToNullableBoolean, Is.EqualTo(expectedResult));

        var constraint = expectedResult.HasValue
            ? (IResolveConstraint)Is.EqualTo(expectedResult.Value)
            : Throws.ArgumentException;

        Assert.That(value!.ToBoolean, constraint);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("\u0020")]
    [TestCase(":")]
    [TestCase(",")]
    public void TestJoinWhenInvalidArgumentsThenThrows(string? separator)
    {
        const IEnumerable<string?> NullCollection = null!;

        Assert.That(
            () => NullCollection!.Join(separator),
            Throws.TypeOf<ArgumentNullException>().With.Property(nameof(ArgumentException.ParamName)).EqualTo("values"));
    }

    [Test]
    [TestCase(new[] { "foo", "bar" }, ",", "foo,bar")]
    [TestCase(new[] { "foo", "bar" }, ",\u0020", "foo, bar")]
    [TestCase(new[] { "foo", "bar" }, "\u0020", "foo bar")]
    [TestCase(new[] { "\u0020foo\u0020", "\u0020bar\u0020" }, "\u0020", " foo   bar ")]
    [TestCase(new[] { "foo;", "bar;" }, ";", "foo;;bar;")]
    [TestCase(new[] { "bar" }, ",\u0020", "bar")]
    [TestCase(new string[0], ",\u0020", "")]
    [TestCase(new[] { "foo", "bar" }, null, "foobar")]
    [TestCase(new[] { "foo", "bar" }, "", "foobar")]
    public void TestJoinWhenValidArgumentsThenSucceeds(string?[] values, string? separator, string expectedResult)
        => Assert.That(() => values.Join(separator), Is.EqualTo(expectedResult));

    [Test]
    public void TestWhereNotEmptyWhenInvalidArgumentsThenThrows()
    {
        // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<string>` instance actually invokes `WhereNotEmpty()`

        Assert.That(
            () => default(IEnumerable<string?>)!.WhereNotEmpty().ToArray(),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("source"));
    }

    [Test]
    public void TestWhereNotEmptyWhenValidArgumentThenSucceeds()
    {
        ExecuteTestCase(Array.Empty<string?>(), Array.Empty<string>());
        ExecuteTestCase(new string?[] { null }, Array.Empty<string>());
        ExecuteTestCase(new string?[] { null, null }, Array.Empty<string>());
        ExecuteTestCase(new[] { null, string.Empty, "\x0020\t\r\n\x0020", null }, new[] { "\x0020\t\r\n\x0020" });
        ExecuteTestCase(new[] { null, "q", null }, new[] { "q" });

        ExecuteTestCase(
            new[] { "Hello\x0020world", "?", null, string.Empty, "\t\x0020\r\n", null, "Bye!" },
            new[] { "Hello\x0020world", "?", "\t\x0020\r\n", "Bye!" });

        //// ReSharper disable once SuggestBaseTypeForParameter
        static void ExecuteTestCase(string?[] input, string[] expectedResult)
        {
            // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<string>` instance actually invokes `WhereNotEmpty()`
            Assert.That(() => input.AsEnumerable().WhereNotEmpty().ToArray(), Is.EqualTo(expectedResult) & Is.TypeOf<string[]>());
        }
    }

    [Test]
    public void TestWhereNotBlankWhenInvalidArgumentsThenThrows()
    {
        // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<string>` instance actually invokes `WhereNotBlank()`

        Assert.That(
            () => default(IEnumerable<string?>)!.WhereNotBlank().ToArray(),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("source"));
    }

    [Test]
    public void TestWhereNotBlankWhenValidArgumentThenSucceeds()
    {
        ExecuteTestCase(Array.Empty<string?>(), Array.Empty<string>());
        ExecuteTestCase(new string?[] { null }, Array.Empty<string>());
        ExecuteTestCase(new string?[] { null, null }, Array.Empty<string>());
        ExecuteTestCase(new[] { null, string.Empty, "\x0020\t\r\n\x0020", null }, Array.Empty<string>());
        ExecuteTestCase(new[] { null, "q", null }, new[] { "q" });

        ExecuteTestCase(
            new[] { "Hello\x0020world", "?", null, string.Empty, "\t\x0020\r\n", null, "Bye!" },
            new[] { "Hello\x0020world", "?", "Bye!" });

        //// ReSharper disable once SuggestBaseTypeForParameter
        static void ExecuteTestCase(string?[] input, string[] expectedResult)
        {
            // Note: `ToArray()` call is required to ensure that the compiler generated `IEnumerable<string>` instance actually invokes `WhereNotBlank()`
            Assert.That(() => input.AsEnumerable().WhereNotBlank().ToArray(), Is.EqualTo(expectedResult) & Is.TypeOf<string[]>());
        }
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("\u0020")]
    [TestCase("foo bar")]
    [TestCase("\u0020foo bar\u0020")]
    public void TestAvoidNull(string? value)
        => Assert.That(value.AvoidNull, value is null ? Is.EqualTo(string.Empty) : Is.SameAs(value));

    [Test]
    [TestCase(null, "null")]
    [TestCase("", @"""""")]
    [TestCase(@" A ""B"" 'C'", @""" A """"B"""" 'C'""")]
    public void TestToUIString(string? value, string expectedResult) => Assert.That(value.ToUIString, Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, "null")]
    [TestCase("", @"{ Length = 0 }")]
    [TestCase("b054ab0a11eb9889c8aB693", @"{ Length = 23 }")]
    [TestCase("4C15538a053f4b89a1961A5e", @"""4C15...1A5e""")]
    [TestCase("a9251868d527450384d88c3a39de1958", @"""a925...1958""")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public void TestToSecuredUIStringWhenDefaultOptionalParametersThenSucceeds(string? value, string expectedResult)
    {
        Assert.That(() => value.ToSecuredUIString(), Is.EqualTo(expectedResult));

        Assert.That(
            () => value.ToSecuredUIString(
                loggedPartLength: OmnifactotumStringExtensions.DefaultLoggedPartLength,
                minimumSecuredPartLength: OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength),
            Is.EqualTo(expectedResult));
    }

    [Test]
    [TestCase(null, 1, 1, "null")]
    [TestCase("", 1, 1, @"{ Length = 0 }")]
    [TestCase("bQ3", 1, 1, @"""b...3""")]
    [TestCase("bQ3", 2, 1, @"{ Length = 3 }")]
    [TestCase("bQ3", 1, 2, @"{ Length = 3 }")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 1, 1, @"""4...e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 2, 1, @"""4C...5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 3, 1, @"""4C""""...A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 4, 1, @"""4C""""5...1A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 11, 1, @"""4C""""5538a053...b""""9a1961A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 11, 2, @"""4C""""5538a053...b""""9a1961A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 12, 1, @"{ Length = 24 }")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 3, 18, @"""4C""""...A5e""")]
    [TestCase(@"4C""5538a053f4b""9a1961A5e", 3, 19, @"{ Length = 24 }")]
    [TestCase("a9251868d527450384d88c3a39de1958", 1, 1, @"""a...8""")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public void TestToSecuredUIStringWhenValidSpecifiedOptionalParametersThenSucceeds(
        string? value,
        int loggedPartLength,
        int minimumSecuredPartLength,
        string expectedResult)
        => Assert.That(
            () => value.ToSecuredUIString(loggedPartLength: loggedPartLength, minimumSecuredPartLength: minimumSecuredPartLength),
            Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, 0, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase(null, -1, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase(null, int.MinValue, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("", 0, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("", -1, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("", int.MinValue, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("Value1", 0, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("Value1", -1, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase("Value1", int.MinValue, OmnifactotumStringExtensions.DefaultMinimumSecuredPartLength, "loggedPartLength")]
    [TestCase(null, OmnifactotumStringExtensions.DefaultLoggedPartLength, 0, "minimumSecuredPartLength")]
    [TestCase(null, OmnifactotumStringExtensions.DefaultLoggedPartLength, -1, "minimumSecuredPartLength")]
    [TestCase(null, OmnifactotumStringExtensions.DefaultLoggedPartLength, int.MinValue, "minimumSecuredPartLength")]
    [TestCase("", OmnifactotumStringExtensions.DefaultLoggedPartLength, 0, "minimumSecuredPartLength")]
    [TestCase("", OmnifactotumStringExtensions.DefaultLoggedPartLength, -1, "minimumSecuredPartLength")]
    [TestCase("", OmnifactotumStringExtensions.DefaultLoggedPartLength, int.MinValue, "minimumSecuredPartLength")]
    [TestCase("Value2", OmnifactotumStringExtensions.DefaultLoggedPartLength, 0, "minimumSecuredPartLength")]
    [TestCase("Value2", OmnifactotumStringExtensions.DefaultLoggedPartLength, -1, "minimumSecuredPartLength")]
    [TestCase("Value2", OmnifactotumStringExtensions.DefaultLoggedPartLength, int.MinValue, "minimumSecuredPartLength")]
    [SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
    public void TestToSecuredUIStringWhenInvalidSpecifiedOptionalParametersThenSucceeds(
        string? value,
        int loggedPartLength,
        int minimumSecuredPartLength,
        string erroneousParameterName)
        => Assert.That(
            () => value.ToSecuredUIString(loggedPartLength: loggedPartLength, minimumSecuredPartLength: minimumSecuredPartLength),
            Throws.TypeOf<ArgumentOutOfRangeException>().With.Property(nameof(ArgumentException.ParamName)).EqualTo(erroneousParameterName));

    [Test]
    [TestCase(null, null, "")]
    [TestCase(null, new[] { '#' }, "")]
    [TestCase("", null, "")]
    [TestCase("", new[] { '#' }, "")]
    [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", null, "A \t\r\nB \t\r\nC")]
    [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", new char[0], "A \t\r\nB \t\r\nC")]
    [TestCase("#A#B#C#", new[] { '#' }, "A#B#C")]
    [TestCase(@"\/\A/\/B\/\C/\/", new[] { '/', '\\' }, @"A/\/B\/\C")]
    public void TestTrimSafely(string? value, char[]? trimChars, string expectedResult)
        => Assert.That(() => value.TrimSafely(trimChars), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, null, "")]
    [TestCase(null, new[] { '#' }, "")]
    [TestCase("", null, "")]
    [TestCase("", new[] { '#' }, "")]
    [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", null, "A \t\r\nB \t\r\nC\u0020\t\r\n")]
    [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", new char[0], "A \t\r\nB \t\r\nC\u0020\t\r\n")]
    [TestCase("#A#B#C#", new[] { '#' }, "A#B#C#")]
    [TestCase(@"\/\A/\/B\/\C/\/", new[] { '/', '\\' }, @"A/\/B\/\C/\/")]
    public void TestTrimStartSafely(string? value, char[]? trimChars, string expectedResult)
        => Assert.That(() => value.TrimStartSafely(trimChars), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, null, "")]
    [TestCase(null, new[] { '#' }, "")]
    [TestCase("", null, "")]
    [TestCase("", new[] { '#' }, "")]
    [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", null, "\u0020\t\r\nA \t\r\nB \t\r\nC")]
    [TestCase("\u0020\t\r\nA\u0020\t\r\nB\u0020\t\r\nC\u0020\t\r\n", new char[0], "\u0020\t\r\nA \t\r\nB \t\r\nC")]
    [TestCase("#A#B#C#", new[] { '#' }, "#A#B#C")]
    [TestCase(@"\/\A/\/B\/\C/\/", new[] { '/', '\\' }, @"\/\A/\/B\/\C")]
    public void TestTrimEndSafely(string value, char[]? trimChars, string expectedResult)
        => Assert.That(() => value.TrimEndSafely(trimChars), Is.EqualTo(expectedResult));

    [Test]
    [TestCase("", "", StringComparison.Ordinal, "")]
    [TestCase("", "Hello", StringComparison.Ordinal, "")]
    [TestCase("Hello", "", StringComparison.Ordinal, "Hello")]
    [TestCase("Hello", "Hell", StringComparison.Ordinal, "o")]
    [TestCase("Hello", "hell", StringComparison.Ordinal, "Hello")]
    [TestCase("Hello", "HeLL", StringComparison.OrdinalIgnoreCase, "o")]
    [TestCase("HelloWorld", "hELLOwORLD", StringComparison.OrdinalIgnoreCase, "")]
    public void TestTrimPrefixWhenValidArgumentsThenSucceeds(string value, string prefix, StringComparison comparison, string expectedResult)
        => Assert.That(() => value.TrimPrefix(prefix, comparison), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, "validPostfix", StringComparison.Ordinal, "value")]
    [TestCase("validValue", null, StringComparison.InvariantCulture, "prefix")]
    public void TestTrimPrefixWhenInvalidArgumentsThenThrows(string? value, string? prefix, StringComparison comparison, string parameterName)
        => Assert.That(
            () => value!.TrimPrefix(prefix!, comparison),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo(parameterName));

    [Test]
    [TestCase("", "", StringComparison.Ordinal, "")]
    [TestCase("", "Hello", StringComparison.Ordinal, "")]
    [TestCase("Hello", "", StringComparison.Ordinal, "Hello")]
    [TestCase("Hello", "lo", StringComparison.Ordinal, "Hel")]
    [TestCase("Hello", "Lo", StringComparison.Ordinal, "Hello")]
    [TestCase("Hello", "Lo", StringComparison.OrdinalIgnoreCase, "Hel")]
    [TestCase("HelloWorld", "hELLOwORLD", StringComparison.OrdinalIgnoreCase, "")]
    public void TestTrimPostfixWhenValidArgumentsThenSucceeds(string value, string postfix, StringComparison comparison, string expectedResult)
        => Assert.That(() => value.TrimPostfix(postfix, comparison), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, "somePostfix", StringComparison.Ordinal, "value")]
    [TestCase("value", null, StringComparison.InvariantCulture, "postfix")]
    public void TestTrimPostfixWhenInvalidArgumentsThenThrows(string? value, string? postfix, StringComparison comparison, string parameterName)
        => Assert.That(
            () => value!.TrimPostfix(postfix!, comparison),
            Throws.ArgumentNullException.With.Property(nameof(ArgumentNullException.ParamName)).EqualTo(parameterName));

    [Test]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void TestShortenWhenInvalidArgumentsThenThrows(int maximumLength)
        => Assert.That(() => "foo".Shorten(maximumLength), Throws.TypeOf<ArgumentOutOfRangeException>());

    [Test]
    [TestCase(null, 0, "")]
    [TestCase(null, int.MaxValue, "")]
    [TestCase("", 0, "")]
    [TestCase("", int.MaxValue, "")]
    [TestCase("\u0020A B C\u0020", 0, "")]
    [TestCase("\u0020A B C\u0020", 3, "\u0020A\u0020")]
    public void TestShortenWhenValidArgumentsThenSucceeds(string value, int maximumLength, string expectedResult)
        => Assert.That(() => value.Shorten(maximumLength), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(-1)]
    [TestCase(int.MinValue)]
    public void TestReplicateWhenInvalidArgumentsThenThrows(int count)
        => Assert.That(() => "ABC".Replicate(count), Throws.TypeOf<ArgumentOutOfRangeException>());

    [Test]
    [TestCase(null, 0, "")]
    [TestCase(null, int.MaxValue, "")]
    [TestCase("", 0, "")]
    [TestCase("", int.MaxValue, "")]
    [TestCase("\u0020A\u0020B\u0020C\u0020", 0, "")]
    [TestCase("\u0020A\u0020B\u0020C\u0020", 3, "\u0020A\u0020B\u0020C\u0020\u0020A\u0020B\u0020C\u0020\u0020A\u0020B\u0020C\u0020")]
    public void TestReplicateWhenValidArgumentsThenSucceeds(string value, int count, string expectedResult)
        => Assert.That(() => value.Replicate(count), Is.EqualTo(expectedResult));

    [Test]
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("\u0020\t\r\n", false)]
    [TestCase("/a/b/c", false)]
    [TestCase("ftp://a/b/c", false)]
    [TestCase("news://a/b/c", false)]
    [TestCase("nntp://a/b/c", false)]
    [TestCase("gopher://a/b/c", false)]
    [TestCase("mailto:john@example.com", false)]
    [TestCase("mailto://john@example.com", false)]
    [TestCase("file://a/b/c", false)]
    [TestCase("net.pipe://a/b/c", false)]
    [TestCase("net.tcp://a/b/c", false)]
    [TestCase("http://a/b/c", true)]
    [TestCase("https://a/b/c", true)]
    public void TestIsWebUriSucceeds(string value, bool expectedResult) => Assert.That(value.IsWebUri, Is.EqualTo(expectedResult));

    [Test]
    public void TestWithSingleLeadingSlashWhenInvalidArgumentThenThrows()
        => Assert.That(() => default(string)!.WithSingleLeadingSlash(), Throws.ArgumentNullException);

    [Test]
    [TestCase("", "/", false)]
    [TestCase("foo", "/foo", false)]
    [TestCase("foo/", "/foo/", false)]
    [TestCase("foo//////////", "/foo//////////", false)]
    [TestCase("foo/bar", "/foo/bar", false)]
    [TestCase("/foo/bar", "/foo/bar", true)]
    [TestCase("foo/bar/", "/foo/bar/", false)]
    [TestCase("/foo/bar/", "/foo/bar/", true)]
    [TestCase("foo/bar//////////", "/foo/bar//////////", false)]
    [TestCase("//foo/bar//////////", "/foo/bar//////////", false)]
    [TestCase("//foo//bar//////////", "/foo//bar//////////", false)]
    [TestCase("mailto://example.com", "/mailto://example.com", false)]
    [TestCase("https://example.com", "/https://example.com", false)]
    public void TestWithSingleLeadingSlashWhenValidArgumentThenSucceeds(
        string value,
        string expectedResult,
        bool isSameObjectAsInput)
    {
        var actualResult = value.WithSingleLeadingSlash();
        Assert.That(actualResult, Is.EqualTo(expectedResult));

        Constraint referenceConstraint = Is.SameAs(value);
        if (!isSameObjectAsInput)
        {
            referenceConstraint = !referenceConstraint;
        }

        Assert.That(actualResult, referenceConstraint);
    }

    [Test]
    public void TestWithoutLeadingSlashWhenInvalidArgumentThenThrows()
        => Assert.That(() => default(string)!.WithoutLeadingSlash(), Throws.ArgumentNullException);

    [Test]
    [TestCase("", "", true)]
    [TestCase("foo", "foo", true)]
    [TestCase("foo/", "foo/", true)]
    [TestCase("foo//////////", "foo//////////", true)]
    [TestCase("foo/bar", "foo/bar", true)]
    [TestCase("/foo/bar", "foo/bar", false)]
    [TestCase("foo/bar/", "foo/bar/", true)]
    [TestCase("/foo/bar/", "foo/bar/", false)]
    [TestCase("foo/bar//////////", "foo/bar//////////", true)]
    [TestCase("//foo/bar//////////", "foo/bar//////////", false)]
    [TestCase("//foo//bar//////////", "foo//bar//////////", false)]
    [TestCase("mailto://example.com", "mailto://example.com", true)]
    [TestCase("https://example.com", "https://example.com", true)]
    public void TestWithoutLeadingSlashWhenValidArgumentThenSucceeds(
        string value,
        string expectedResult,
        bool isSameObjectAsInput)
    {
        var actualResult = value.WithoutLeadingSlash();
        Assert.That(actualResult, Is.EqualTo(expectedResult));

        Constraint referenceConstraint = Is.SameAs(value);
        if (!isSameObjectAsInput)
        {
            referenceConstraint = !referenceConstraint;
        }

        Assert.That(actualResult, referenceConstraint);
    }

    [Test]
    public void TestWithSingleTrailingSlashWhenInvalidArgumentThenThrows()
        => Assert.That(() => default(string)!.WithSingleTrailingSlash(), Throws.ArgumentNullException);

    [Test]
    [TestCase("", "/", false)]
    [TestCase("foo", "foo/", false)]
    [TestCase("foo/", "foo/", true)]
    [TestCase("foo//////////", "foo/", false)]
    [TestCase("foo/bar", "foo/bar/", false)]
    [TestCase("/foo/bar", "/foo/bar/", false)]
    [TestCase("foo/bar/", "foo/bar/", true)]
    [TestCase("/foo/bar/", "/foo/bar/", true)]
    [TestCase("foo/bar//////////", "foo/bar/", false)]
    [TestCase("//foo/bar//////////", "//foo/bar/", false)]
    [TestCase("//foo//bar//////////", "//foo//bar/", false)]
    [TestCase("mailto://example.com", "mailto://example.com/", false)]
    [TestCase("https://example.com", "https://example.com/", false)]
    [TestCase("https://example.com/", "https://example.com/", true)]
    [TestCase("https://example.com//////////", "https://example.com/", false)]
    [TestCase("https://example.com/api/v1", "https://example.com/api/v1/", false)]
    [TestCase("https://example.com/api/v1/", "https://example.com/api/v1/", true)]
    [TestCase("https://example.com/api/v1//////////", "https://example.com/api/v1/", false)]
    public void TestWithSingleTrailingSlashWhenValidArgumentThenSucceeds(
        string value,
        string expectedResult,
        bool isSameObjectAsInput)
    {
        var actualResult = value.WithSingleTrailingSlash();
        Assert.That(actualResult, Is.EqualTo(expectedResult));

        Constraint referenceConstraint = Is.SameAs(value);
        if (!isSameObjectAsInput)
        {
            referenceConstraint = !referenceConstraint;
        }

        Assert.That(actualResult, referenceConstraint);
    }

    [Test]
    public void TestWithoutTrailingSlashWhenInvalidArgumentThenThrows()
        => Assert.That(() => default(string)!.WithoutTrailingSlash(), Throws.ArgumentNullException);

    [Test]
    [TestCase("", "", true)]
    [TestCase("foo", "foo", true)]
    [TestCase("foo/", "foo", false)]
    [TestCase("foo//////////", "foo", false)]
    [TestCase("foo/bar", "foo/bar", true)]
    [TestCase("/foo/bar", "/foo/bar", true)]
    [TestCase("foo/bar/", "foo/bar", false)]
    [TestCase("/foo/bar/", "/foo/bar", false)]
    [TestCase("foo/bar//////////", "foo/bar", false)]
    [TestCase("//foo/bar//////////", "//foo/bar", false)]
    [TestCase("//foo//bar//////////", "//foo//bar", false)]
    [TestCase("mailto://example.com", "mailto://example.com", true)]
    [TestCase("https://example.com", "https://example.com", true)]
    [TestCase("https://example.com/", "https://example.com", false)]
    [TestCase("https://example.com//////////", "https://example.com", false)]
    [TestCase("https://example.com/api/v1", "https://example.com/api/v1", true)]
    [TestCase("https://example.com/api/v1/", "https://example.com/api/v1", false)]
    [TestCase("https://example.com/api/v1//////////", "https://example.com/api/v1", false)]
    public void TestWithoutTrailingSlashWhenValidArgumentThenSucceeds(
        string value,
        string expectedResult,
        bool isSameObjectAsInput)
    {
        var actualResult = value.WithoutTrailingSlash();
        Assert.That(actualResult, Is.EqualTo(expectedResult));

        Constraint referenceConstraint = Is.SameAs(value);
        if (!isSameObjectAsInput)
        {
            referenceConstraint = !referenceConstraint;
        }

        Assert.That(actualResult, referenceConstraint);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("\u0020")]
    [TestCase("\t")]
    [TestCase("\r")]
    [TestCase("\n")]
    [TestCase(@"breaKFAst/завтрак/朝ごはん/petiT-DÉjeuner/早餐/")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public void TestToSecureString(string? plainText)
    {
        using var secureString = plainText.ToSecureString();

        if (plainText is null)
        {
            Assert.That(secureString, Is.Null);
            return;
        }

        var actualResult = new NetworkCredential(string.Empty, secureString).Password;
        Assert.That(actualResult, Is.EqualTo(plainText));
    }
}