using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// ASP.NET MVC attribute. Indicates that a parameter is an MVC area.
    /// Use this attribute for custom wrappers similar to
    /// <c>System.Web.Mvc.Html.ChildActionExtensions.RenderAction(HtmlHelper, String)</c>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class AspMvcAreaAttribute : PathReferenceAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AspMvcAreaAttribute"/> class.
        /// </summary>
        public AspMvcAreaAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspMvcAreaAttribute"/> class.
        /// </summary>
        /// <param name="anonymousProperty">
        /// The anonymous property.
        /// </param>
        public AspMvcAreaAttribute([NotNull] string anonymousProperty)
        {
            AnonymousProperty = anonymousProperty;
        }

        /// <summary>
        /// Gets the anonymous property.
        /// </summary>
        [NotNull]
        public string AnonymousProperty
        {
            get;
            private set;
        }
    }
}