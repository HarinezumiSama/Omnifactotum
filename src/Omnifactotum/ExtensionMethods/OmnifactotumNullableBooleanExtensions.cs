using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable once CheckNamespace - Namespace is intentionally named so in order to simplify usage of extension methods

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
        ///     A string value to return when <paramref name="value"/> is <c>null</c>.
        /// </param>
        /// <param name="trueValueString">
        ///     A string value to return when <paramref name="value"/> is <c>true</c>.
        /// </param>
        /// <param name="falseValueString">
        ///     A string value to return when <paramref name="value"/> is <c>false</c>.
        /// </param>
        /// <returns>
        ///     The string representation of the specified nullable Boolean value.
        /// </returns>
        [Pure]
        public static string ToString(
            [CanBeNull] this bool? value,
            [CanBeNull] string noValueString,
            [CanBeNull] string trueValueString,
            [CanBeNull] string falseValueString)
            //// ReSharper disable once ArrangeRedundantParentheses :: For clarity
            => value.HasValue ? (value.Value ? trueValueString : falseValueString) : noValueString;

        /// <summary>
        ///     Gets the string representation of the specified nullable Boolean value.
        /// </summary>
        /// <param name="value">
        ///     The value to get the string representation of.
        /// </param>
        /// <param name="noValueProvider">
        ///     A reference to a method that provides a value to return when <paramref name="value"/>
        ///     is <c>null</c>.
        /// </param>
        /// <param name="trueValueProvider">
        ///     A reference to a method that provides a value to return when <paramref name="value"/>
        ///     is <c>true</c>.
        /// </param>
        /// <param name="falseValueProvider">
        ///     A reference to a method that provides a value to return when <paramref name="value"/>
        ///     is <c>false</c>.
        /// </param>
        /// <returns>
        ///     The string representation of the specified nullable Boolean value.
        /// </returns>
        public static string ToString(
            [CanBeNull] this bool? value,
            [NotNull] [InstantHandle] Func<string> noValueProvider,
            [NotNull] [InstantHandle] Func<string> trueValueProvider,
            [NotNull] [InstantHandle] Func<string> falseValueProvider)
            //// ReSharper disable once ArrangeRedundantParentheses :: For clarity
            => value.HasValue
                ? (value.Value
                    ? (trueValueProvider ?? throw new ArgumentNullException(nameof(trueValueProvider))).Invoke()
                    : (falseValueProvider ?? throw new ArgumentNullException(nameof(falseValueProvider))).Invoke())
                : (noValueProvider ?? throw new ArgumentNullException(nameof(noValueProvider))).Invoke();
    }
}