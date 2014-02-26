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
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        public ObjectValidationException(string message)
            : base(message)
        {
            // Nothing to do
        }

        private ObjectValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Nothing to do
        }

        #endregion
    }
}