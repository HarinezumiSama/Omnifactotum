using System;
using Omnifactotum.Annotations;
using NotNullIfNotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute;

//// ReSharper disable AnnotationRedundancyInHierarchy
//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

namespace Omnifactotum.Validation;

/// <summary>
///     A container of the validation error details.
/// </summary>
public sealed class ValidationErrorDetails
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValidationErrorDetails"/> class.
    /// </summary>
    /// <param name="text">
    ///     The validation error text.
    /// </param>
    /// <param name="description">
    ///     The validation error description.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     <paramref name="text"/> or <paramref name="description"/> is <see langword="null"/> or a blank string.
    /// </exception>
    public ValidationErrorDetails([NotNull] string text, [NotNull] string description)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("The value cannot be null or a blank string.", nameof(text));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("The value cannot be null or a blank string.", nameof(description));
        }

        Text = text;
        Description = description;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ValidationErrorDetails"/> class.
    /// </summary>
    /// <param name="text">
    ///     The validation error text (see <see cref="Text"/>). The same value is used for <see cref="Description"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     <paramref name="text"/> is <see langword="null"/> or a blank string.
    /// </exception>
    public ValidationErrorDetails([NotNull] string text)
        : this(text, text)
    {
        // Nothing to do
    }

    /// <summary>
    ///     Gets the validation error text (usually, a user-friendly message describing the validation failure reason).
    /// </summary>
    [NotNull]
    public string Text { get; }

    /// <summary>
    ///     Gets the validation error description (may provide more internal details about the validation failure).
    /// </summary>
    [NotNull]
    public string Description { get; }

    /// <summary>
    ///     Implicitly converts the specified validation error text to an instance of <see cref="ValidationErrorDetails"/> class.
    /// </summary>
    /// <param name="text">
    ///     The validation error text (can be <see langword="null"/>).
    /// </param>
    /// <returns>
    ///     An instance of <see cref="ValidationErrorDetails"/> class if the <paramref name="text"/> is not <see langword="null"/>;
    ///     otherwise, <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(text))]
    public static implicit operator ValidationErrorDetails?([CanBeNull] string? text) => text is null ? null : new ValidationErrorDetails(text);
}