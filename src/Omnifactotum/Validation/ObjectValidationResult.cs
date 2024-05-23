using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation;

/// <summary>
///     Represents the result of object validation.
/// </summary>
[DebuggerDisplay("{ToDebuggerString(),nq}")]
public sealed class ObjectValidationResult
{
    internal static readonly ObjectValidationResult SuccessfulResult = new(Array.Empty<MemberConstraintValidationError>());

    private readonly Lazy<string?> _failureMessageLazy;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ObjectValidationResult"/> class.
    /// </summary>
    /// <param name="errors">
    ///     The collection of the validation errors found, if any.
    /// </param>
    internal ObjectValidationResult(IReadOnlyCollection<MemberConstraintValidationError> errors)
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        //// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract :: Validation
        if (errors.Any(item => item is null))
        {
            throw new ArgumentException("The collection contains a null element.", nameof(errors));
        }

        _failureMessageLazy = Lazy.Create(CreateFailureMessage, LazyThreadSafetyMode.PublicationOnly);
        Errors = errors.ToArray().AsReadOnly();
    }

    /// <summary>
    ///     Gets a value indicating whether the object checked is valid.
    /// </summary>
    [DebuggerNonUserCode]
    public bool IsObjectValid => Errors.Count == 0;

    /// <summary>
    ///     Gets the collection of the validation errors found. Can be empty.
    /// </summary>
    [NotNull]
    public ReadOnlyCollection<MemberConstraintValidationError> Errors { get; }

    /// <summary>
    ///     Gets the validation failure message if validation failed, or <see langword="null"/> if validation succeeded.
    /// </summary>
    /// <value>
    ///     The validation failure message if validation failed; or <see langword="null"/> if validation succeeded.
    /// </value>
    [Pure]
    [CanBeNull]
    public string? FailureMessage
    {
        [Omnifactotum.Annotations.Pure]
        get => _failureMessageLazy.Value;
    }

    /// <summary>
    ///     Gets the validation exception based on the validation result.
    /// </summary>
    /// <returns>
    ///     An <see cref="ObjectValidationException"/> if validation failed; or <see langword="null"/> if validation succeeded.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [CanBeNull]
    public ObjectValidationException? GetException() => FailureMessage is { } message ? new ObjectValidationException(this, message) : null;

    /// <summary>
    ///     Checks if validation succeeded and if it is not, throws an <see cref="ObjectValidationException"/>.
    /// </summary>
    [DebuggerStepThrough]
    public void EnsureSucceeded()
    {
        var exception = GetException();
        if (exception is not null)
        {
            throw exception;
        }
    }

    /// <summary>
    ///     Returns a string that represents this <see cref="ObjectValidationResult"/>.
    /// </summary>
    /// <returns>
    ///     A string that represents this <see cref="ObjectValidationResult"/>.
    /// </returns>
    public override string ToString() => ToDebuggerString();

    internal string ToDebuggerString() => $"{nameof(IsObjectValid)} = {IsObjectValid}, {nameof(Errors)}.{nameof(Errors.Count)} = {Errors.Count}";

    private string? CreateFailureMessage()
    {
        if (IsObjectValid)
        {
            return null;
        }

        var errorCount = Errors.Count;

        var result = Errors
            .Select((error, index) => AsInvariant($"[{index + 1}/{errorCount}] [{error.Context.Expression}] {error.Details.Text}"))
            .Join(Environment.NewLine);

        return result;
    }
}