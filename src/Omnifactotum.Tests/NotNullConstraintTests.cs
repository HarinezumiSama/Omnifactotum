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
            var constraint = new NotNullConstraint();

            Assert.That(() => constraint.Validate(null, new object()), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestValidateAgainstNotNull()
        {
            var constraint = new NotNullConstraint();
            var validationContext = CreateTestValidationContext();
            var validationError = constraint.Validate(validationContext, new object());

            Assert.That(validationError, Is.Null);
        }

        [Test]
        public void TestValidateAgainstNull()
        {
            var constraint = new NotNullConstraint();
            var validationContext = CreateTestValidationContext();
            var validationError = constraint.Validate(validationContext, null);

            Assert.That(validationError, Is.Not.Null);
            Assert.That(validationError.FailedConstraintType, Is.EqualTo(constraint.GetType()));
            Assert.That(validationError.Context, Is.SameAs(validationContext));
        }

        #endregion
    }
}