using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the collection of validation errors.
    /// </summary>
    public sealed class ValidationErrorCollection
    {
        #region Constants and Fields

        private readonly List<MemberConstraintValidationError> _innerItems;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValidationErrorCollection"/> class.
        /// </summary>
        internal ValidationErrorCollection()
        {
            _innerItems = new List<MemberConstraintValidationError>();
            this.Items = new ReadOnlyCollection<MemberConstraintValidationError>(_innerItems);
        }

        #endregion

        #region Internal Properties

        internal ReadOnlyCollection<MemberConstraintValidationError> Items
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds the specified error to the collection.
        /// </summary>
        /// <param name="error">
        ///     <para>The error.</para>
        ///     <para>Can be <b>null</b>, in which case it is simply not added to the collection.</para>
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
        ///     <para>Can be <b>null</b>, in which case it is simply not added to the collection.</para>
        ///     <para>
        ///         One or more items in the collection can be <b>null</b>, in which case those items are
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

        #endregion
    }
}