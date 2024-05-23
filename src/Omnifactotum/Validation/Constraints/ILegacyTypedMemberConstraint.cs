using System;

namespace Omnifactotum.Validation.Constraints;

internal interface ILegacyTypedMemberConstraint
{
    Type ActualConstraintType { get; }
}