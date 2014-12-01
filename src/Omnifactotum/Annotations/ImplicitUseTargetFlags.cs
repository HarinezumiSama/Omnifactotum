using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    ///     Specify what is considered used implicitly when marked with <see cref="MeansImplicitUseAttribute"/>
    ///     or <see cref="UsedImplicitlyAttribute"/>.
    /// </summary>
    [Flags]
    public enum ImplicitUseTargetFlags
    {
        /// <summary>
        ///     The default set of the flags.
        /// </summary>
        Default = Itself,

        /// <summary>
        ///     The entity marked with the attribute is considered used.
        /// </summary>
        Itself = 1,

        /// <summary>
        ///     The members of the entity marked with the attribute are considered used.
        /// </summary>
        Members = 2,

        /// <summary>
        ///     The entity marked with the attribute and all its members are considered used.
        /// </summary>
        WithMembers = Itself | Members
    }
}