using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// Should be used on attributes and causes ReSharper
    /// to not mark symbols marked with such attributes as unused
    /// (as well as by other usage inspections)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MeansImplicitUseAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
        /// </summary>
        public MeansImplicitUseAttribute()
            : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
        /// </summary>
        /// <param name="useKindFlags">
        /// The use kind flags.
        /// </param>
        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
            : this(useKindFlags, ImplicitUseTargetFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
        /// </summary>
        /// <param name="targetFlags">
        /// The target flags.
        /// </param>
        public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
            : this(ImplicitUseKindFlags.Default, targetFlags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
        /// </summary>
        /// <param name="useKindFlags">
        /// The use kind flags.
        /// </param>
        /// <param name="targetFlags">
        /// The target flags.
        /// </param>
        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
        {
            UseKindFlags = useKindFlags;
            TargetFlags = targetFlags;
        }

        /// <summary>
        /// Gets the use kind flags.
        /// </summary>
        [UsedImplicitly]
        public ImplicitUseKindFlags UseKindFlags
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the target flags.
        /// </summary>
        [UsedImplicitly]
        public ImplicitUseTargetFlags TargetFlags
        {
            get;
            private set;
        }
    }
}