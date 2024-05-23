namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents the strongly-typed constraint that ignores validation.
///     Can be used for cases when a constraint must be specified but validation is not needed
///     (for instance, in <see cref="KeyValuePairConstraint{TKey,TValue,TKeyConstraint,TValueConstraint}"/>).
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