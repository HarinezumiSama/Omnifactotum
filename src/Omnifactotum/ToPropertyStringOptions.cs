using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents the options used by
    ///     the <see cref="Helper.ToPropertyString{T}(T,ToPropertyStringOptions)"/> method.
    /// </summary>
    public sealed class ToPropertyStringOptions : ICloneable
    {
        #region Constants and Fields

        /// <summary>
        ///     The default value of the <see cref="ToPropertyStringOptions.MaxCollectionItemCount"/> property.
        /// </summary>
        public static readonly int DefaultMaxCollectionItemCount = 32;

        private static readonly PropertyInfo[] FlagPropertyInfos = typeof(ToPropertyStringOptions)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(item => item.CanWrite && item.PropertyType == typeof(bool) && !item.GetIndexParameters().Any())
            .ToArray();

        private int _maxCollectionItemCount;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ToPropertyStringOptions"/> class.
        /// </summary>
        public ToPropertyStringOptions()
        {
            this.MaxCollectionItemCount = DefaultMaxCollectionItemCount;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the actual type of the root object should be rendered..
        /// </summary>
        public bool RenderRootActualType
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the actual type of each inner object should be rendered..
        /// </summary>
        public bool RenderActualType
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the complex properties should be rendered.
        /// </summary>
        public bool RenderComplexProperties
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the non-public members should be included.
        /// </summary>
        public bool IncludeNonPublicMembers
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the declared type of each member should be rendered.
        /// </summary>
        public bool RenderMemberType
        {
            get;
            set;
        }

        // TODO [vmcl] Add AutoRenderStringRepresentation flag
        // TODO [vmcl] Add AlwaysRenderStringRepresentation flag
        // TODO [vmcl] Add RenderFields flag
        // TODO [vmcl] Add MaxRecursionLevel property
        // TODO [vmcl] Add callback Func<MemberInfo, bool> that determines whether to include a specific member

        /// <summary>
        ///     Gets or sets a value indicating whether the members should be sorted alphabetically.
        /// </summary>
        public bool SortMembersAlphabetically
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the maximum number of items to render from collections.
        /// </summary>
        public int MaxCollectionItemCount
        {
            [DebuggerStepThrough]
            get
            {
                return _maxCollectionItemCount;
            }

            [DebuggerNonUserCode]
            set
            {
                #region Argument Check

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", value, "The value cannot be negative.");
                }

                #endregion

                _maxCollectionItemCount = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Creates a new <see cref="ToPropertyStringOptions"/> that is a copy of this instance.
        /// </summary>
        /// <returns>
        ///     A new <see cref="ToPropertyStringOptions"/> that is a copy of this instance.
        /// </returns>
        public ToPropertyStringOptions Clone()
        {
            return (ToPropertyStringOptions)MemberwiseClone();
        }

        /// <summary>
        ///     Sets all the possible flags in the options.
        /// </summary>
        /// <param name="value">
        ///     The value to set to each flag.
        /// </param>
        /// <returns>
        ///     This <see cref="ToPropertyStringOptions"/>.
        /// </returns>
        public ToPropertyStringOptions SetAllFlags(bool value)
        {
            FlagPropertyInfos.DoForEach(item => item.SetValue(this, value, null));
            return this;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}