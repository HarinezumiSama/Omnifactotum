using System;
using NUnit.Framework;
using Omnifactotum.Annotations;
using Omnifactotum.NUnit;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Tests.Validation;

internal sealed partial class ObjectValidatorTests
{
    private interface ILastMemberContextProvider
    {
#if NET7_0_OR_GREATER
        abstract static MemberConstraintValidationContext? LastMemberContext { get; set; }
#endif
    }

    private sealed class NotAbcStringConstraint : TypedMemberConstraintBase<string>, ILastMemberContextProvider
    {
        // Making sure that a public constructor is allowed for a member constraint
        [UsedImplicitly]
        public NotAbcStringConstraint()
        {
            // Nothing to do
        }

        public static MemberConstraintValidationContext? LastMemberContext { get; set; }

        protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, string value)
        {
            LastMemberContext = memberContext;

            if (value == "abc")
            {
                AddError(memberContext, null);
            }
        }
    }

    // ReSharper disable once ClassCanBeSealed.Local
    private class CustomNotAbcStringConstraint : IMemberConstraint, ILastMemberContextProvider
    {
        // Making sure that a public constructor is allowed for a member constraint
        [UsedImplicitly]
        private protected CustomNotAbcStringConstraint()
        {
            // Nothing to do
        }

        public static MemberConstraintValidationContext? LastMemberContext { get; set; }

        public void Validate(MemberConstraintValidationContext memberContext, object? value)
        {
            LastMemberContext = memberContext;

            if (value is "abc")
            {
                this.AddError(memberContext, null);
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

    // ReSharper disable once ClassCanBeSealed.Local
    private class CustomNotAbcStringMapKeyConstraint : KeyValuePairConstraintBase<string, SimpleContainer<int?>>
    {
        // Making sure that an protected internal constructor is allowed for a member constraint
        [UsedImplicitly]
        protected internal CustomNotAbcStringMapKeyConstraint()
            : base(typeof(CustomNotAbcStringConstraint), typeof(IgnoredConstraint<SimpleContainer<int?>>))
        {
            // Nothing to do
        }
    }

    private sealed class UtcDateConstraint : MemberConstraintBase, ILastMemberContextProvider
    {
        // Making sure that a private constructor is allowed for a member constraint
        [UsedImplicitly]
        private UtcDateConstraint()
        {
            // Nothing to do
        }

        public static MemberConstraintValidationContext? LastMemberContext { get; set; }

        protected override void ValidateValue(MemberConstraintValidationContext memberContext, object? value)
        {
            LastMemberContext = memberContext;

            var dateTime = (DateTime)value.AssertNotNull();
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return;
            }

            AddError(memberContext, null);
        }
    }

    private sealed class UtcDateTypedConstraint : TypedMemberConstraintBase<DateTime>, ILastMemberContextProvider
    {
        public static MemberConstraintValidationContext? LastMemberContext { get; set; }

        protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, DateTime value)
        {
            LastMemberContext = memberContext;

            if (value.Kind != DateTimeKind.Utc)
            {
                AddError(memberContext, null);
            }
        }
    }

    private sealed class NeverCalledConstraint : MemberConstraintBase
    {
        protected override void ValidateValue(MemberConstraintValidationContext memberContext, object? value)
        {
            const string Message = "This constraint is not supposed to be called ever.";

            Assert.Fail(Message);
            throw new InvalidOperationException(Message);
        }
    }
}