using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// The html attribute value attribute.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property,
        Inherited = true)]
    public sealed class HtmlAttributeValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlAttributeValueAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name to initialize with.
        /// </param>
        public HtmlAttributeValueAttribute([NotNull] string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        [NotNull]
        public string Name
        {
            get;
            private set;
        }
    }
}