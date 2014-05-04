using System;
using System.Collections.Generic;
using System.Linq;
using Omnifactotum.Annotations;
using Omnifactotum.Validation.Constraints;

namespace Omnifactotum.Validation
{
    /// <summary>
    ///     Represents the context of the <see cref="ObjectValidator"/>.
    /// </summary>
    public sealed class ObjectValidatorContext
    {
        #region Constants and Fields

        private readonly Dictionary<Type, IMemberConstraint> _constraintCache;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObjectValidatorContext"/> class.
        /// </summary>
        internal ObjectValidatorContext()
        {
            _constraintCache = new Dictionary<Type, IMemberConstraint>();
        }

        #endregion

        #region Public Methods

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
        public IMemberConstraint ResolveConstraint(Type constraintType)
        {
            #region Argument Check

            if (constraintType == null)
            {
                throw new ArgumentNullException("constraintType");
            }

            #endregion

            lock (_constraintCache)
            {
                var result = _constraintCache.GetValueOrCreate(
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

        #endregion
    }
}