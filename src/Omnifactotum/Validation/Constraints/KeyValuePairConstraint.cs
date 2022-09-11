#nullable enable

using System.Collections.Generic;

namespace Omnifactotum.Validation.Constraints
{
    /// <summary>
    ///     Represents the constraint for validating <see cref="KeyValuePair{TKey,TValue}"/> instances.
    /// </summary>
    /// <typeparam name="TKey">
    ///     The type of the key.
    /// </typeparam>
    /// <typeparam name="TValue">
    ///     The type of the value.
    /// </typeparam>
    /// <typeparam name="TKeyConstraint">
    ///     The type specifying the key constraint.
    /// </typeparam>
    /// <typeparam name="TValueConstraint">
    ///     The type specifying the value constraint.
    /// </typeparam>
    public sealed class KeyValuePairConstraint<TKey, TValue, TKeyConstraint, TValueConstraint>
        : KeyValuePairConstraintBase<TKey, TValue>
        where TKeyConstraint : TypedMemberConstraintBase<TKey>
        where TValueConstraint : TypedMemberConstraintBase<TValue>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KeyValuePairConstraint{TKey,TValue,TKeyConstraint,TValueConstraint}"/> class.
        /// </summary>
        public KeyValuePairConstraint()
            : base(typeof(TKeyConstraint), typeof(TValueConstraint))
        {
            // Nothing to do
        }
    }
}