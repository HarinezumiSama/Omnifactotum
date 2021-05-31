using System;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

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
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException(
                    @"The value can be neither empty nor whitespace-only string nor null.",
                    nameof(errorMessage));
            }

            Context = context ?? throw new ArgumentNullException(nameof(context));
            FailedConstraintType = failedConstraintType ?? throw new ArgumentNullException(nameof(failedConstraintType));
            ErrorMessage = errorMessage;
        }

        /// <summary>
        ///     Gets the context of validation.
        /// </summary>
        public MemberConstraintValidationContext Context { get; }

        /// <summary>
        ///     Gets the type of the constraint that the value failed against.
        /// </summary>
        public Type FailedConstraintType { get; }

        /// <summary>
        ///     Gets the error message.
        /// </summary>
        public string ErrorMessage { get; }

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
            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            return AsInvariant($@"[{error.Context.Expression}] {error.ErrorMessage}");
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents
        ///     this <see cref="MemberConstraintValidationError"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this <see cref="MemberConstraintValidationError"/>.
        /// </returns>
        public override string ToString()
            => AsInvariant(
                $@"{{{GetType().GetQualifiedName()}: Failed {FailedConstraintType.GetQualifiedName().ToUIString()} for [{
                    Context.Expression}]}}");

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