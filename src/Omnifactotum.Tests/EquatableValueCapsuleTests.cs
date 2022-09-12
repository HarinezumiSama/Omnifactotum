using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Tests;

[TestFixture(TestOf = typeof(EquatableValueCapsule<>))]
internal sealed class EquatableValueCapsuleTests
{
    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("85dc4bfa130e4c26ad634343a23fda82")]
    public void TestConstruction(string? value)
    {
        var capsule = new CaseInsensitiveStringEquatableValueCapsule(value);
        Assert.That(() => capsule.Value, Is.EqualTo(value));
    }

    [Test]
    public void TestValuePropertyIsReadOnly()
    {
        var valuePropertyInfo = typeof(EquatableValueCapsule<>).GetProperty(
            nameof(EquatableValueCapsule<object>.Value),
            BindingFlags.Instance | BindingFlags.Public);

        Assert.That(valuePropertyInfo, Is.Not.Null);

        Assert.That(() => valuePropertyInfo!.GetGetMethod(false), Is.Not.Null);
        Assert.That(() => valuePropertyInfo!.GetSetMethod(true), Is.Null);
    }

    [Test]
    [TestCase(null, null, true, true)]
    [TestCase(null, @"Value", false, false)]
    [TestCase(@"Value", @"Value", true, true)]
    //// ReSharper disable once StringLiteralTypo :: Test value
    [TestCase(@"Value", @"vALUE", true, false)]
    [TestCase(@"Value", @"V@lue", false, false)]
    public void TestEquality(
        string? value1,
        string? value2,
        bool isCaseInsensitiveStringEqual,
        bool isRegularStringEqual)
    {
        var caseInsensitiveCapsule1 = new CaseInsensitiveStringEquatableValueCapsule(value1);
        var caseInsensitiveCapsule2 = new CaseInsensitiveStringEquatableValueCapsule(value2);

        NUnitFactotum.AssertEquality(
            caseInsensitiveCapsule1,
            caseInsensitiveCapsule2,
            isCaseInsensitiveStringEqual
                ? AssertEqualityExpectation.EqualAndCannotBeSame
                : AssertEqualityExpectation.NotEqual);

        Assert.That(() => caseInsensitiveCapsule1 == caseInsensitiveCapsule2, Is.EqualTo(isCaseInsensitiveStringEqual));
        Assert.That(() => caseInsensitiveCapsule1 != caseInsensitiveCapsule2, Is.EqualTo(!isCaseInsensitiveStringEqual));

        var regularCapsule1 = new RegularStringEquatableValueCapsule(value1);
        var regularCapsule2 = new RegularStringEquatableValueCapsule(value2);

        NUnitFactotum.AssertEquality(
            regularCapsule1,
            regularCapsule2,
            isRegularStringEqual
                ? AssertEqualityExpectation.EqualAndCannotBeSame
                : AssertEqualityExpectation.NotEqual);

        Assert.That(() => regularCapsule1 == regularCapsule2, Is.EqualTo(isRegularStringEqual));
        Assert.That(() => regularCapsule1 != regularCapsule2, Is.EqualTo(!isRegularStringEqual));
    }

    private sealed class RegularStringEquatableValueCapsule : EquatableValueCapsule<string?>
    {
        public RegularStringEquatableValueCapsule([CanBeNull] string? value)
            : base(value)
        {
            // Nothing to do
        }
    }

    private sealed class CaseInsensitiveStringEquatableValueCapsule : EquatableValueCapsule<string?>
    {
        public CaseInsensitiveStringEquatableValueCapsule([CanBeNull] string? value)
            : base(value)
        {
            // Nothing to do
        }

        protected override IEqualityComparer<string?> GetValueEqualityComparer() => StringComparer.OrdinalIgnoreCase;
    }
}