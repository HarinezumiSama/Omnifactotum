using System;
using System.Runtime.Serialization;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the object validation exception.
    /// </summary>
    [Serializable]
    public sealed class ObjectValidationException : Exception
    {
        private readonly ObjectValidationResult? _validationResult;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectValidationException"/> class.
        /// </summary>
        /// <param name="validationResult">
        ///     The validation result caused this exception.
        /// </param>
        /// <param name="message">
        ///     The message that describes the error.
        /// </param>
        internal ObjectValidationException([NotNull] ObjectValidationResult validationResult, [NotNull] string message)
            : base(message)
            => _validationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));

        private ObjectValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Nothing to do
        }

        /// <summary>
        ///     Gets the validation result caused this exception.
        /// </summary>
        [NotNull]
        public ObjectValidationResult ValidationResult => _validationResult.EnsureNotNull();
    }
}