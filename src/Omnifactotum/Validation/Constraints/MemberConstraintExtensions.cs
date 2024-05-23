using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable AnnotationRedundancyInHierarchy
//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Contains extension methods for <see cref="IMemberConstraint"/>.
/// </summary>
public static class MemberConstraintExtensions
{
    private static readonly Dictionary<Type, ValidationErrorDetails> ConstraintTypeToDefaultErrorDetailsMap = new();

    /// <summary>
    ///     Creates a new <see cref="MemberConstraintValidationError"/> instance using the specified member context
    ///     and failure message and then adds the created error to the validator context.
    /// </summary>
    /// <param name="constraint">
    ///     The failed <see cref="IMemberConstraint"/> to add an error to the validator context for.
    /// </param>
    /// <param name="memberContext">
    ///     The context of the validated member to create an error for.
    /// </param>
    /// <param name="details">
    ///     The validation error details, or <see langword="null"/> to use the default error details.
    /// </param>
    public static void AddError(
        [NotNull] this IMemberConstraint constraint,
        [NotNull] MemberConstraintValidationContext memberContext,
        [CanBeNull] ValidationErrorDetails? details)
    {
        if (constraint is null)
        {
            throw new ArgumentNullException(nameof(constraint));
        }

        if (memberContext is null)
        {
            throw new ArgumentNullException(nameof(memberContext));
        }

        var failedConstraintType = constraint.GetType();
        var resolvedErrorText = details ?? GetDefaultErrorDetails(failedConstraintType);

        var error = new MemberConstraintValidationError(memberContext, failedConstraintType, resolvedErrorText);
        memberContext.ValidatorContext.AddError(error);
    }

    private static ValidationErrorDetails GetDefaultErrorDetails(this Type constraintType)
    {
        lock (ConstraintTypeToDefaultErrorDetailsMap)
        {
            return ConstraintTypeToDefaultErrorDetailsMap.GetOrCreateValue(constraintType, CreateValue);
        }

        static ValidationErrorDetails CreateValue(Type type) => AsInvariant($"Validation of the constraint {type.GetFullName().ToUIString()} failed.");
    }
}