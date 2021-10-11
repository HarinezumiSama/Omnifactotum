using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    internal abstract class ConstraintTestsBase<TConstraint> : ConstraintTestsBase
        where TConstraint : IMemberConstraint, new()
    {
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
            var validValues = GetValidValues();
            foreach (var validValue in validValues)
            {
                var objectValidatorContext = CreateObjectValidatorContext();
                var testee = CreateTestee();
                var memberContext = CreateMemberConstraintValidationContext();
                testee.Validate(objectValidatorContext, memberContext, validValue);
                var validationErrors = objectValidatorContext.Errors.Items;

                Assert.That(validationErrors, Is.Not.Null & Is.Empty);
            }
        }

        [Test]
        public void TestValidateWhenInvalidValueThenSingleValidationError()
        {
            var invalidValues = GetInvalidValues();
            foreach (var invalidValue in invalidValues)
            {
                var objectValidatorContext = CreateObjectValidatorContext();
                var testee = CreateTestee();
                var validationContext = CreateMemberConstraintValidationContext();

                testee.Validate(objectValidatorContext, validationContext, invalidValue);
                var validationError = objectValidatorContext.Errors.Items.Single();

                Assert.That(validationError, Is.Not.Null);
                Assert.That(validationError.FailedConstraintType, Is.EqualTo(testee.GetType()));
                Assert.That(validationError.Context, Is.SameAs(validationContext));
            }
        }

        protected static TConstraint CreateTestee() => new();

        protected abstract IEnumerable<object> GetValidValues();

        protected abstract IEnumerable<object> GetInvalidValues();

        private sealed class UnknownClass
        {
            // No members
        }
    }
}