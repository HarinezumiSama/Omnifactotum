using System;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     [Internal] A base class for legacy constraints, that forwards the logic to the respective modern implementations.
/// </summary>
/// <typeparam name="T">
///     The type of the value to validate.
/// </typeparam>
/// <typeparam name="TModernConstraint">
///     The modern constraint that implements value validation.
/// </typeparam>
public abstract class LegacyTypedMemberConstraintBase<T, TModernConstraint> : TypedMemberConstraintBase<T>, ILegacyTypedMemberConstraint
    where TModernConstraint : TypedMemberConstraintBase<T>
{
    private readonly TModernConstraint _constraint;

    private protected LegacyTypedMemberConstraintBase(TModernConstraint constraint)
        => _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));

    private protected virtual Type ActualConstraintType => typeof(TModernConstraint);

    Type ILegacyTypedMemberConstraint.ActualConstraintType => ActualConstraintType;

    /// <inheritdoc />
    protected sealed override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T value)
        => _constraint.InternalValidateTypedValue(memberContext, value);
}