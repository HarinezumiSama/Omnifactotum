using System;
using System.Linq;

namespace Omnifactotum.Annotations
{
    /// <summary>
    /// Indicates that the marked method builds string by format pattern and (optional) arguments.
    /// Parameter, which contains format string, should be given in constructor. The format string
    /// should be in <see cref="string.Format(IFormatProvider,string,object[])"/>-like form
    /// </summary>
    /// <example><code>
    /// [StringFormatMethod("message")]
    /// public void ShowError(string message, params object[] args) { /* do something */ }
    /// public void Foo() {
    ///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class StringFormatMethodAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringFormatMethodAttribute"/> class.
        /// The string format method attribute.
        /// </summary>
        /// <param name="formatParameterName">
        /// Specifies which parameter of an annotated method should be treated as format-string
        /// </param>
        public StringFormatMethodAttribute(string formatParameterName)
        {
            FormatParameterName = formatParameterName;
        }

        /// <summary>
        /// Gets the format parameter name.
        /// </summary>
        public string FormatParameterName
        {
            get;
            private set;
        }
    }
}