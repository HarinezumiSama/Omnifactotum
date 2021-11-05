using System;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

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
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseMemberConstraintAttribute"/> class.
        /// </summary>
        /// <param name="constraintType">
        ///     The type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="BaseMemberConstraintAttribute"/> attribute. The type must
        ///     have parameterless constructor.
        /// </param>
        internal BaseMemberConstraintAttribute([NotNull] Type constraintType)
            => ConstraintType = constraintType.EnsureValidMemberConstraintType();

        /// <summary>
        ///     Gets the type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="BaseMemberConstraintAttribute"/> attribute.
        /// </summary>
        public Type ConstraintType { get; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this <see cref="BaseMemberConstraintAttribute"/>.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this <see cref="BaseMemberConstraintAttribute"/>.
        /// </returns>
        public override string ToString()
            => AsInvariant(
                $@"{{{GetType().GetQualifiedName()}: {
                    Factotum.For<BaseMemberConstraintAttribute>.GetPropertyName(obj => obj.ConstraintType)} = {
                        ConstraintType.GetQualifiedName().ToUIString()}}}");
    }
}