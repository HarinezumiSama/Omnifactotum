﻿using System;
using System.Collections.Immutable;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member should not be <see langword="null"/> or an uninitialized <see cref="ImmutableArray{T}"/>.
/// </summary>
/// <typeparam name="T">
///     The type of the value to validate.
/// </typeparam>
/// <seealso cref="ImmutableArray{T}.IsDefault"/>
public sealed class NotNullConstraint<T> : TypedMemberConstraintBase<T?>
    where T : class
{
    private static readonly string FailureMessage = $"The '{typeof(T).GetQualifiedName()}' value cannot be null.";

    /// <inheritdoc />
    protected override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T? value)
    {
        if (value is null or ImmutableArray<T> { IsDefault: true })
        {
            AddError(memberContext, FailureMessage);
        }
    }
}