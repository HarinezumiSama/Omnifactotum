using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation;

/// <summary>
///     Represents the context of the <see cref="ObjectValidator"/>.
/// </summary>
[DebuggerDisplay("{ToDebuggerString(),nq}")]
public sealed class ObjectValidatorContext
{
    private readonly Dictionary<Type, IMemberConstraint> _constraintCache;
    private readonly List<MemberConstraintValidationError> _innerErrors;
    private readonly bool _ownsRecursiveProcessingContext;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ObjectValidatorContext"/> class.
    /// </summary>
    /// <param name="recursiveProcessingContext">
    ///     The context of the recursive processing, or <see langword="null"/> to use a new context.
    /// </param>
    internal ObjectValidatorContext(RecursiveProcessingContext<MemberData>? recursiveProcessingContext)
    {
        var actualRecursiveProcessingContext = recursiveProcessingContext
            ?? Omnifactotum.RecursiveProcessingContext.Create(InternalMemberDataEqualityComparer.Instance);

        _constraintCache = new Dictionary<Type, IMemberConstraint>();
        _innerErrors = new List<MemberConstraintValidationError>();
        _ownsRecursiveProcessingContext = recursiveProcessingContext is null;

        Errors = _innerErrors.AsReadOnly();
        RecursiveProcessingContext = actualRecursiveProcessingContext;
    }

    /// <summary>
    ///     Gets the collection of validation errors.
    /// </summary>
    [NotNull]
    internal ReadOnlyCollection<MemberConstraintValidationError> Errors { get; }

    [NotNull]
    internal RecursiveProcessingContext<MemberData> RecursiveProcessingContext { get; }

    internal bool IsValidationComplete { get; private set; }

    /// <summary>
    ///     Resolves the constraint with the specified type.
    /// </summary>
    /// <param name="constraintType">
    ///     The type of the constraint to resolve.
    /// </param>
    /// <returns>
    ///     An <see cref="IMemberConstraint"/> instance representing the resolved constraint.
    /// </returns>
    [NotNull]
    public IMemberConstraint ResolveConstraint([NotNull] Type constraintType)
    {
        constraintType.ValidateAndRegisterMemberConstraintType();

        lock (_constraintCache)
        {
            return _constraintCache.GetOrCreateValue(constraintType, ValidationFactotum.CreateMemberConstraint);
        }
    }

    /// <summary>
    ///     Resolves the constraint with the specified type.
    /// </summary>
    /// <typeparam name="TMemberConstraint">
    ///     The type of the constraint to resolve.
    /// </typeparam>
    /// <returns>
    ///     An <typeparamref name="TMemberConstraint"/> instance representing the resolved constraint.
    /// </returns>
    [NotNull]
    public TMemberConstraint ResolveConstraint<TMemberConstraint>()
        where TMemberConstraint : IMemberConstraint
        => (TMemberConstraint)ResolveConstraint(typeof(TMemberConstraint));

    internal string ToDebuggerString()
        => $"{nameof(Errors)} = {{ {nameof(Errors.Count)} = {Errors.Count}, {nameof(IsValidationComplete)} = {IsValidationComplete} }}";

    internal void AddError(MemberConstraintValidationError error)
    {
        if (error is null)
        {
            throw new ArgumentNullException(nameof(error));
        }

        if (IsValidationComplete)
        {
            throw new InvalidOperationException("Object validation is already completed.");
        }

        _innerErrors.Add(error);
    }

    internal void OnCompleteValidation()
    {
        Factotum.Assert(!IsValidationComplete);

        if (_ownsRecursiveProcessingContext)
        {
            RecursiveProcessingContext.ItemsBeingProcessed?.Clear();
        }

        lock (_constraintCache)
        {
            _constraintCache.Clear();
        }

        IsValidationComplete = true;
    }
}