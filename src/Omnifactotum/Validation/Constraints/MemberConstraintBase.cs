using System;
using System.Globalization;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     <para>Represents the basic implementation of the <see cref="IMemberConstraint"/> interface.</para>
///     <para><b>NOTE to implementers</b>: implementation has to be stateless.</para>
/// </summary>
public abstract class MemberConstraintBase : IMemberConstraint
{
    /// <inheritdoc />
    public void Validate(MemberConstraintValidationContext memberContext, object? value)
    {
        if (memberContext is null)
        {
            throw new ArgumentNullException(nameof(memberContext));
        }

        ValidateValue(memberContext, value);
    }

    /// <summary>
    ///     Formats the specified value as a string.
    /// </summary>
    /// <param name="value">
    ///     The value to format.
    /// </param>
    /// <typeparam name="TValue">
    ///     The type of value to format.
    /// </typeparam>
    /// <returns>
    ///     The specified value formatted as a string.
    /// </returns>
    protected static string FormatValue<TValue>(TValue value)
        => ValidationFactotum.TryFormatSimpleValue(value)
            ?? value switch
            {
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => (value?.ToString()).AvoidNull()
            };

    /// <summary>
    ///     Validates the specified value is scope of the specified memberContext.
    /// </summary>
    /// <param name="memberContext">
    ///     The context of the validated member.
    /// </param>
    /// <param name="value">
    ///     The value to validate.
    /// </param>
    protected abstract void ValidateValue([NotNull] MemberConstraintValidationContext memberContext, [CanBeNull] object? value);

    /// <summary>
    ///     Tries to cast the specified value to the specified target type and
    ///     if the value is not compatible with the target type, throws an exception with the detailed description.
    /// </summary>
    /// <typeparam name="TTarget">
    ///     The type to cast to.
    /// </typeparam>
    /// <param name="value">
    ///     The value to cast.
    /// </param>
    /// <returns>
    ///     The value cast to the specified target type.
    /// </returns>
    protected TTarget CastTo<TTarget>([CanBeNull] object? value)
    {
        var targetType = typeof(TTarget);

        return value switch
        {
            TTarget target => target,

            null => !targetType.IsValueType || targetType.IsNullableValueType()
                ? (TTarget)value!
                : throw new InvalidOperationException(
                    AsInvariant(
                        $@"The null value is not compatible with the type {targetType.GetFullName().ToUIString()} expected by the constraint {
                            GetType().GetQualifiedName().ToUIString()}.")),

            _ => throw new InvalidOperationException(
                AsInvariant(
                    $@"The type of the value {value.GetTypeSafely().GetFullName().ToUIString()} is not compatible with the type {
                        targetType.GetFullName().ToUIString()} expected by the constraint {GetType().GetQualifiedName().ToUIString()}."))
        };
    }

    /// <summary>
    ///     Creates a new <see cref="MemberConstraintValidationError"/> instance using the specified member context
    ///     and failure message and then adds the created error to the validator context.
    /// </summary>
    /// <param name="memberContext">
    ///     The context of the validated member to create an error for.
    /// </param>
    /// <param name="failureMessage">
    ///     The message describing the validation error, or <see langword="null"/> to use a default message.
    /// </param>
    protected void AddError([NotNull] MemberConstraintValidationContext memberContext, [CanBeNull] string? failureMessage)
    {
        //// ReSharper disable once InvokeAsExtensionMethod :: JIC, to avoid ambiguity between instance and extension method
        MemberConstraintExtensions.AddError(this, memberContext, failureMessage);
    }

    /// <summary>
    ///     Creates a new <see cref="MemberConstraintValidationError"/> instance using the specified member context
    ///     and failure message and then adds the created error to the validator context.
    /// </summary>
    /// <param name="validatorContext">
    ///     The context of the <see cref="ObjectValidator"/>.
    /// </param>
    /// <param name="memberContext">
    ///     The context of the validated member to create an error for.
    /// </param>
    /// <param name="failureMessage">
    ///     The message describing the validation error.
    /// </param>
    [Obsolete($"Use `{nameof(AddError)}({nameof(MemberConstraintValidationContext)}, {nameof(String)}?)` instead.")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected void AddError(
        [NotNull] ObjectValidatorContext validatorContext,
        [NotNull] MemberConstraintValidationContext memberContext,
        [NotNull] string failureMessage)
    {
        //// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        Factotum.Assert(validatorContext is not null && ReferenceEquals(validatorContext, memberContext.ValidatorContext));

        AddError(memberContext, failureMessage);
    }

    /// <summary>
    ///     Creates a new <see cref="MemberConstraintValidationError"/> instance using the specified member context
    ///     and default failure message and then adds the created error to the validator context.
    /// </summary>
    /// <param name="validatorContext">
    ///     The context of the <see cref="ObjectValidator"/>.
    /// </param>
    /// <param name="memberContext">
    ///     The context of the validated member to create an error for.
    /// </param>
    [Obsolete($"Use `{nameof(AddError)}({nameof(MemberConstraintValidationContext)}, {nameof(String)}?)` instead.")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    protected void AddDefaultError(
        [NotNull] ObjectValidatorContext validatorContext,
        [NotNull] MemberConstraintValidationContext memberContext)
    {
        //// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        Factotum.Assert(validatorContext is not null && ReferenceEquals(validatorContext, memberContext.ValidatorContext));

        AddError(memberContext, null);
    }
}