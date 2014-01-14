using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// This attribute is intended to mark publicly available API
    /// which should not be removed and so is treated as used
    /// </summary>
    [MeansImplicitUse]
    //// ReSharper disable once InconsistentNaming
    public sealed class PublicAPIAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicAPIAttribute"/> class.
        /// </summary>
        public PublicAPIAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicAPIAttribute"/> class.
        /// </summary>
        /// <param name="comment">
        /// The comment.
        /// </param>
        public PublicAPIAttribute([NotNull] string comment)
        {
            Comment = comment;
        }

        /// <summary>
        /// Gets the comment.
        /// </summary>
        [NotNull]
        public string Comment
        {
            get;
            private set;
        }
    }
}