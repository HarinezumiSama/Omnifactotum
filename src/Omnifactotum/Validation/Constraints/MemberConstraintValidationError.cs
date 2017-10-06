using System;
using System.Globalization;
using Omnifactotum.Annotations;

namespace Omnifactotum.Validation.Constraints
{
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
        /// <param name="errorMessage">
        ///     The error message.
        /// </param>
        internal MemberConstraintValidationError(
            MemberConstraintValidationContext context,
            Type failedConstraintType,
            string errorMessage)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (failedConstraintType == null)
            {
                throw new ArgumentNullException("failedConstraintType");
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException(
                    @"The value can be neither empty nor whitespace-only string nor null.",
                    "errorMessage");
            }

            Context = context;
            FailedConstraintType = failedConstraintType;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Gets the context of validation.
        /// </summary>
        public MemberConstraintValidationContext Context
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the type of the constraint that the value failed against.
        /// </summary>
        public Type FailedConstraintType
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the default description of the specified validation error.
        /// </summary>
        /// <param name="error">
        ///     The validation error to get the default description of.
        /// </param>
        /// <returns>
        ///     The default description of the specified validation error.
        /// </returns>
        public static string GetDefaultDescription([NotNull] MemberConstraintValidationError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            return string.Format(
                CultureInfo.InvariantCulture,
                "[{0}] {1}",
                error.Context.Expression,
                error.ErrorMessage);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents
        ///     this <see cref="MemberConstraintValidationError"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this <see cref="MemberConstraintValidationError"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{{0}: Failed '{1}' for [{2}]}}",
                GetType().GetQualifiedName(),
                FailedConstraintType.GetQualifiedName(),
                Context.Expression);
        }

        /// <summary>
        ///     Gets the default description of the current validation error.
        /// </summary>
        /// <returns>
        ///     The default description of the current validation error.
        /// </returns>
        public string GetDefaultDescription()
        {
            return GetDefaultDescription(this);
        }
    }
}