#nullable enable

using System;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Tests.Validation.Constraints
{
    [TestFixture(TestOf = typeof(MemberConstraintBase))]
    internal sealed class MemberConstraintBaseTests : ConstraintTestsBase
    {
        [Test]
        public void TestValidateWhenInvalidContextArgumentThenThrows()
        {
            var objectValidatorContext = CreateObjectValidatorContext();
            var memberContext = CreateMemberConstraintValidationContext();
            var testee = CreateTestee();

            Assert.That(
                () => testee.Validate(null!, memberContext, this),
                Throws.TypeOf<ArgumentNullException>());

            Assert.That(
                () => testee.Validate(objectValidatorContext, null!, this),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCase(null)]
        [TestCase(int.MinValue)]
        [TestCase(0)]
        [TestCase(int.MaxValue)]
        public void TestCastToWhenValidInputThenSucceeds(object? value)
        {
            var testee = CreateTestee();
            var castValue = testee.CallCastTo<int?>(value);

            Assert.That(castValue, Is.EqualTo(value));
            if (value is null)
            {
                Assert.That(castValue.HasValue, Is.False);
            }
            else
            {
                Assert.That(castValue!.Value, Is.EqualTo((int)value));
            }
        }

        [Test]
        public void TestCastToWhenInvalidInputThenThrows()
        {
            var testee = CreateTestee();
            Assert.That(() => testee.CallCastTo<int?>(new object()), Throws.InvalidOperationException);
            Assert.That(() => testee.CallCastTo<int?>("1"), Throws.InvalidOperationException);
            Assert.That(() => testee.CallCastTo<string>(1), Throws.InvalidOperationException);
            Assert.That(() => testee.CallCastTo<int>(null), Throws.InvalidOperationException);
        }

        [NotNull]
        private static ExposedMemberConstraintBase CreateTestee() => new();

        private sealed class ExposedMemberConstraintBase : MemberConstraintBase
        {
            public TTarget CallCastTo<TTarget>([CanBeNull] object? value) => CastTo<TTarget>(value);

            protected override void ValidateValue(
                ObjectValidatorContext validatorContext,
                MemberConstraintValidationContext memberContext,
                object? value)
                => throw new NotImplementedException();
        }
    }
}