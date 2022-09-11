using System.Collections.Generic;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation
{
    internal sealed class InternalMemberDataEqualityComparer : IEqualityComparer<MemberData>
    {
        /// <summary>
        ///     The sole instance of the <see cref="InternalMemberDataEqualityComparer"/> class.
        /// </summary>
        public static readonly InternalMemberDataEqualityComparer Instance = new();

        private static readonly ByReferenceEqualityComparer<object> EqualityComparer = ByReferenceEqualityComparer<object>.Instance;

        private InternalMemberDataEqualityComparer()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="left">
        ///     The first object of type <see cref="MemberData"/> to compare.
        /// </param>
        /// <param name="right">
        ///     The second object of type <see cref="MemberData"/> to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals([CanBeNull] MemberData? left, [CanBeNull] MemberData? right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return EqualityComparer.Equals(left.Value, right.Value);
        }

        /// <summary>
        ///     Returns a hash code for the specified instance.
        /// </summary>
        /// <param name="obj">
        ///     The object to get a hash code for.
        /// </param>
        /// <returns>
        ///     A hash code for the specified instance.
        /// </returns>
        //// ReSharper disable once ConditionIsAlwaysTrueOrFalse :: Potentially false detection
        public int GetHashCode([CanBeNull] MemberData? obj) => obj is null ? 0 : EqualityComparer.GetHashCode(obj.Value);
    }
}