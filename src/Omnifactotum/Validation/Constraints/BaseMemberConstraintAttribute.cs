using System;
using System.Globalization;
using System.Linq;

namespace Omnifactotum.Validation.Constraints
{
    // As per my understanding, this warning does not make sense for an abstract attribute class ->
    // -> and therefore it can be turned off here
#pragma warning disable 3015

    /// <summary>
    ///     The base class for member constraint attributes.
    /// </summary>
    public abstract class BaseMemberConstraintAttribute : BaseValidatableMemberAttribute
#pragma warning restore 3015
    {
        #region Constants and Fields

        private static readonly Type CompatibleMemberConstraintType = typeof(IMemberConstraint);

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMemberConstraintAttribute"/> class.
        /// </summary>
        /// <param name="constraintType">
        ///     The type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="BaseMemberConstraintAttribute"/> attribute. The type must
        ///     have parameterless constructor.
        /// </param>
        internal BaseMemberConstraintAttribute(Type constraintType)
        {
            #region Argument Check

            if (constraintType == null)
            {
                throw new ArgumentNullException("constraintType");
            }

            if (!CompatibleMemberConstraintType.IsAssignableFrom(constraintType) || constraintType.IsInterface)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    @"The specified type is not a valid constraint type (must implement '{0}').",
                    CompatibleMemberConstraintType.GetFullName());

                throw new ArgumentException(message, "constraintType");
            }

            #endregion

            this.ConstraintType = constraintType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="BaseMemberConstraintAttribute"/> attribute.
        /// </summary>
        public Type ConstraintType
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this <see cref="BaseMemberConstraintAttribute"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this <see cref="BaseMemberConstraintAttribute"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{{0}: {1} = '{2}'}}",
                GetType().GetQualifiedName(),
                Factotum.For<BaseMemberConstraintAttribute>.GetPropertyName(obj => obj.ConstraintType),
                this.ConstraintType.GetQualifiedName());
        }

        #endregion
    }
}