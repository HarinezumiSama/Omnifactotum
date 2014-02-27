using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the result of object validation.
    /// </summary>
    public sealed class ObjectValidationResult
    {
        #region Constants and Fields

        internal static readonly ObjectValidationResult SuccessfulResult =
            new ObjectValidationResult(new MemberConstraintValidationError[0]);

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectValidationResult"/> class.
        /// </summary>
        /// <param name="errors">
        ///     The collection of the validation errors found, if any.
        /// </param>
        internal ObjectValidationResult(ICollection<MemberConstraintValidationError> errors)
        {
            #region Argument Check

            if (errors == null)
            {
                throw new ArgumentNullException("errors");
            }

            if (errors.Any(item => item == null))
            {
                throw new ArgumentException(@"The collection contains a null element.", "errors");
            }

            #endregion

            this.Errors = errors.ToArray().AsReadOnly();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the object checked is valid.
        /// </summary>
        public bool IsObjectValid
        {
            [DebuggerNonUserCode]
            get
            {
                return this.Errors.Count == 0;
            }
        }

        /// <summary>
        ///     Gets the collection of the validation errors found. Can be empty.
        /// </summary>
        public ReadOnlyCollection<MemberConstraintValidationError> Errors
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     <para>Gets the validation exception based on the validation result.</para>
        ///     <para>If validation succeeded, this method returns <b>null</b>.</para>
        /// </summary>
        /// <param name="getErrorDescription">
        ///     A reference to a method the retrieves the description for a specified validation error.
        /// </param>
        /// <param name="errorDescriptionSeparator">
        ///     The string value that is used to separate a list of validation error descriptions.
        ///     Can be <b>null</b> (in which case an empty string is used).
        /// </param>
        /// <returns>
        ///     An <see cref="ObjectValidationException"/> if validation failed;
        ///     or <b>null</b> if validation succeeded.
        /// </returns>
        [CanBeNull]
        public ObjectValidationException GetException(
            Func<MemberConstraintValidationError, string> getErrorDescription,
            string errorDescriptionSeparator)
        {
            #region Argument Check

            if (getErrorDescription == null)
            {
                throw new ArgumentNullException("getErrorDescription");
            }

            #endregion

            if (this.IsObjectValid)
            {
                return null;
            }

            var message = this.Errors.Select(getErrorDescription).Join(errorDescriptionSeparator);
            return new ObjectValidationException(this, message);
        }

        /// <summary>
        ///     <para>
        ///         Gets the validation exception based on the validation result, using the default description
        ///         (<see cref="MemberConstraintValidationError.GetDefaultDescription()"/>) and
        ///         <see cref="Environment.NewLine"/> separator.
        ///     </para>
        ///     <para>
        ///         If validation succeeded, this method returns <b>null</b>.
        ///     </para>
        /// </summary>
        /// <returns>
        ///     An <see cref="ObjectValidationException"/> if validation failed;
        ///     or <b>null</b> if validation succeeded.
        /// </returns>
        [CanBeNull]
        public ObjectValidationException GetException()
        {
            return GetException(MemberConstraintValidationError.GetDefaultDescription, Environment.NewLine);
        }

        /// <summary>
        ///     Checks if validation succeeded and if it is not, throws an <see cref="ObjectValidationException"/>.
        /// </summary>
        public void EnsureSucceeded()
        {
            var exception = GetException();
            if (exception == null)
            {
                return;
            }

            throw exception;
        }

        #endregion
    }
}