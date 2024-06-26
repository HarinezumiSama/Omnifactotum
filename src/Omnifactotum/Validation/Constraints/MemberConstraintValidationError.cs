﻿using System;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents the member constraint validation error.
/// </summary>
public sealed class MemberConstraintValidationError
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MemberConstraintValidationError"/> class.
    /// </summary>
    /// <param name="context">
    ///     The context of validation.
    /// </param>
    /// <param name="failedConstraintType">
    ///     The type of the constraint that the value failed against.
    /// </param>
    /// <param name="details">
    ///     The validation error details.
    /// </param>
    internal MemberConstraintValidationError(
        [NotNull] MemberConstraintValidationContext context,
        [NotNull] Type failedConstraintType,
        [NotNull] ValidationErrorDetails details)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        FailedConstraintType = failedConstraintType.ValidateConstraintType();
        Details = details ?? throw new ArgumentNullException(nameof(details));
    }

    /// <summary>
    ///     Gets the context of validation.
    /// </summary>
    [NotNull]
    public MemberConstraintValidationContext Context { get; }

    /// <summary>
    ///     Gets the type of the constraint that the value failed against.
    /// </summary>
    [NotNull]
    public Type FailedConstraintType { get; }

    /// <summary>
    ///     Gets the error message.
    /// </summary>
    [NotNull]
    [Obsolete($"Use the '{nameof(Details)}.{nameof(ValidationErrorDetails.Text)}' property instead.")]
    public string ErrorMessage => Details.Text;

    /// <summary>
    ///     Gets the error text.
    /// </summary>
    [NotNull]
    public ValidationErrorDetails Details { get; }

    /// <summary>
    ///     Returns a <see cref="System.String" /> that represents
    ///     this <see cref="MemberConstraintValidationError"/>.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.String" /> that represents this <see cref="MemberConstraintValidationError"/>.
    /// </returns>
    public override string ToString()
        => AsInvariant($"{{ {GetType().GetQualifiedName()}: {FailedConstraintType.GetQualifiedName().ToUIString()} failed for [{Context.Expression}] }}");
}