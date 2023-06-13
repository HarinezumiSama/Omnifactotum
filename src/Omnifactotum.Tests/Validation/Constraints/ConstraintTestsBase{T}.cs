﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
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
        var objectValidatorContext = CreateObjectValidatorContext();
        var memberContext = CreateMemberConstraintValidationContext();
        var testee = CreateTestee();

        Assert.That(
            () => testee.Validate(null!, memberContext, new UnknownClass()),
            Throws.TypeOf<ArgumentNullException>());

        Assert.That(
            () => testee.Validate(objectValidatorContext, null!, new UnknownClass()),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public virtual void TestValidateWhenIncorrectValueTypeThenThrows()
    {
        var objectValidatorContext = CreateObjectValidatorContext();
        var testee = CreateTestee();
        var validationContext = CreateMemberConstraintValidationContext();

        Assert.That(
            () => testee.Validate(objectValidatorContext, validationContext, new UnknownClass()),
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

            var testCaseDetails = $"Test case value: {FormatTestCaseValue(validValue)}";

            var objectValidatorContext = CreateObjectValidatorContext();
            var testee = CreateTestee();
            var memberContext = CreateMemberConstraintValidationContext();
            testee.Validate(objectValidatorContext, memberContext, validValue);
            var validationErrors = objectValidatorContext.Errors.Items;

            Assert.That(validationErrors, Is.Not.Null & Is.Empty, testCaseDetails);
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

            var testCaseDetails = $"Test case value: {FormatTestCaseValue(invalidValue)}";

            var expectedErrorMessage = GetInvalidValueErrorMessage(invalidValue).AssertNotNull();

            var objectValidatorContext = CreateObjectValidatorContext();
            var testee = CreateTestee();
            var validationContext = CreateMemberConstraintValidationContext();

            testee.Validate(objectValidatorContext, validationContext, invalidValue);

            var validationErrors = objectValidatorContext.Errors.Items;
            Assert.That(validationErrors, Is.Not.Null & Has.Count.EqualTo(1), testCaseDetails);

            var validationError = validationErrors.Single();
            Assert.That(validationError, Is.Not.Null, testCaseDetails);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(testee.GetType()), testCaseDetails);
            Assert.That(validationError.Context, Is.SameAs(validationContext), testCaseDetails);
            Assert.That(validationError.ErrorMessage, Is.EqualTo(expectedErrorMessage), testCaseDetails);
        }

        Assert.That(atLeastOneValue, Is.True);
    }

    //// ReSharper disable once MemberCanBePrivate.Global
    protected static TConstraint CreateTestee() => new();

    protected virtual void OnTestConstruction(TConstraint testee) => testee.AssertNotNull();

    protected abstract IEnumerable<object?> GetValidValues();

    protected abstract IEnumerable<object?> GetInvalidValues();

    protected abstract string GetInvalidValueErrorMessage(object? invalidValue);

    private static string FormatTestCaseValue(object? value)
        => value switch
        {
            null => OmnifactotumRepresentationConstants.NullValueRepresentation,
            string s => s.ToUIString(),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString().AvoidNull()
        };

    private sealed class UnknownClass
    {
        // No members
    }
}