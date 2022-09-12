using System;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;

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
    /// <inheritdoc />
    protected sealed override void ValidateValue(
        ObjectValidatorContext validatorContext,
        MemberConstraintValidationContext memberContext,
        object? value)
    {
        var typedValue = CastTo<T>(value);
        ValidateTypedValue(validatorContext, memberContext, typedValue);
    }

    /// <summary>
    ///     Validates the specified strongly-typed value is scope of the specified memberContext.
    /// </summary>
    /// <param name="validatorContext">
    ///     The context of the <see cref="ObjectValidator"/>.
    /// </param>
    /// <param name="memberContext">
    ///     The context of the validated member.
    /// </param>
    /// <param name="value">
    ///     The value to validate.
    /// </param>
    protected abstract void ValidateTypedValue(
        [NotNull] ObjectValidatorContext validatorContext,
        [NotNull] MemberConstraintValidationContext memberContext,
        T value);

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
    /// <param name="validatorContext">
    ///     The object validator memberContext.
    /// </param>
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
        [NotNull] ObjectValidatorContext validatorContext,
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

        var memberContext = CreateMemberContext(valueContext, value, memberGetterExpression);
        var constraint = validatorContext.ResolveConstraint(constraintType);
        var memberValue = memberGetterExpression.Compile().Invoke(value);

        constraint.Validate(validatorContext, memberContext, memberValue);

        if (memberValue is null)
        {
            return;
        }

        var memberValidationResult = ObjectValidator.Validate(memberValue, validatorContext.RecursiveProcessingContext);
        if (memberValidationResult.IsObjectValid)
        {
            return;
        }

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
                valueContext.Root,
                error.Context.Container,
                combinedExpressions.Body,
                valueContext.RootParameterExpression);

            var newError = new MemberConstraintValidationError(
                newContext,
                error.FailedConstraintType,
                error.ErrorMessage);

            validatorContext.Errors.Add(newError);
        }
    }
}