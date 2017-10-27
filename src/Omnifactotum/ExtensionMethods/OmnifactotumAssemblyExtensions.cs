﻿using Omnifactotum.Annotations;

//// ReSharper disable once CheckNamespace - Namespace is intentionally named so in order to simplify usage of extension methods

namespace System.Reflection
{
    /// <summary>
    ///     Contains extension methods for the <see cref="System.Reflection.Assembly"/> class.
    /// </summary>
    public static class OmnifactotumAssemblyExtensions
    {
        /// <summary>
        ///     Gets the local path of the assembly.
        /// </summary>
        /// <param name="assembly">
        ///     The assembly to get the local path of.
        /// </param>
        /// <returns>
        ///     The local path of the specified assembly.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The specified assembly is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The specified assembly does not have a local path.
        /// </exception>
        public static string GetLocalPath([NotNull] this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (assembly.IsDynamic)
            {
                throw new ArgumentException(
                    $@"The assembly {{ {assembly.FullName} }} has no local path because it is dynamic.",
                    nameof(assembly));
            }

            ArgumentException CreateNoLocalPathException()
                => new ArgumentException(
                    $@"The assembly {{ {assembly.FullName} }} does not have a local path.",
                    nameof(assembly));

            if (string.IsNullOrEmpty(assembly.Location) || string.IsNullOrEmpty(assembly.CodeBase))
            {
                throw CreateNoLocalPathException();
            }

            var uri = new Uri(assembly.CodeBase);
            if (!uri.IsFile)
            {
                throw CreateNoLocalPathException();
            }

            return uri.LocalPath.EnsureNotNull();
        }
    }
}