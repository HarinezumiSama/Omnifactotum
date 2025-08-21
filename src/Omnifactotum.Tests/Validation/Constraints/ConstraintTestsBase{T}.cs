using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints;

internal abstract class ConstraintTestsBase<[MeansImplicitUse] TConstraint> : ConstraintTestsBase
    where TConstraint : class, IMemberConstraint, new()
{
    [Test]
    public void TestConstruction()
    {
        var testee = CreateTestee();
        OnTestConstruction(testee);
    }

    [Test]
    public void TestValidateWhenContextArgumentIsNullThenThrows()
    {
        var testee = CreateTestee();

        Assert.That(
            () => testee.Validate(null!, new UnknownClass()),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public virtual void TestValidateWhenIncorrectValueTypeThenThrows()
    {
        var testee = CreateTestee();
        var memberContext = CreateMemberConstraintValidationContext();

        Assert.That(
            () => testee.Validate(memberContext, new UnknownClass()),
            Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void TestValidateWhenValidValueThenNoValidationErrors()
    {
        var atLeastOneValue = false;

        var validValues = GetValidValues();
        foreach (var validValue in validValues)
        {
            atLeastOneValue = true;

            var testCaseDetails = GetTestCaseDetails(validValue);

            var testee = CreateTestee();
            var memberContext = CreateMemberConstraintValidationContext();
            testee.Validate(memberContext, validValue);
            var validationErrors = memberContext.ValidatorContext.Errors;

            Assert.That(validationErrors, Is.Not.Null, testCaseDetails);
            Assert.That(validationErrors, Is.Empty, testCaseDetails);
        }

        Assert.That(atLeastOneValue, Is.True);
    }

    [Test]
    public void TestValidateWhenInvalidValueThenSingleValidationError()
    {
        var atLeastOneValue = false;

        var invalidValues = GetInvalidValues();
        foreach (var invalidValue in invalidValues)
        {
            atLeastOneValue = true;

            var testCaseDetails = GetTestCaseDetails(invalidValue);
            var expectedErrorDetails = GetInvalidValueErrorDetails(invalidValue).AssertNotNull();

            var testee = CreateTestee();
            var memberContext = CreateMemberConstraintValidationContext();

            testee.Validate(memberContext, invalidValue);

            var validationErrors = memberContext.ValidatorContext.Errors;
            Assert.That(validationErrors, Is.Not.Null, testCaseDetails);
            Assert.That(validationErrors, Has.Count.EqualTo(1), $"{testCaseDetails} There must be exactly one error");

            var expectedFailedConstraintType = testee.GetType();

            var validationError = validationErrors.Single();
            Assert.That(validationError, Is.Not.Null, testCaseDetails);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(expectedFailedConstraintType), testCaseDetails);
            Assert.That(validationError.Context, Is.SameAs(memberContext), testCaseDetails);
            Assert.That(validationError.Details.Text, Is.EqualTo(expectedErrorDetails.Text), testCaseDetails);
            Assert.That(validationError.Details.Description, Is.EqualTo(expectedErrorDetails.Description), testCaseDetails);
        }

        Assert.That(atLeastOneValue, Is.True);
    }

    //// ReSharper disable once MemberCanBePrivate.Global
    protected static TConstraint CreateTestee() => new();

    protected virtual void OnTestConstruction(TConstraint testee) => testee.AssertNotNull();

    protected abstract IEnumerable<object?> GetValidValues();

    protected abstract IEnumerable<object?> GetInvalidValues();

    protected abstract ValidationErrorDetails GetInvalidValueErrorDetails(object? invalidValue);

    private static string GetTestCaseDetails(object? value) => $"[Test case value: {FormatTestCaseValue(value)}]";

    private static string FormatTestCaseValue(object? value)
        => value switch
        {
            null => OmnifactotumRepresentationConstants.NullValueRepresentation,
            string s => s.ToUIString(),
            _ when ValidationFactotum.IsDefaultImmutableArray(value) => OmnifactotumRepresentationConstants.NullValueRepresentation,
            ICollection collection => $"{{ {collection.GetType().GetQualifiedName()} : {nameof(collection.Count)} = {collection.Count} }}",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString().AvoidNull()
        };

    protected sealed class PureReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private readonly ImmutableList<T> _items;

        public PureReadOnlyCollection(params T[] items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = items.ToImmutableList();
        }

        public static PureReadOnlyCollection<T> Empty { get; } = new();

        public int Count => _items.Count;

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    protected static class PureReadOnlyCollection
    {
        public static PureReadOnlyCollection<T> Create<T>(params T[] items) => new(items);
    }

    private sealed class UnknownClass
    {
        // No members
    }
}