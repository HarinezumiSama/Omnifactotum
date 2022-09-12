using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NUnit.Framework;

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(FormattableStringFactotum))]
internal sealed class FormattableStringFactotumTests
{
    [Test]
    [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
    public void TestAsInvariantWhenInvalidArgumentThenThrows()
    {
        const FormattableString? SourceInterpolationValue = null;
        Assert.That(() => FormattableStringFactotum.AsInvariant(SourceInterpolationValue!), Throws.ArgumentNullException);
    }

    [Test]
    [SetCulture("ru-RU")]
    public void TestAsInvariantWhenValidArgumentThenSucceeds()
    {
        const decimal Number = 3_141_592_653.589m;
        var dateTime = new DateTime(2021, 5, 30, 13, 1, 51);

        //// ReSharper disable once StringLiteralTypo :: Non-English word
        const string CurrentCultureExpectedResult = @"3 141 592 653,589|воскресенье, 30.мая.2021 13:01:51";

        const string InvariantCultureExpectedResult = @"3,141,592,653.589|Sunday, 30/May/2021 13:01:51";

        FormattableString sourceInterpolationValue = $@"{Number:N3}|{dateTime:dddd, d/MMMM/yyyy HH:mm:ss}";

        var currentCultureActualResult = sourceInterpolationValue.ToString(CultureInfo.CurrentCulture);
        Assert.That(currentCultureActualResult, Is.EqualTo(CurrentCultureExpectedResult));

        var invariantCultureActualResult = FormattableStringFactotum.AsInvariant(sourceInterpolationValue);
        Assert.That(invariantCultureActualResult, Is.EqualTo(InvariantCultureExpectedResult));
    }
}