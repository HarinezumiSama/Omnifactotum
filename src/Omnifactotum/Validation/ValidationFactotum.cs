﻿using System;
using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the internal helper for the <see cref="ObjectValidator"/>.
    /// </summary>
    internal static class ValidationFactotum
    {
        private static readonly Type CompatibleMemberConstraintType = typeof(IMemberConstraint);

        /// <summary>
        ///     Converts the type of the expression, if needed.
        /// </summary>
        /// <param name="expression">
        ///     The expression to convert.
        /// </param>
        /// <param name="valueType">
        ///     The type of the value.
        /// </param>
        /// <returns>
        ///     An original expression, if conversion was not needed; otherwise, a converted expression.
        /// </returns>
        public static Expression ConvertTypeAuto([NotNull] Expression expression, [NotNull] Type valueType)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (valueType is null)
            {
                throw new ArgumentNullException(nameof(valueType));
            }

            var expressionType = expression.Type.GetCollectionElementType() ?? expression.Type;

            return expressionType == valueType ? expression : Expression.Convert(expression, valueType);
        }

        /// <summary>
        ///     Converts the type of the expression, if needed.
        /// </summary>
        /// <param name="expression">
        ///     The expression to convert.
        /// </param>
        /// <param name="value">
        ///     The value to which type to convert the expression.
        /// </param>
        /// <returns>
        ///     An original expression, if conversion was not needed; otherwise, a converted expression.
        /// </returns>
        public static Expression ConvertTypeAuto([NotNull] Expression expression, [CanBeNull] object? value)
            => value is null ? expression.EnsureNotNull() : ConvertTypeAuto(expression, value.GetType());

        /// <summary>
        ///     Determines whether the specified constraint type is a valid member constraint type.
        /// </summary>
        /// <param name="constraintType">
        ///     The type of the constraint to check.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if specified constraint type is a valid member constraint type; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintType"/> is <see langword="null"/>.
        /// </exception>
        public static bool IsValidMemberConstraintType([NotNull] this Type constraintType)
        {
            if (constraintType is null)
            {
                throw new ArgumentNullException(nameof(constraintType));
            }

            return !constraintType.IsInterface && CompatibleMemberConstraintType.IsAssignableFrom(constraintType);
        }

        /// <summary>
        ///     Ensures that the specified constraint type is a valid member constraint type.
        /// </summary>
        /// <param name="constraintType">
        ///     The type of the constraint to check.
        /// </param>
        /// <returns>
        ///     The input constraint type, if it is a valid member constraint type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraintType"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///     The specified constraint type is not a valid member constraint type.
        /// </exception>
        [NotNull]
        public static Type EnsureValidMemberConstraintType([NotNull] this Type constraintType)
        {
            if (IsValidMemberConstraintType(constraintType))
            {
                return constraintType.EnsureNotNull();
            }

            var message = AsInvariant(
                $@"The specified type {constraintType.GetFullName().ToUIString()} is not a valid constraint type (must implement {
                    CompatibleMemberConstraintType.GetFullName().ToUIString()}).");

            throw new ArgumentException(message, nameof(constraintType));
        }
    }
}