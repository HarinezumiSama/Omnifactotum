using System;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the member data.
    /// </summary>
    internal sealed class MemberData
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberData"/> class.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="container">
        ///     The object containing the value that is being validated. Can be <c>null</c>.
        /// </param>
        /// <param name="value">
        ///     The member value.
        /// </param>
        /// <param name="attributes">
        ///     The constraint attributes.
        /// </param>
        /// <param name="effectiveAttributes">
        ///     The effective constraint attributes.
        /// </param>
        internal MemberData(
            [NotNull] Expression expression,
            [CanBeNull] object container,
            [CanBeNull] object value,
            [CanBeNull] BaseValidatableMemberAttribute[] attributes,
            [CanBeNull] BaseMemberConstraintAttribute[] effectiveAttributes)
        {
            #region Argument Check

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            #endregion

            this.Expression = expression;
            this.Container = container;
            this.Value = value;
            this.Attributes = attributes;
            this.EffectiveAttributes = effectiveAttributes;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the expression.
        /// </summary>
        public Expression Expression
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the object containing the value that is being, or was, validated.
        /// </summary>
        public object Container
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        public object Value
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the constraint attributes.
        /// </summary>
        public BaseValidatableMemberAttribute[] Attributes
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the effective constraint attributes.
        /// </summary>
        public BaseMemberConstraintAttribute[] EffectiveAttributes
        {
            get;
            private set;
        }

        #endregion
    }
}