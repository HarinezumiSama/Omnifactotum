using System;
using System.Linq;
using System.Linq.Expressions;
using Omnifactotum.Annotations;

namespace Omnifactotum.Validation.Constraints
{
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
        #region Protected Methods

        /// <summary>
        ///     Validates the specified value is scope of the specified context.
        /// </summary>
        /// <param name="objectValidatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="context">
        ///     The context of validation.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        /// <returns>
        ///     <list type="bullet">
        ///         <item><b>null</b> or an empty array, if validation succeeded;</item>
        ///         <item>
        ///             or an array of <see cref="MemberConstraintValidationError"/> instances describing
        ///             validation errors, if validation failed.
        ///         </item>
        ///     </list>
        /// </returns>
        protected override sealed MemberConstraintValidationError[] ValidateValue(
            ObjectValidatorContext objectValidatorContext,
            MemberConstraintValidationContext context,
            object value)
        {
            var typedValue = CastTo<T>(value);
            return ValidateTypedValue(objectValidatorContext, context, typedValue);
        }

        /// <summary>
        ///     Validates the specified strongly-typed value is scope of the specified context.
        /// </summary>
        /// <param name="objectValidatorContext">
        ///     The context of the <see cref="ObjectValidator"/>.
        /// </param>
        /// <param name="context">
        ///     The context of validation.
        /// </param>
        /// <param name="value">
        ///     The value to validate.
        /// </param>
        /// <returns>
        ///     <list type="bullet">
        ///         <item><b>null</b> or an empty array, if validation succeeded;</item>
        ///         <item>
        ///             or an array of <see cref="MemberConstraintValidationError"/> instances describing
        ///             validation errors, if validation failed.
        ///         </item>
        ///     </list>
        /// </returns>
        protected abstract MemberConstraintValidationError[] ValidateTypedValue(
            ObjectValidatorContext objectValidatorContext,
            [NotNull] MemberConstraintValidationContext context,
            T value);

        /// <summary>
        ///     Creates a new <see cref="MemberConstraintValidationContext"/> for the member specified
        ///     by the lambda expression.
        /// </summary>
        /// <typeparam name="TMember">
        ///     The type of the member.
        /// </typeparam>
        /// <param name="parentContext">
        ///     The parent context.
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
            [NotNull] MemberConstraintValidationContext parentContext,
            T value,
            [NotNull] Expression<Func<T, TMember>> memberGetterExpression)
        {
            #region Argument Check

            if (parentContext == null)
            {
                throw new ArgumentNullException("parentContext");
            }

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }

            if (memberGetterExpression == null)
            {
                throw new ArgumentNullException("memberGetterExpression");
            }

            #endregion

            var memberInfo = Factotum.For<T>.GetFieldOrPropertyInfo(memberGetterExpression);

            var memberExpression = Expression.MakeMemberAccess(
                ValidationFactotum.ConvertTypeAuto(parentContext.Expression, value),
                memberInfo);

            var result = new MemberConstraintValidationContext(
                parentContext.Root,
                value,
                memberExpression,
                parentContext.RootParameterExpression);

            return result;
        }

        /// <summary>
        ///     Validates the member specified by the specified lambda expression.
        /// </summary>
        /// <typeparam name="TMember">
        ///     The type of the member.
        /// </typeparam>
        /// <param name="objectValidatorContext">
        ///     The object validator context.
        /// </param>
        /// <param name="valueContext">
        ///     The context of the value.
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
        /// <returns>
        ///     
        /// </returns>
        protected MemberConstraintValidationError[] ValidateMember<TMember>(
            ObjectValidatorContext objectValidatorContext,
            [NotNull] MemberConstraintValidationContext valueContext,
            T value,
            [NotNull] Expression<Func<T, TMember>> memberGetterExpression,
            [NotNull] Type constraintType)
        {
            #region Argument Check

            if (valueContext == null)
            {
                throw new ArgumentNullException("valueContext");
            }

            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException("value");
            }

            if (memberGetterExpression == null)
            {
                throw new ArgumentNullException("memberGetterExpression");
            }

            if (constraintType == null)
            {
                throw new ArgumentNullException("constraintType");
            }

            #endregion

            var memberContext = CreateMemberContext(valueContext, value, memberGetterExpression);
            var constraint = objectValidatorContext.ResolveConstraint(constraintType);
            var memberValue = memberGetterExpression.Compile().Invoke(value);

            var result = constraint.Validate(objectValidatorContext, memberContext, memberValue);
            return result;
        }

        #endregion
    }
}