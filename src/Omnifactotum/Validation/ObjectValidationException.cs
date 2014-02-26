using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the object validation exception.
    /// </summary>
    [Serializable]
    public sealed class ObjectValidationException : Exception
    {
        #region Constructors

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
            this.ValidationResult = validationResult;
        }

        private ObjectValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Nothing to do
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the validation result caused this exception.
        /// </summary>
        public ObjectValidationResult ValidationResult
        {
            get;
            private set;
        }

        #endregion
    }
}