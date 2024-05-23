using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member should not be <see langword="null"/> or an uninitialized <see cref="ImmutableArray{T}"/>.
/// </summary>
/// <typeparam name="T">
///     The type of the value to validate.
/// </typeparam>
/// <seealso cref="ImmutableArray{T}.IsDefault"/>
public sealed class NotNullConstraint<T> : TypedMemberConstraintBase<T?>
    where T : class
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "False detection.")]
    private static readonly ValidationErrorDetails FailureMessage = AsInvariant($"The '{ValueTypeQualifiedName}' value cannot be null.");

    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T? value)
    {
        if (value is null or ImmutableArray<T> { IsDefault: true })
        {
            AddError(memberContext, FailureMessage);
        }
    }
}