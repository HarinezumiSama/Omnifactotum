using System.Collections.Generic;
using System.Collections.ObjectModel;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

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

        internal ReadOnlyCollection<MemberConstraintValidationError> Items
        {
            get;
        }

        /// <summary>
        ///     Adds the specified error to the collection.
        /// </summary>
        /// <param name="error">
        ///     <para>The error.</para>
        ///     <para>Can be <c>null</c>, in which case it is simply not added to the collection.</para>
        /// </param>
        public void Add([CanBeNull] MemberConstraintValidationError error)
        {
            if (error == null)
            {
                return;
            }

            _innerItems.Add(error);
        }

        /// <summary>
        ///     Adds the specified error to the collection.
        /// </summary>
        /// <param name="errors">
        ///     The collection of errors.
        ///     <para>Can be <c>null</c>, in which case it is simply not added to the collection.</para>
        ///     <para>
        ///         One or more items in the collection can be <c>null</c>, in which case those items are
        ///         simply not added to the collection.
        ///     </para>
        /// </param>
        public void AddRange([CanBeNull] IEnumerable<MemberConstraintValidationError> errors)
        {
            if (errors == null)
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