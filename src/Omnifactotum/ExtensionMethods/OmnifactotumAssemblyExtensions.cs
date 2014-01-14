using System;
using System.Diagnostics;
using System.Globalization;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Reflection.Assembly"/> class.
    /// </summary>
    public static class OmnifactotumAssemblyExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the local path of the assembly.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly to get the local path of.
        /// </param>
        /// <returns>
        ///     The local path of the specified assembly.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     The specified assembly does not have a local path.
        /// </exception>
        [DebuggerNonUserCode]
        public static string GetLocalPath(this Assembly assembly)
        {
            #region Argument Check

            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            #endregion

            var uri = new Uri(assembly.CodeBase);
            if (!uri.IsFile)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The path of the assembly {{{0}}} is not local.",
                        assembly.FullName));
            }

            return uri.LocalPath.EnsureNotNull();
        }

        #endregion
    }
}