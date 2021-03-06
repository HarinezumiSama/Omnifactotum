﻿using System;
using System.Linq.Expressions;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the member data.
    /// </summary>
    internal sealed class MemberData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberData"/> class.
        /// </summary>
        /// <param name="expression">
        ///     The expression.
        /// </param>
        /// <param name="container">
        ///     The object containing the value that is being validated. Can be <c>null</c>.
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
            [NotNull] Expression expression,
            [CanBeNull] object container,
            [CanBeNull] object value,
            [CanBeNull] BaseValidatableMemberAttribute[] attributes,
            [CanBeNull] BaseMemberConstraintAttribute[] effectiveAttributes)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));

            Container = container;
            Value = value;
            Attributes = attributes;
            EffectiveAttributes = effectiveAttributes;
        }

        /// <summary>
        ///     Gets the expression.
        /// </summary>
        public Expression Expression
        {
            get;
        }

        /// <summary>
        ///     Gets the object containing the value that is being, or was, validated.
        /// </summary>
        public object Container
        {
            get;
        }

        /// <summary>
        ///     Gets the value.
        /// </summary>
        public object Value
        {
            get;
        }

        /// <summary>
        ///     Gets the constraint attributes.
        /// </summary>
        public BaseValidatableMemberAttribute[] Attributes
        {
            get;
        }

        /// <summary>
        ///     Gets the effective constraint attributes.
        /// </summary>
        public BaseMemberConstraintAttribute[] EffectiveAttributes
        {
            get;
        }
    }
}