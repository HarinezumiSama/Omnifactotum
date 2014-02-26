using System;
using System.Linq;
using NUnit.Framework;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests
{
    [TestFixture]
    public sealed class NotNullOrEmptyStringConstraintTests : ConstraintTestsBase
    {
        #region Tests

        [Test]
        public void TestValidateNegativeContextNull()
        {
            var constraint = new NotNullOrEmptyStringConstraint();

            Assert.That(() => constraint.Validate(null, "A"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestValidateNegativeInvalidValueType()
        {
            var constraint = new NotNullOrEmptyStringConstraint();
            var validationContext = CreateTestValidationContext();

            Assert.That(
                () => constraint.Validate(validationContext, this),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        [TestCase("A")]
        [TestCase(" ")]
        public void TestValidateAgainstNotNullOrEmpty(string value)
        {
            var constraint = new NotNullOrEmptyStringConstraint();
            var validationContext = CreateTestValidationContext();
            var validationError = constraint.Validate(validationContext, value);

            Assert.That(validationError, Is.Null);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void TestValidateAgainstInvalidValue(string value)
        {
            var constraint = new NotNullOrEmptyStringConstraint();
            var validationContext = CreateTestValidationContext();
            var validationError = constraint.Validate(validationContext, value);

            Assert.That(validationError, Is.Not.Null);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(constraint.GetType()));
            Assert.That(validationError.Context, Is.SameAs(validationContext));
        }

        #endregion
    }
}