using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Validation.Annotations;
using Omnifactotum.Validation.Constraints;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation;

/// <summary>
///     Represents the member data.
/// </summary>
[DebuggerDisplay("{ToDebuggerString(),nq}")]
internal sealed class MemberData
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MemberData"/> class.
    /// </summary>
    /// <param name="expression">
    ///     The expression.
    /// </param>
    /// <param name="container">
    ///     The object containing the value that is being validated. Can be <see langword="null"/>.
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
        Expression expression,
        object? container,
        object? value,
        IBaseValidatableMemberAttribute[] attributes,
        IBaseMemberConstraintAttribute[] effectiveAttributes)
    {
        if (effectiveAttributes is null)
        {
            throw new ArgumentNullException(nameof(effectiveAttributes));
        }

        Expression = expression ?? throw new ArgumentNullException(nameof(expression));

        Container = container;
        Value = value;
        ValueType = value?.GetType() ?? expression.Type;
        Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
        EffectiveAttributes = effectiveAttributes.OrderBy(attribute => attribute.ConstraintType.GetFullName()).ToArray();

        ExpressionString = expression.ToStringSafely();
    }

    /// <summary>
    ///     Gets the expression.
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    ///     Gets the object containing the value that is being, or was, validated.
    /// </summary>
    public object? Container { get; }

    /// <summary>
    ///     Gets the value.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    ///     Gets the type of <see cref="Value"/>, or the type of <see cref="Expression"/> if <see cref="Value"/> is <see langword="null"/>.
    /// </summary>
    public Type ValueType { get; }

    /// <summary>
    ///     Gets the constraint attributes.
    /// </summary>
    public IBaseValidatableMemberAttribute[] Attributes { get; }

    /// <summary>
    ///     Gets the effective constraint attributes.
    /// </summary>
    public IBaseMemberConstraintAttribute[] EffectiveAttributes { get; }

    /// <summary>
    ///     Gets the expression string.
    /// </summary>
    internal string ExpressionString { get; }

    private static string FormatValue(object? value)
        => ValidationFactotum.TryFormatSimpleValue(value)
            ?? (value?.GetType() is { IsValueType: true } type ? $"{{ {type.GetQualifiedName()} }}" : $"{{ {value.GetShortObjectReferenceDescription()} }}");

    private string ToDebuggerString()
        => $"{{ {nameof(Expression)} = {ExpressionString}, {nameof(Value)} = {FormatValue(Value)}, {nameof(Container)} = {FormatValue(Container)} }}";
}