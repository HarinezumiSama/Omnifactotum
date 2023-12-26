using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     <para>
///         Represents a constraint for a type member (that is, for a field or a property).
///     </para>
///     <para>
///         <b>NOTES</b>:
///         <list type="bullet">
///             <item>
///                 Implementation must be stateless because the instance is reused for different validation calls.
///             </item>
///             <item>
///                 Implementation must have a parameterless constructor.
///             </item>
///         </list>
///     </para>
/// </summary>
public interface IMemberConstraint
{
    /// <summary>
    ///     Validates the specified value in scope of the specified context.
    /// </summary>
    /// <param name="validatorContext">
    ///     The context of the <see cref="ObjectValidator"/>.
    /// </param>
    /// <param name="memberContext">
    ///     The context of validation.
    /// </param>
    /// <param name="value">
    ///     The value to validate.
    /// </param>
    void Validate(
        [NotNull] ObjectValidatorContext validatorContext,
        [NotNull] MemberConstraintValidationContext memberContext,
        [CanBeNull] object? value);
}