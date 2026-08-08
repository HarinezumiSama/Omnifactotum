using Omnifactotum.Annotations;
using AllowNullAttribute = System.Diagnostics.CodeAnalysis.AllowNullAttribute;

//// ReSharper disable AnnotationRedundancyInHierarchy
//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableAnnotationInsteadOfAttribute

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents an abstract base class for creating typed member constraints with a validation function
///     that maps to a single <see cref="ValidationErrorDetails"/>.
/// </summary>
/// <typeparam name="T">
///     The type of the value to be validated by the constraint.
/// </typeparam>
public abstract class SimpleTypedMemberConstraintBase<T> : TypedMemberConstraintBase<T>
{
    /// <inheritdoc />
    protected sealed override void ValidateTypedValue([NotNull] MemberConstraintValidationContext memberContext, [AllowNull] T value)
    {
        if (IsValid(memberContext, value))
        {
            return;
        }

        var validationErrorDetails = CreateValidationErrorDetails(memberContext, value);
        AddError(memberContext, validationErrorDetails);
    }

    /// <summary>
    ///     Determines whether the specified value satisfies the constraint.
    /// </summary>
    /// <param name="memberContext">
    ///     The context of the member being validated.
    /// </param>
    /// <param name="value">
    ///     The value to validate.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the specified value satisfies the constraint; otherwise, <see langword="false" />.
    /// </returns>
    protected abstract bool IsValid([NotNull] MemberConstraintValidationContext memberContext, [AllowNull] T value);

    /// <summary>
    ///     Creates an instance of <see cref="ValidationErrorDetails"/> based on the specified value.
    /// </summary>
    /// <param name="memberContext">
    ///     The context of the member being validated.
    /// </param>
    /// <param name="value">
    ///     The value for which the validation error details are to be created.
    /// </param>
    /// <returns>
    ///     A <see cref="ValidationErrorDetails"/> instance representing the details of the validation error for the specified value.
    /// </returns>
    protected abstract ValidationErrorDetails CreateValidationErrorDetails([NotNull] MemberConstraintValidationContext memberContext, [AllowNull] T value);
}