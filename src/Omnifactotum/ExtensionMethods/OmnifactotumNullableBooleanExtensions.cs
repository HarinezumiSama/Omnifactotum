using Omnifactotum.Annotations;

//// Namespace is intentionally named so in order to simplify usage of extension methods
//// ReSharper disable once CheckNamespace
namespace System
{
    /// <summary>
    ///     Contains extension methods for
    ///     the <see cref="System.Nullable{T}">System.Nullable</see>&lt;<see cref="System.Boolean"/>&gt; type.
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
        ///     Specifies the string to return if <paramref name="value"/> is <c>null</c>, that is, has no inner value.
        /// </param>
        /// <param name="trueValueString">
        ///     Specifies the string to return if <paramref name="value"/> is <c>true</c>.
        /// </param>
        /// <param name="falseValueString">
        ///     Specifies the string to return if <paramref name="value"/> is <c>false</c>.
        /// </param>
        /// <returns>
        ///     The string representation of the specified nullable Boolean value.
        /// </returns>
        public static string ToString(
            [CanBeNull] this bool? value,
            string noValueString,
            string trueValueString,
            string falseValueString)
        {
            return value.HasValue ? (value.Value ? trueValueString : falseValueString) : noValueString;
        }
    }
}