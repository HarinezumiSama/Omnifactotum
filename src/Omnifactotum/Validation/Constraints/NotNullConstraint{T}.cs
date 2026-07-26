using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     [DEPRECATED] Specifies that the annotated member should not be <see langword="null"/> or an uninitialized <see cref="ImmutableArray{T}"/>.
/// </summary>
/// <typeparam name="T">
///     The type of the value to validate.
/// </typeparam>
[Obsolete(
    $"Use '{nameof(NotNullConstraint)}.{nameof(NotNullConstraint.Ref<>)}<{nameof(T)}>' or '{
        nameof(NotNullConstraint)}.{nameof(NotNullConstraint.NullableRef<>)}<{nameof(T)}>' instead.")]
public sealed class NotNullConstraint<T> : TypedMemberConstraintBase<T?>
    where T : class
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "False detection.")]
    private static readonly ValidationErrorDetails FailureMessage = AsInvariant($"The '{ValueTypeQualifiedName}' value must not be null.");

    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T? value)
    {
        if (value is null or ImmutableArray<T> { IsDefault: true })
        {
            AddError(memberContext, FailureMessage);
        }
    }
}