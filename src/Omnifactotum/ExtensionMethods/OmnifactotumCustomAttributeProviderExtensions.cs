using System;
using System.Collections.Generic;
using System.Linq;
using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    /// <summary>
    ///     Contains extension methods for <see cref="ICustomAttributeProvider"/> interface.
    /// </summary>
    public static class OmnifactotumCustomAttributeProviderExtensions
    {
        #region Public Methods

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
            #region Argument Check

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            #endregion

            //// As per MSDN:
            ////    Calling ICustomAttributeProvider.GetCustomAttributes on PropertyInfo or EventInfo when the inherit
            ////    parameter of GetCustomAttributes is true does not walk the type hierarchy. Use System.Attribute to
            ////    inherit custom attributes.
            var memberInfo = provider as MemberInfo;
            if (memberInfo != null)
            {
                return Attribute
                    .GetCustomAttributes(memberInfo, typeof(TAttribute), inherit)
                    .Cast<TAttribute>()
                    .ToArray();
            }

            return provider.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().ToArray();
        }

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
        [Obsolete(
            "For compatibility with FW 4.5+: use OmnifactotumCustomAttributeProviderExtensions.GetCustomAttributeArray.",
            true)]
        public static TAttribute[] GetCustomAttributes<TAttribute>(
            [NotNull] this ICustomAttributeProvider provider,
            bool inherit)
            where TAttribute : Attribute
        {
            return GetCustomAttributeArray<TAttribute>(provider, inherit);
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
        {
            return GetCustomAttributeArray<TAttribute>(provider, inherit).Single();
        }

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
        {
            return GetCustomAttributeArray<TAttribute>(provider, inherit).SingleOrDefault();
        }

        #endregion
    }
}