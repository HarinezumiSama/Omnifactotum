using System;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class NotNullConstraintTests : ConstraintTestsBase
    {
        #region Tests

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
            var validationErrors = constraint.Validate(objectValidatorContext, validationContext, new object());

            Assert.That(validationErrors, Is.Null);
        }

        [Test]
        public void TestValidateAgainstNull()
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var constraint = new NotNullConstraint();
            var validationContext = CreateTestValidationContext();
            var validationErrors = constraint.Validate(objectValidatorContext, validationContext, null);
            var validationError = validationErrors.Single();

            Assert.That(validationError, Is.Not.Null);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(constraint.GetType()));
            Assert.That(validationError.Context, Is.SameAs(validationContext));
        }

        #endregion
    }
}