﻿using System;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Contains extension methods for <see cref="IMemberConstraint"/>.
/// </summary>
public static class MemberConstraintExtensions
{
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
    /// <param name="failureMessage">
    ///     The message describing the validation error, or <see langword="null"/> to use a default message.
    /// </param>
    public static void AddError(
        [NotNull] this IMemberConstraint constraint,
        [NotNull] MemberConstraintValidationContext memberContext,
        [CanBeNull] string? failureMessage)
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

        var resolvedFailureMessage = failureMessage.IsNullOrWhiteSpace()
            ? AsInvariant($"Validation of the constraint {failedConstraintType.GetFullName().ToUIString()} failed.")
            : failureMessage;

        var error = new MemberConstraintValidationError(memberContext, failedConstraintType, resolvedFailureMessage);
        memberContext.ValidatorContext.AddError(error);
    }
}