using System.IO;
using Omnifactotum.Annotations;
using static Omnifactotum.FormattableStringFactotum;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
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
        ///     The specified assembly is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     The specified assembly does not have a local path.
        /// </exception>
        [NotNull]
        public static string GetLocalPath([NotNull] this Assembly assembly)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.Location))
            {
                throw CreateNoLocalPathException(assembly, nameof(assembly));
            }

            var uri = new Uri(assembly.Location);
            if (!uri.IsFile || !uri.IsAbsoluteUri || string.IsNullOrWhiteSpace(uri.LocalPath))
            {
                throw CreateNoLocalPathException(assembly, nameof(assembly));
            }

            return Path.GetFullPath(uri.LocalPath);

            static ArgumentException CreateNoLocalPathException(Assembly assembly, string argumentName)
                => new(AsInvariant($@"The assembly {{ {assembly.FullName} }} does not have a local path."), argumentName);
        }
    }
}