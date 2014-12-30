using System;
using System.Diagnostics;
using System.Globalization;
using Omnifactotum.Annotations;

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
        public static string GetLocalPath([NotNull] this Assembly assembly)
        {
            #region Argument Check

            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            if (assembly.IsDynamic)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The assembly {{ {0} }} is dynamic.",
                        assembly.FullName),
                    "assembly");
            }

            #endregion

            if (string.IsNullOrEmpty(assembly.Location))
            {
                throw CreateNoLocalPathException(assembly);
            }

            var uri = new Uri(assembly.CodeBase);
            if (!uri.IsFile)
            {
                throw CreateNoLocalPathException(assembly);
            }

            return uri.LocalPath.EnsureNotNull();
        }

        #endregion

        #region Private Methods

        private static InvalidOperationException CreateNoLocalPathException(Assembly assembly)
        {
            return new InvalidOperationException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "The assembly {{ {0} }} does not have a local path.",
                    assembly.FullName));
        }

        #endregion
    }
}