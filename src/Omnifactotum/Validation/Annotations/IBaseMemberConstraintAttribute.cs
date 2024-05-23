using System;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation.Annotations;

internal interface IBaseMemberConstraintAttribute : IBaseValidatableMemberAttribute
{
    /// <summary>
    ///     Gets the type, implementing the <see cref="IMemberConstraint"/> interface, used to validate the member annotated.
    /// </summary>
    Type ConstraintType { get; }
}