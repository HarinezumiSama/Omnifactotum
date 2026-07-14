namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     <para>
///         Represents the strongly typed constraint that does not perform any validation.
///     </para>
///     <para>
///         Can be used for cases when a constraint must be specified, but no validation is needed for this particular value/type
///         (for instance, in <see cref="KeyValuePairConstraint{TKey,TValue,TKeyConstraint,TValueConstraint}"/>).
///     </para>
/// </summary>
/// <typeparam name="T">
///     The type of the value to validate.
/// </typeparam>
/// <example>
///     <code>
/// <![CDATA[
///         [MemberItemConstraint(typeof(KeyValuePairConstraint<string, Account, NotNullAndNotBlankStringConstraint, IgnoredConstraint<Account>>))]
///         public Dictionary<string, Account> AccountMap { get; set; }
/// ]]>
///     </code>
/// </example>
public sealed class IgnoredConstraint<T> : TypedMemberConstraintBase<T>
{
    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T value)
    {
        // Nothing to do
    }
}