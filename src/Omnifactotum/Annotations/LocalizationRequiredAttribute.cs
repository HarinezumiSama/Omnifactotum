using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// Indicates that marked element should be localized or not
    /// </summary>
    /// <example><code>
    /// [LocalizationRequiredAttribute(true)]
    /// public class Foo {
    ///   private string str = "my string"; // Warning: Localizable string
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class LocalizationRequiredAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationRequiredAttribute"/> class.
        /// </summary>
        public LocalizationRequiredAttribute()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationRequiredAttribute"/> class.
        /// </summary>
        /// <param name="required">
        /// The required.
        /// </param>
        public LocalizationRequiredAttribute(bool required)
        {
            Required = required;
        }

        /// <summary>
        /// Gets a value indicating whether required.
        /// </summary>
        public bool Required
        {
            get;
            private set;
        }
    }
}