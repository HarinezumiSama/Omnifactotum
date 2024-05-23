using System;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;
using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     <para>
///         Represents the basic strongly-typed implementation of the <see cref="IMemberConstraint"/> interface.
///     </para>
///     <para><b>NOTE to implementers</b>: implementation has to be stateless.</para>
/// </summary>
/// <typeparam name="T">
///     The type of the value to validate.
/// </typeparam>
public abstract class TypedMemberConstraintBase<T> : MemberConstraintBase
{
    private const string StaticMemberInGenericTypeFalseDetectionMessage = "In fact, this field depends on the generic type argument.";

    /// <summary>
    ///     The type of the value that this constraint can validate.
    /// </summary>
    protected static readonly Type ValueType = typeof(T);

    /// <summary>
    ///     The non-nullable type of the value that this constraint can validate.
    ///     It is equal to <see cref="ValueType"/> when <typeparamref name="T"/> is a reference type or non-nullable value type;
    ///     otherwise, it is the underlying type of the nullable value type.
    /// </summary>
    /// <seealso cref="ValueType"/>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = StaticMemberInGenericTypeFalseDetectionMessage)]
    protected static readonly Type NonNullableValueType = Nullable.GetUnderlyingType(ValueType) ?? ValueType;

    /// <summary>
    ///     The full name of the type of the value that this constraint can validate.
    /// </summary>
    /// <seealso cref="ValueType"/>
    /// <seealso cref="OmnifactotumTypeExtensions.GetFullName"/>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = StaticMemberInGenericTypeFalseDetectionMessage)]
    protected static readonly string ValueTypeFullName = ValueType.GetFullName();

    /// <summary>
    ///     The full name of the non-nullable type of the value that this constraint can validate.
    ///     It is equal to <see cref="ValueTypeFullName"/> when <typeparamref name="T"/> is a reference type or non-nullable value type;
    ///     otherwise, it is the full name of the underlying type of the nullable value type.
    /// </summary>
    /// <seealso cref="NonNullableValueType"/>
    /// <seealso cref="OmnifactotumTypeExtensions.GetFullName"/>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = StaticMemberInGenericTypeFalseDetectionMessage)]
    protected static readonly string NonNullableValueTypeFullName = NonNullableValueType.GetFullName();

    /// <summary>
    ///     The qualified name of the type of the value that this constraint can validate.
    /// </summary>
    /// <seealso cref="ValueType"/>
    /// <seealso cref="OmnifactotumTypeExtensions.GetQualifiedName"/>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = StaticMemberInGenericTypeFalseDetectionMessage)]
    protected static readonly string ValueTypeQualifiedName = ValueType.GetQualifiedName();

    /// <summary>
    ///     The qualified name of the non-nullable type of the value that this constraint can validate.
    ///     It is equal to <see cref="ValueTypeQualifiedName"/> when <typeparamref name="T"/> is a reference type or non-nullable value type;
    ///     otherwise, it is the qualified name of the underlying type of the nullable value type.
    /// </summary>
    /// <seealso cref="NonNullableValueType"/>
    /// <seealso cref="OmnifactotumTypeExtensions.GetQualifiedName"/>
    [SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = StaticMemberInGenericTypeFalseDetectionMessage)]
    protected static readonly string NonNullableValueTypeQualifiedName = NonNullableValueType.GetQualifiedName();

    /// <inheritdoc />
    protected sealed override void ValidateValue(
        MemberConstraintValidationContext memberContext,
        object? value)
    {
        var typedValue = CastTo<T>(value);
        ValidateTypedValue(memberContext, typedValue);
    }

    /// <summary>
    ///     Validates the specified strongly-typed value is scope of the specified memberContext.
    /// </summary>
    /// <param name="memberContext">
    ///     The context of the validated member.
    /// </param>
    /// <param name="value">
    ///     The value to validate.
    /// </param>
    protected abstract void ValidateTypedValue([NotNull] MemberConstraintValidationContext memberContext, T value);

    /// <summary>
    ///     Creates a new <see cref="MemberConstraintValidationContext"/> for the member specified
    ///     by the lambda expression.
    /// </summary>
    /// <typeparam name="TMember">
    ///     The type of the member.
    /// </typeparam>
    /// <param name="valueContext">
    ///     The parent memberContext.
    /// </param>
    /// <param name="value">
    ///     The value which member is accessed.
    /// </param>
    /// <param name="memberGetterExpression">
    ///     The member getter lambda expression.
    /// </param>
    /// <returns>
    ///     A new <see cref="MemberConstraintValidationContext"/> for the member specified
    ///     by the lambda expression.
    /// </returns>
    protected MemberConstraintValidationContext CreateMemberContext<TMember>(
        [NotNull] MemberConstraintValidationContext valueContext,
        T value,
        [NotNull] Expression<Func<T, TMember>> memberGetterExpression)
    {
        if (valueContext is null)
        {
            throw new ArgumentNullException(nameof(valueContext));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (memberGetterExpression is null)
        {
            throw new ArgumentNullException(nameof(memberGetterExpression));
        }

        var memberInfo = Factotum.For<T>.GetFieldOrPropertyInfo(memberGetterExpression);

        var memberExpression = Expression.MakeMemberAccess(
            ValidationFactotum.ConvertTypeAuto(valueContext.Expression, value),
            memberInfo);

        var result = new MemberConstraintValidationContext(
            valueContext.ValidatorContext,
            valueContext.Root,
            value,
            memberExpression,
            valueContext.RootParameterExpression);

        return result;
    }

    /// <summary>
    ///     Validates the member specified by the specified lambda expression.
    /// </summary>
    /// <typeparam name="TMember">
    ///     The type of the member.
    /// </typeparam>
    /// <param name="valueContext">
    ///     The memberContext of the value.
    /// </param>
    /// <param name="value">
    ///     The value containing the member.
    /// </param>
    /// <param name="memberGetterExpression">
    ///     The member getter lambda expression.
    /// </param>
    /// <param name="constraintType">
    ///     The type of the constraint.
    /// </param>
    protected void ValidateMember<TMember>(
        [NotNull] MemberConstraintValidationContext valueContext,
        T value,
        [NotNull] Expression<Func<T, TMember>> memberGetterExpression,
        [NotNull] Type constraintType)
    {
        if (valueContext is null)
        {
            throw new ArgumentNullException(nameof(valueContext));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (memberGetterExpression is null)
        {
            throw new ArgumentNullException(nameof(memberGetterExpression));
        }

        if (constraintType is null)
        {
            throw new ArgumentNullException(nameof(constraintType));
        }

        var objectValidatorContext = valueContext.ValidatorContext;
        var memberContext = CreateMemberContext(valueContext, value, memberGetterExpression);
        var constraint = objectValidatorContext.ResolveConstraint(constraintType);
        var memberValue = memberGetterExpression.Compile().Invoke(value);

        constraint.Validate(memberContext, memberValue);

        if (memberValue is null)
        {
            return;
        }

        var memberValidationResult = ObjectValidator.Validate(
            memberValue,
            ObjectValidator.DefaultRootObjectParameterName,
            objectValidatorContext.RecursiveProcessingContext);

        if (memberValidationResult.IsObjectValid)
        {
            return;
        }

        //// ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var error in memberValidationResult.Errors)
        {
            var funcType = Expression.GetFuncType(valueContext.Root.GetType(), typeof(T));

            var lambda = Expression.Lambda(
                funcType,
                Expression.Convert(valueContext.Expression, typeof(T)),
                valueContext.LambdaExpression.Parameters.Single());

            var combinedExpressions = lambda
                .InjectInto(memberGetterExpression)
                .InjectInto(error.Context.LambdaExpression);

            var newContext = new MemberConstraintValidationContext(
                objectValidatorContext,
                valueContext.Root,
                error.Context.Container,
                combinedExpressions.Body,
                valueContext.RootParameterExpression);

            var newError = new MemberConstraintValidationError(
                newContext,
                error.FailedConstraintType,
                error.Details);

            objectValidatorContext.AddError(newError);
        }
    }

    internal void InternalValidateTypedValue(MemberConstraintValidationContext memberContext, T value)
        => ValidateTypedValue(memberContext, value);
}