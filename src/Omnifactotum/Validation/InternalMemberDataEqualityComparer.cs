using System.Collections.Generic;

namespace Omnifactotum.Validation
{
    internal sealed class InternalMemberDataEqualityComparer : IEqualityComparer<MemberData>
    {
        /// <summary>
        ///     The sole instance of the <see cref="InternalMemberDataEqualityComparer"/> class.
        /// </summary>
        public static readonly InternalMemberDataEqualityComparer Instance = new InternalMemberDataEqualityComparer();

        private InternalMemberDataEqualityComparer()
        {
            // Nothing to do
        }

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">
        ///     The first object of type <see cref="MemberData"/> to compare.
        /// </param>
        /// <param name="y">
        ///     The second object of type <see cref="MemberData"/> to compare.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the specified objects are equal; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Equals(MemberData x, MemberData y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return ByReferenceEqualityComparer<object>.Instance.Equals(x.Value, y.Value);
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
        public int GetHashCode(MemberData obj)
        {
            //// ReSharper disable once ConditionIsAlwaysTrueOrFalse - Potentially false detection
            return obj is null ? 0 : ByReferenceEqualityComparer<object>.Instance.GetHashCode(obj.Value);
        }
    }
}