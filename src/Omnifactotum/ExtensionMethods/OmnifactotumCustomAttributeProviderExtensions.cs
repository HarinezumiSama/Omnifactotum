using System.Linq;
using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System.Reflection;

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
    ///     <see langword="true"/> to look up the hierarchy chain for the inherited custom attribute;
    ///     otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    ///     The list of the attributes of the specified type.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    [NotNull]
    public static TAttribute[] GetCustomAttributeArray<TAttribute>(
        [NotNull] this ICustomAttributeProvider provider,
        bool inherit)
        where TAttribute : Attribute
    {
        //// As per MSDN:
        ////    Calling ICustomAttributeProvider.GetCustomAttributes on PropertyInfo or EventInfo when the inherit
        ////    parameter of GetCustomAttributes is true does not walk the type hierarchy. Use System.Attribute to
        ////    inherit custom attributes.

        return provider switch
        {
            null => throw new ArgumentNullException(nameof(provider)),

            MemberInfo memberInfo => Attribute
                .GetCustomAttributes(memberInfo, typeof(TAttribute), inherit)
                .Cast<TAttribute>()
                .ToArray(),

            _ => provider.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().ToArray()
        };
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
    ///     <see langword="true"/> to look up the hierarchy chain for the inherited custom attribute;
    ///     otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    ///     The attribute of the specified type.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    ///     The specified attribute either is not applied to the specified provider at all
    ///     or is applied more than once.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [NotNull]
    public static TAttribute GetSingleCustomAttribute<TAttribute>(
        [NotNull] this ICustomAttributeProvider provider,
        bool inherit)
        where TAttribute : Attribute
        => GetCustomAttributeArray<TAttribute>(provider, inherit).Single();

    /// <summary>
    ///     Gets the sole specified attribute applied to the specified provider of custom attributes,
    ///     or <see langword="null"/> if the specified attribute is not applied.
    /// </summary>
    /// <typeparam name="TAttribute">
    ///     The type of the attribute.
    /// </typeparam>
    /// <param name="provider">
    ///     The provider of custom attributes to get the sole attribute of.
    /// </param>
    /// <param name="inherit">
    ///     <see langword="true"/> to look up the hierarchy chain for the inherited custom attribute;
    ///     otherwise, <see langword="false"/>.
    /// </param>
    /// <returns>
    ///     The attribute of the specified type.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     <paramref name="provider"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.InvalidOperationException">
    ///     The specified attribute is applied more than once.
    /// </exception>
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Maximum)]
    [CanBeNull]
    public static TAttribute? GetSingleOrDefaultCustomAttribute<TAttribute>(
        [NotNull] this ICustomAttributeProvider provider,
        bool inherit)
        where TAttribute : Attribute
        => GetCustomAttributeArray<TAttribute>(provider, inherit).SingleOrDefault();
}