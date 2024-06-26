﻿using System;
using System.Runtime.CompilerServices;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents a base constraint specifying that the annotated member of the nullable type <typeparamref name="T"/>
///     may be <see langword="null"/>, but otherwise should be within the specified range.
/// </summary>
public abstract class OptionalValueRangeConstraintBase<T> : TypedMemberConstraintBase<T?>
    where T : struct, IComparable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OptionalValueRangeConstraintBase{T}" /> class.
    /// </summary>
    /// <param name="range">
    ///     The range of valid values.
    /// </param>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    protected OptionalValueRangeConstraintBase(ValueRange<T> range) => Range = range;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OptionalValueRangeConstraintBase{T}" /> class.
    /// </summary>
    /// <param name="lower">
    ///     The lower boundary of the valid range.
    /// </param>
    /// <param name="upper">
    ///     The upper boundary of the valid range.
    /// </param>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    protected OptionalValueRangeConstraintBase(T lower, T upper)
        : this(ValueRange.Create(lower, upper))
    {
        // Nothing to do
    }

    /// <summary>
    ///     Gets the range of valid values.
    /// </summary>
    protected ValueRange<T> Range { get; }

    /// <summary>
    ///     Formats <see cref="Range"/> as a string to use in the validation error message as needed.
    /// </summary>
    /// <returns>
    ///     The formatted <see cref="Range"/>.
    /// </returns>
    protected virtual string FormatRange() => AsInvariant($"[{FormatValue(Range.Lower)}{ValueRange.DefaultBoundarySeparator}{FormatValue(Range.Upper)}]");

    /// <inheritdoc />
    protected sealed override void ValidateTypedValue(MemberConstraintValidationContext memberContext, T? value)
    {
        if (value is not { } innerValue)
        {
            return;
        }

        if (Range.Contains(innerValue))
        {
            return;
        }

        AddError(memberContext, $"The value {FormatValue(value)} is not within the valid range {FormatRange()}.");
    }
}