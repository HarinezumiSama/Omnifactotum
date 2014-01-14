using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// The html element attributes attribute.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field,
        Inherited = true)]
    public sealed class HtmlElementAttributesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlElementAttributesAttribute"/> class.
        /// </summary>
        public HtmlElementAttributesAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlElementAttributesAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name to initialize with.
        /// </param>
        public HtmlElementAttributesAttribute([NotNull] string name)
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