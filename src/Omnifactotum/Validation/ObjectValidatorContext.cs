﻿using System;
using System.Collections.Generic;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the context of the <see cref="ObjectValidator"/>.
    /// </summary>
    public sealed class ObjectValidatorContext
    {
        private readonly Dictionary<Type, IMemberConstraint> _constraintCache;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectValidatorContext"/> class.
        /// </summary>
        /// <param name="recursiveProcessingContext">
        ///     The context of the recursive processing, or <c>null</c> to use a new context.
        /// </param>
        internal ObjectValidatorContext([CanBeNull] RecursiveProcessingContext<MemberData> recursiveProcessingContext)
        {
            var actualRecursiveProcessingContext = recursiveProcessingContext
                ?? new RecursiveProcessingContext<MemberData>(InternalMemberDataEqualityComparer.Instance);

            _constraintCache = new Dictionary<Type, IMemberConstraint>();
            Errors = new ValidationErrorCollection();
            RecursiveProcessingContext = actualRecursiveProcessingContext;
        }

        /// <summary>
        ///     Gets the collection of errors.
        /// </summary>
        public ValidationErrorCollection Errors
        {
            get;
        }

        internal RecursiveProcessingContext<MemberData> RecursiveProcessingContext
        {
            get;
        }

        /// <summary>
        ///     Resolves the constraint with the specified type.
        /// </summary>
        /// <param name="constraintType">
        ///     The type of the constraint to resolve.
        /// </param>
        /// <returns>
        ///     An <see cref="IMemberConstraint"/> instance representing the resolved constraint.
        /// </returns>
        [NotNull]
        public IMemberConstraint ResolveConstraint([NotNull] Type constraintType)
        {
            constraintType.EnsureValidMemberConstraintType();

            lock (_constraintCache)
            {
                var result = _constraintCache.GetOrCreateValue(
                    constraintType,
                    obj => (IMemberConstraint)Activator.CreateInstance(obj));

                return result;
            }
        }

        /// <summary>
        ///     Resolves the constraint with the specified type.
        /// </summary>
        /// <typeparam name="TMemberConstraint">
        ///     The type of the constraint to resolve.
        /// </typeparam>
        /// <returns>
        ///     An <typeparamref name="TMemberConstraint"/> instance representing the resolved constraint.
        /// </returns>
        [NotNull]
        public TMemberConstraint ResolveConstraint<TMemberConstraint>()
            where TMemberConstraint : IMemberConstraint
        {
            return (TMemberConstraint)ResolveConstraint(typeof(TMemberConstraint));
        }
    }
}