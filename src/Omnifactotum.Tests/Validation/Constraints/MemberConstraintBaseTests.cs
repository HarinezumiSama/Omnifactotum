using System;
using NUnit.Framework;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation.Constraints
{
    [TestFixture]
    public sealed class MemberConstraintBaseTests
    {
        [Test]
        [TestCase(null)]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void TestCastToSucceedsWithValidInput(object value)
        {
            var testee = new ExposedMemberConstraintBase();
            var castValue = testee.CallCastTo<int?>(value);

            Assert.That(castValue, Is.EqualTo(value));
            if (value == null)
            {
                Assert.That(castValue.HasValue, Is.False);
            }
            else
            {
                // ReSharper disable once PossibleInvalidOperationException
                Assert.That(castValue.Value, Is.EqualTo((int)value));
            }
        }

        [Test]
        public void TestCastToFailsWithInvalidInput()
        {
            var testee = new ExposedMemberConstraintBase();
            Assert.That(() => testee.CallCastTo<int?>(new object()), Throws.InvalidOperationException);
            Assert.That(() => testee.CallCastTo<int?>("1"), Throws.InvalidOperationException);
            Assert.That(() => testee.CallCastTo<string>(1), Throws.InvalidOperationException);
            Assert.That(() => testee.CallCastTo<int>(null), Throws.InvalidOperationException);
        }

        private sealed class ExposedMemberConstraintBase : MemberConstraintBase
        {
            public TTarget CallCastTo<TTarget>(object value)
            {
                return CastTo<TTarget>(value);
            }

            protected override void ValidateValue(
                ObjectValidatorContext validatorContext,
                MemberConstraintValidationContext memberContext,
                object value)
            {
                throw new NotImplementedException();
            }
        }
    }
}