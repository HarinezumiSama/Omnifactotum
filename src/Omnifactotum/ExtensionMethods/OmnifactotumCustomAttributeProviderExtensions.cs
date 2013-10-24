﻿using System;
using System.Collections.Generic;
using System.Linq;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System.Reflection

// ReSharper restore CheckNamespace
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
        ///     <b>true</b> to look up the hierarchy chain for the inherited custom attribute;
        ///     otherwise, <b>false</b>.
        /// </param>
        /// <returns>
        ///     The list of the attributes of the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="provider"/> is <b>null</b>.
        /// </exception>
        public static List<TAttribute> GetCustomAttributes<TAttribute>(
            this ICustomAttributeProvider provider,
            bool inherit = false)
            where TAttribute : Attribute
        {
            #region Argument Check

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            #endregion

            return provider.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().ToList();
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
        ///     <b>true</b> to look up the hierarchy chain for the inherited custom attribute;
        ///     otherwise, <b>false</b>.
        /// </param>
        /// <returns>
        ///     The attribute of the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="provider"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The specified attribute either is not applied to the specified provider at all
        ///     or is applied more than once.
        /// </exception>
        public static TAttribute GetSingleCustomAttribute<TAttribute>(
            this ICustomAttributeProvider provider,
            bool inherit = false)
            where TAttribute : Attribute
        {
            return GetSingleCustomAttributeInternal<TAttribute>(provider, inherit, Enumerable.Single);
        }

        /// <summary>
        ///     Gets the sole specified attribute applied to the specified provider of custom attributes,
        ///     or <b>null</b> if the specified attribute is not applied.
        /// </summary>
        /// <typeparam name="TAttribute">
        ///     The type of the attribute.
        /// </typeparam>
        /// <param name="provider">
        ///     The provider of custom attributes to get the sole attribute of.
        /// </param>
        /// <param name="inherit">
        ///     <b>true</b> to look up the hierarchy chain for the inherited custom attribute;
        ///     otherwise, <b>false</b>.
        /// </param>
        /// <returns>
        ///     The attribute of the specified type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///     <paramref name="provider"/> is <b>null</b>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     The specified attribute is applied more than once.
        /// </exception>
        public static TAttribute GetSingleOrDefaultCustomAttribute<TAttribute>(
            this ICustomAttributeProvider provider,
            bool inherit = false)
            where TAttribute : Attribute
        {
            return GetSingleCustomAttributeInternal<TAttribute>(provider, inherit, Enumerable.SingleOrDefault);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Gets the single custom attribute.
        /// </summary>
        /// <typeparam name="TAttribute">
        ///     The type of the attribute.
        /// </typeparam>
        /// <param name="provider">
        ///     The attribute provider.
        /// </param>
        /// <param name="inherit">
        ///     <b>true</b> to look up the hierarchy chain for the inherited custom attribute;
        ///     otherwise, <b>false</b>.
        /// </param>
        /// <param name="getSingle">
        ///     A reference to a method obtaining the single element.
        /// </param>
        /// <returns>
        ///     A single attribute.
        /// </returns>
        private static TAttribute GetSingleCustomAttributeInternal<TAttribute>(
            ICustomAttributeProvider provider,
            bool inherit,
            Func<IEnumerable<object>, object> getSingle)
            where TAttribute : Attribute
        {
            #region Argument Check

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            #endregion

            return (TAttribute)getSingle(provider.GetCustomAttributes(typeof(TAttribute), inherit));
        }

        #endregion
    }
}