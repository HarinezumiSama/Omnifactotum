﻿using System;
using System.Globalization;
using System.Linq;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Specifies how the annotated member is validated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class MemberConstraintAttribute : Attribute
    {
        #region Constants and Fields

        private static readonly Type CompatibleMemberConstraintType = typeof(IMemberConstraint);

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberConstraintAttribute"/> class.
        /// </summary>
        /// <param name="constraintType">
        ///     The type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="MemberConstraintAttribute"/> attribute. The type must have
        ///     parameterless constructor.
        /// </param>
        public MemberConstraintAttribute(Type constraintType)
        {
            #region Argument Check

            if (constraintType == null)
            {
                throw new ArgumentNullException("constraintType");
            }

            if (!CompatibleMemberConstraintType.IsAssignableFrom(constraintType) || constraintType.IsInterface)
            {
                var message = string.Format(
                    CultureInfo.InvariantCulture,
                    @"The specified type is not a valid constraint type (must implement '{0}').",
                    CompatibleMemberConstraintType.GetFullName());

                throw new ArgumentException(message, "constraintType");
            }

            #endregion

            this.ConstraintType = constraintType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the type, implementing the <see cref="IMemberConstraint"/> interface, used to validate
        ///     the member annotated with this <see cref="MemberConstraintAttribute"/> attribute.
        /// </summary>
        public Type ConstraintType
        {
            get;
            private set;
        }

        #endregion
    }
}