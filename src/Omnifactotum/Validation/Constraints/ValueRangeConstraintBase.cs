using System;
using static Omnifactotum.FormattableStringFactotum;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Represents a base constraint specifying that the annotated member of the type <typeparamref name="T"/> should be within the specified range.
/// </summary>
public abstract class ValueRangeConstraintBase<T> : TypedMemberConstraintBase<T>
    where T : IComparable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValueRangeConstraintBase{T}" /> class.
    /// </summary>
    /// <param name="range">
    ///     The range of valid values.
    /// </param>
    protected ValueRangeConstraintBase(ValueRange<T> range) => Range = range;

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
    protected virtual string FormatRange() => AsInvariant($"[ {Range.Lower} ~ {Range.Upper} ]");

    /// <inheritdoc />
    protected sealed override void ValidateTypedValue(
        ObjectValidatorContext validatorContext,
        MemberConstraintValidationContext memberContext,
        T value)
    {
        if (Range.Contains(value))
        {
            return;
        }

        AddError(validatorContext, memberContext, $"The value must be within the range {FormatRange()}.");
    }
}