﻿using System;

//// ReSharper disable RedundantAttributeUsageProperty

namespace Omnifactotum.Validation.Annotations;

/// <summary>
///     Specifies that the content of annotated member is validated.
///     Used for members that do not have constraints applied to them.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
[CLSCompliant(false)]
public sealed class ValidatableMemberAttribute : BaseValidatableMemberAttribute
{
    // No members
}