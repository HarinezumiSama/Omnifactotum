using System;
using System.Collections.Generic;
using System.Linq;

namespace Omnifactotum.Validation
{
    internal sealed class InternalMemberDataEqualityComparer : IEqualityComparer<MemberData>
    {
        #region Constants and Fields

        /// <summary>
        ///     The sole instance of the <see cref="InternalMemberDataEqualityComparer"/> class.
        /// </summary>
        public static readonly InternalMemberDataEqualityComparer Instance = new InternalMemberDataEqualityComparer();

        #endregion

        #region Constructors

        private InternalMemberDataEqualityComparer()
        {
            // Nothing to do
        }

        #endregion

        #region IEqualityComparer<MemberData> Members

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
        ///     <b>true</b> if the specified objects are equal; otherwise, <b>false</b>.
        /// </returns>
        public bool Equals(MemberData x, MemberData y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
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
            return obj == null ? 0 : ByReferenceEqualityComparer<object>.Instance.GetHashCode(obj.Value);
        }

        #endregion
    }
}