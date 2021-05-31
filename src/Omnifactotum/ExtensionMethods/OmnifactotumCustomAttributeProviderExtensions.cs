using System.Linq;
using Omnifactotum.Annotations;

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods

namespace System.Reflection
{
    /// <summary>
    ///     Contains extension methods for <see cref="ICustomAttributeProvider"/> interface.
    /// </summary>
    public static class OmnifactotumCustomAttributeProviderExtensions
    {
        /// <summary>
        ///     Gets the list of the attributes, of the specified type, applied to the specified provider of
        ///     custom attributes.
        /// </summary>
        /// <typeparam name="TAttribute">
        ///     The type of the attribute.
        /// </typeparam>
        /// <param name="provider">
        ///     The provider of custom attributes to get the attribute list of.
        /// </param>
        /// <param name="inherit">
        ///     <c>true</c> to look up the hierarchy chain for the inherited custom attribute;
        ///     otherwise, <c>false</c>.
        /// </param>
        /// <returns>
        ///     The list of the attributes of the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="provider"/> is <c>null</c>.
        /// </exception>
        public static TAttribute[] GetCustomAttributeArray<TAttribute>(
            [NotNull] this ICustomAttributeProvider provider,
            bool inherit)
            where TAttribute : Attribute
        {
            //// As per MSDN:
            ////    Calling ICustomAttributeProvider.GetCustomAttributes on PropertyInfo or EventInfo when the inherit
            ////    parameter of GetCustomAttributes is true does not walk the type hierarchy. Use System.Attribute to
            ////    inherit custom attributes.

            switch (provider)
            {
                //// ReSharper disable once HeuristicUnreachableCode :: False detection
                case null:
                    //// ReSharper disable once HeuristicUnreachableCode :: False detection
                    throw new ArgumentNullException(nameof(provider));

                case MemberInfo memberInfo:
                    return Attribute
                        .GetCustomAttributes(memberInfo, typeof(TAttribute), inherit)
                        .Cast<TAttribute>()
                        .ToArray();

                default:
                    return provider.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().ToArray();
            }
        }

        /// <summary>
        ///     Gets the sole specified attribute applied to the specified provider of custom attributes.
        /// </summary>
        /// <typeparam name="TAttribute">
        ///     The type of the attribute.
        /// </typeparam>
        /// <param name="provider">
        ///     The provider of custom attributes to get the sole attribute of.
        /// </param>
        /// <param name="inherit">
        ///     <c>true</c> to look up the hierarchy chain for the inherited custom attribute;
        ///     otherwise, <c>false</c>.
        /// </param>
        /// <returns>
        ///     The attribute of the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="provider"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The specified attribute either is not applied to the specified provider at all
        ///     or is applied more than once.
        /// </exception>
        public static TAttribute GetSingleCustomAttribute<TAttribute>(
            [NotNull] this ICustomAttributeProvider provider,
            bool inherit)
            where TAttribute : Attribute
            => GetCustomAttributeArray<TAttribute>(provider, inherit).Single();

        /// <summary>
        ///     Gets the sole specified attribute applied to the specified provider of custom attributes,
        ///     or <c>null</c> if the specified attribute is not applied.
        /// </summary>
        /// <typeparam name="TAttribute">
        ///     The type of the attribute.
        /// </typeparam>
        /// <param name="provider">
        ///     The provider of custom attributes to get the sole attribute of.
        /// </param>
        /// <param name="inherit">
        ///     <c>true</c> to look up the hierarchy chain for the inherited custom attribute;
        ///     otherwise, <c>false</c>.
        /// </param>
        /// <returns>
        ///     The attribute of the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="provider"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The specified attribute is applied more than once.
        /// </exception>
        public static TAttribute GetSingleOrDefaultCustomAttribute<TAttribute>(
            [NotNull] this ICustomAttributeProvider provider,
            bool inherit)
            where TAttribute : Attribute
            => GetCustomAttributeArray<TAttribute>(provider, inherit).SingleOrDefault();
    }
}