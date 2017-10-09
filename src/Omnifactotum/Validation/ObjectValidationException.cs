using System;
using System.Runtime.Serialization;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the object validation exception.
    /// </summary>
    [Serializable]
    public sealed class ObjectValidationException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectValidationException"/> class.
        /// </summary>
        /// <param name="validationResult">
        ///     The validation result caused this exception.
        /// </param>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        internal ObjectValidationException(ObjectValidationResult validationResult, string message)
            : base(message)
        {
            ValidationResult = validationResult;
        }

        private ObjectValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Nothing to do
        }

        /// <summary>
        ///     Gets the validation result caused this exception.
        /// </summary>
        public ObjectValidationResult ValidationResult
        {
            get;
        }
    }
}