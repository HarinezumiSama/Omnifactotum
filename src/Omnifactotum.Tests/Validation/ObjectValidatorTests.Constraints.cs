using System;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Validation;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation;

internal sealed partial class ObjectValidatorTests
{
    private sealed class NotAbcStringConstraint : TypedMemberConstraintBase<string>
    {
        // Making sure that a public constructor is allowed for a member constraint
        [UsedImplicitly]
        public NotAbcStringConstraint()
        {
            // Nothing to do
        }

        protected override void ValidateTypedValue(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, string value)
        {
            if (value == "abc")
            {
                AddDefaultError(validatorContext, memberContext);
            }
        }
    }

    private sealed class MapContainerPropertiesPairConstraint : KeyValuePairConstraintBase<string, SimpleContainer<int?>>
    {
        // Making sure that an internal constructor is allowed for a member constraint
        [UsedImplicitly]
        internal MapContainerPropertiesPairConstraint()
            : base(typeof(NotNullOrEmptyStringConstraint), typeof(NotNullConstraint<SimpleContainer<int?>>))
        {
            // Nothing to do
        }
    }

    private sealed class UtcDateConstraint : MemberConstraintBase
    {
        // Making sure that a private constructor is allowed for a member constraint
        [UsedImplicitly]
        private UtcDateConstraint()
        {
            // Nothing to do
        }

        protected override void ValidateValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            object? value)
        {
            var dateTime = (DateTime)value.AssertNotNull();
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return;
            }

            AddDefaultError(validatorContext, memberContext);
        }
    }

    private sealed class UtcDateTypedConstraint : TypedMemberConstraintBase<DateTime>
    {
        /// <inheritdoc />
        protected override void ValidateTypedValue(ObjectValidatorContext validatorContext, MemberConstraintValidationContext memberContext, DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                AddDefaultError(validatorContext, memberContext);
            }
        }
    }

    private sealed class NeverCalledConstraint : MemberConstraintBase
    {
        protected override void ValidateValue(
            ObjectValidatorContext validatorContext,
            MemberConstraintValidationContext memberContext,
            object? value)
        {
            const string Message = "This constraint is not supposed to be called ever.";

            Assert.Fail(Message);
            throw new InvalidOperationException(Message);
        }
    }
}