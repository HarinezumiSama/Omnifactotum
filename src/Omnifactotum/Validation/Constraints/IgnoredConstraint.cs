namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents the strongly-typed constraint that ignores validation.
/// </summary>
/// <typeparam name="T">
///     The type of the value to validate.
/// </typeparam>
public sealed class IgnoredConstraint<T> : TypedMemberConstraintBase<T>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T value)
    {
        // Nothing to do
    }
}