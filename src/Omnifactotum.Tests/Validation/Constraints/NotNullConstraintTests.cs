using System;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    [TestFixture]
    internal sealed class NotNullConstraintTests : ConstraintTestsBase
    {
        [Test]
        public void TestValidateNegativeContextNull()
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullConstraint();

            Assert.That(
                //// ReSharper disable once AssignNullToNotNullAttribute - OK for test
                () => constraint.Validate(objectValidatorContext, null, new object()),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestValidateAgainstNotNull()
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullConstraint();
            var validationContext = CreateTestValidationContext();

            constraint.Validate(objectValidatorContext, validationContext, new object());
            var validationErrors = objectValidatorContext.Errors.Items;

            Assert.That(validationErrors, Is.Not.Null);
            Assert.That(validationErrors, Is.Empty);
        }

        [Test]
        public void TestValidateAgainstNull()
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullConstraint();
            var validationContext = CreateTestValidationContext();

            constraint.Validate(objectValidatorContext, validationContext, null);
            var validationError = objectValidatorContext.Errors.Items.Single();

            Assert.That(validationError, Is.Not.Null);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(constraint.GetType()));
            Assert.That(validationError.Context, Is.SameAs(validationContext));
        }
    }
}