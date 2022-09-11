using System.Runtime.CompilerServices;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;
using SuppressMessageAttribute = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable UseNullableReferenceTypesAnnotationSyntax

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System
{
    /// <summary>
    ///     Contains extension methods for the
    ///     <see cref="System.Nullable{T}">System.Nullable</see>&lt;<see cref="System.Boolean"/>&gt; type.
    /// </summary>
    public static class OmnifactotumNullableBooleanExtensions
    {
        /// <summary>
        ///     Gets the string representation of the specified nullable Boolean value.
        /// </summary>
        /// <param name="value">
        ///     The value to get the string representation of.
        /// </param>
        /// <param name="noValueString">
        ///     A string value to return when <paramref name="value"/> is <see langword="null"/>.
        /// </param>
        /// <param name="trueValueString">
        ///     A string value to return when <paramref name="value"/> is <see langword="true"/>.
        /// </param>
        /// <param name="falseValueString">
        ///     A string value to return when <paramref name="value"/> is <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     The string representation of the specified nullable Boolean value.
        /// </returns>
        [Pure]
        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses", Justification = "For clarity.")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        public static string? ToString(
            [CanBeNull] this bool? value,
            [CanBeNull] string? noValueString,
            [CanBeNull] string? trueValueString,
            [CanBeNull] string? falseValueString)
            => value.HasValue ? (value.Value ? trueValueString : falseValueString) : noValueString;

        /// <summary>
        ///     Gets the string representation of the specified nullable Boolean value.
        /// </summary>
        /// <param name="value">
        ///     The value to get the string representation of.
        /// </param>
        /// <param name="noValueProvider">
        ///     A reference to a method that provides a value to return when <paramref name="value"/>
        ///     is <see langword="null"/>.
        /// </param>
        /// <param name="trueValueProvider">
        ///     A reference to a method that provides a value to return when <paramref name="value"/>
        ///     is <see langword="true"/>.
        /// </param>
        /// <param name="falseValueProvider">
        ///     A reference to a method that provides a value to return when <paramref name="value"/>
        ///     is <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     The string representation of the specified nullable Boolean value.
        /// </returns>
        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses", Justification = "For clarity.")]
        [CanBeNull]
        public static string? ToString(
            [CanBeNull] this bool? value,
            [NotNull] [InstantHandle] Func<string?> noValueProvider,
            [NotNull] [InstantHandle] Func<string?> trueValueProvider,
            [NotNull] [InstantHandle] Func<string?> falseValueProvider)
            => value.HasValue
                ? (value.Value
                    ? (trueValueProvider ?? throw new ArgumentNullException(nameof(trueValueProvider))).Invoke()
                    : (falseValueProvider ?? throw new ArgumentNullException(nameof(falseValueProvider))).Invoke())
                : (noValueProvider ?? throw new ArgumentNullException(nameof(noValueProvider))).Invoke();
    }
}