using System.Collections.Generic;
using System.Collections.ObjectModel;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the collection of validation errors.
    /// </summary>
    public sealed class ValidationErrorCollection
    {
        private readonly List<MemberConstraintValidationError> _innerItems;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValidationErrorCollection"/> class.
        /// </summary>
        internal ValidationErrorCollection()
        {
            _innerItems = new List<MemberConstraintValidationError>();
            Items = new ReadOnlyCollection<MemberConstraintValidationError>(_innerItems);
        }

        internal ReadOnlyCollection<MemberConstraintValidationError> Items { get; }

        /// <summary>
        ///     Adds the specified error to the collection.
        /// </summary>
        /// <param name="error">
        ///     <para>
        ///         The error.
        ///     </para>
        ///     <para>
        ///         Can be <see langword="null"/>, in which case it is simply not added to the collection.
        ///     </para>
        /// </param>
        public void Add([CanBeNull] MemberConstraintValidationError? error)
        {
            if (error is null)
            {
                return;
            }

            _innerItems.Add(error);
        }

        /// <summary>
        ///     Adds the specified collection of errors to this <see cref="ValidationErrorCollection"/>.
        /// </summary>
        /// <param name="errors">
        ///     <para>
        ///         The collection of errors.
        ///     </para>
        ///     <para>
        ///         Can be <see langword="null"/>, in which case nothing is added to this <see cref="ValidationErrorCollection"/>.
        ///     </para>
        ///     <para>
        ///         One or more items in the collection can be <see langword="null"/>, in which case these <see langword="null"/> items
        ///         are not added to this <see cref="ValidationErrorCollection"/>.
        ///     </para>
        /// </param>
        public void AddRange([CanBeNull] IEnumerable<MemberConstraintValidationError?>? errors)
        {
            if (errors is null)
            {
                return;
            }

            foreach (var error in errors)
            {
                Add(error);
            }
        }
    }
}