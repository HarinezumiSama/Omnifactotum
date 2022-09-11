using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the result of object validation.
    /// </summary>
    public sealed class ObjectValidationResult
    {
        internal static readonly ObjectValidationResult SuccessfulResult = new(Array.Empty<MemberConstraintValidationError>());

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectValidationResult"/> class.
        /// </summary>
        /// <param name="errors">
        ///     The collection of the validation errors found, if any.
        /// </param>
        internal ObjectValidationResult(ICollection<MemberConstraintValidationError> errors)
        {
            if (errors is null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            //// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract :: Validation
            if (errors.Any(item => item is null))
            {
                throw new ArgumentException(@"The collection contains a null element.", nameof(errors));
            }

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
        ///     <para>Gets the validation exception based on the validation result.</para>
        ///     <para>If validation succeeded, this method returns <see langword="null"/>.</para>
        /// </summary>
        /// <param name="getErrorDescription">
        ///     A reference to a method the retrieves the description for a specified validation error.
        /// </param>
        /// <param name="errorDescriptionSeparator">
        ///     The string value that is used to separate a list of validation error descriptions.
        ///     Can be <see langword="null"/> (in which case an empty string is used).
        /// </param>
        /// <returns>
        ///     An <see cref="ObjectValidationException"/> if validation failed;
        ///     or <see langword="null"/> if validation succeeded.
        /// </returns>
        [CanBeNull]
        public ObjectValidationException? GetException(
            [NotNull] [InstantHandle] Func<MemberConstraintValidationError, string> getErrorDescription,
            [CanBeNull] string? errorDescriptionSeparator)
        {
            if (getErrorDescription is null)
            {
                throw new ArgumentNullException(nameof(getErrorDescription));
            }

            if (IsObjectValid)
            {
                return null;
            }

            var message = Errors.Select(getErrorDescription).Join(errorDescriptionSeparator);
            return new ObjectValidationException(this, message);
        }

        /// <summary>
        ///     <para>
        ///         Gets the validation exception based on the validation result, using the default description
        ///         (<see cref="MemberConstraintValidationError.GetDefaultDescription(Omnifactotum.Validation.Constraints.MemberConstraintValidationError)"/>)
        ///         and <see cref="Environment.NewLine"/> separator.
        ///     </para>
        ///     <para>
        ///         If validation succeeded, this method returns <see langword="null"/>.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     An <see cref="ObjectValidationException"/> if validation failed;
        ///     or <see langword="null"/> if validation succeeded.
        /// </returns>
        [CanBeNull]
        public ObjectValidationException? GetException() => GetException(MemberConstraintValidationError.GetDefaultDescription, Environment.NewLine);

        /// <summary>
        ///     Checks if validation succeeded and if it is not, throws an <see cref="ObjectValidationException"/>.
        /// </summary>
        public void EnsureSucceeded()
        {
            var exception = GetException();
            if (exception is not null)
            {
                throw exception;
            }
        }
    }
}