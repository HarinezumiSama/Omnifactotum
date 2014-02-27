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
            var constraint = new NotNullOrWhiteSpaceStringConstraint();

            Assert.That(() => constraint.Validate(null, "A"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestValidateNegativeInvalidValueType()
        {
            var constraint = new NotNullOrWhiteSpaceStringConstraint();
            var validationContext = CreateTestValidationContext();

            Assert.That(
                () => constraint.Validate(validationContext, this),
                Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        [TestCase("A")]
        public void TestValidateAgainstNotNullOrEmptyOrWhiteSpace(string value)
        {
            var constraint = new NotNullOrWhiteSpaceStringConstraint();
            var validationContext = CreateTestValidationContext();
            var validationError = constraint.Validate(validationContext, value);

            Assert.That(validationError, Is.Null);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void TestValidateAgainstInvalidValue(string value)
        {
            var constraint = new NotNullOrWhiteSpaceStringConstraint();
            var validationContext = CreateTestValidationContext();
            var validationError = constraint.Validate(validationContext, value);

            Assert.That(validationError, Is.Not.Null);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(constraint.GetType()));
            Assert.That(validationError.Context, Is.SameAs(validationContext));
        }

        #endregion
    }
}