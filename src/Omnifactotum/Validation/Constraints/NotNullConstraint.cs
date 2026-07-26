using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member should not be <see langword="null"/> or an uninitialized <see cref="ImmutableArray{T}"/>.
/// </summary>
public sealed class NotNullConstraint : MemberConstraintBase
{
    /// <inheritdoc />
    protected override void ValidateValue(MemberConstraintValidationContext memberContext, object? value)
    {
        if (value is null || ValidationFactotum.IsDefaultImmutableArray(value))
        {
            AddError(memberContext, ValidationErrorDetails.Predefined.MustNotBeNull);
        }
    }

    /// <summary>
    ///     The base type for <see cref="Ref{T}"/>, <see cref="NullableRef{T}"/>, and <see cref="NullableValue{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to validate.
    /// </typeparam>
    public abstract class Base<T> : TypedMemberConstraintBase<T>
    {
        [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = "False detection.")]
        private protected static readonly ValidationErrorDetails FailureMessage = $"The '{ValueTypeQualifiedName}' value must not be null.";

        private protected Base()
        {
            // Nothing to do
        }

        /// <inheritdoc />
        protected sealed override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T? value)
        {
            if (value is null || ValidationFactotum.IsDefaultImmutableArray(value))
            {
                AddError(memberContext, FailureMessage);
            }
        }
    }

    /// <summary>
    ///     Specifies that the annotated member of a reference type should not be <see langword="null"/> or an uninitialized <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to validate.
    /// </typeparam>
    public sealed class Ref<T> : Base<T>
        where T : class;

    /// <summary>
    ///     Specifies that the annotated member of a nullable reference type should not be <see langword="null"/> or
    ///     an uninitialized <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to validate.
    /// </typeparam>
    public sealed class NullableRef<T> : Base<T?>
        where T : class;

    /// <summary>
    ///     Specifies that the annotated member of a <see cref="System.Nullable{T}"/> type should not be <see langword="null"/> or
    ///     an uninitialized <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to validate.
    /// </typeparam>
    /// <seealso cref="ImmutableArray{T}.IsDefault"/>
    public sealed class NullableValue<T> : Base<T?>
        where T : struct;
}