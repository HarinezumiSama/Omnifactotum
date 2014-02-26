using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
        ///     Gets the validation exception based on the validation result.
        ///     If validation succeeded, this method returns <b>null</b>.
        /// </summary>
        /// <param name="errorDescriptionSeparator">
        ///     The string value that is used to separate a list of validation error descriptions.
        /// </param>
        /// <returns>
        ///     An <see cref="ObjectValidationException"/> if validation failed;
        ///     or <b>null</b> if validation succeeded.
        /// </returns>
        [CanBeNull]
        public ObjectValidationException GetException(string errorDescriptionSeparator)
        {
            if (this.IsObjectValid)
            {
                return null;
            }

            var message = this.Errors.Select(GetValidationErrorDescription).Join(errorDescriptionSeparator);
            return new ObjectValidationException(this, message);
        }

        /// <summary>
        ///     Gets the validation exception based on the validation result.
        ///     If validation succeeded, this method returns <b>null</b>.
        /// </summary>
        /// <returns>
        ///     An <see cref="ObjectValidationException"/> if validation failed;
        ///     or <b>null</b> if validation succeeded.
        /// </returns>
        [CanBeNull]
        public ObjectValidationException GetException()
        {
            return GetException(Environment.NewLine);
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

        #region Private Methods

        private static string GetValidationErrorDescription(MemberConstraintValidationError error)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "[{0}] {1}",
                error.Context.Expression,
                error.ErrorMessage);
        }

        #endregion
    }
}