using System;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class NotNullOrWhiteSpaceStringConstraintTests : ConstraintTestsBase
    {
        #region Tests

        [Test]
        public void TestValidateNegativeContextNull()
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullOrWhiteSpaceStringConstraint();

            Assert.That(
                //// ReSharper disable once AssignNullToNotNullAttribute - OK for test
                () => constraint.Validate(objectValidatorContext, null, "A"),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestValidateNegativeInvalidValueType()
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullOrWhiteSpaceStringConstraint();
            var validationContext = CreateTestValidationContext();

            Assert.That(
                () => constraint.Validate(objectValidatorContext, validationContext, this),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        [TestCase("A")]
        public void TestValidateAgainstNotNullOrEmptyOrWhiteSpace(string value)
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullOrWhiteSpaceStringConstraint();
            var validationContext = CreateTestValidationContext();
            constraint.Validate(objectValidatorContext, validationContext, value);
            var validationErrors = objectValidatorContext.Errors.Items;

            Assert.That(validationErrors, Is.Not.Null);
            Assert.That(validationErrors, Is.Empty);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void TestValidateAgainstInvalidValue(string value)
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullOrWhiteSpaceStringConstraint();
            var validationContext = CreateTestValidationContext();

            constraint.Validate(objectValidatorContext, validationContext, value);
            var validationError = objectValidatorContext.Errors.Items.Single();

            Assert.That(validationError, Is.Not.Null);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(constraint.GetType()));
            Assert.That(validationError.Context, Is.SameAs(validationContext));
        }

        #endregion
    }
}