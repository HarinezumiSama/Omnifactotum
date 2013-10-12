using System;
using System.Collections.Generic;
using System.Diagnostics;

//// Namespace is intentionally named so in order to simplify usage of extension methods

// ReSharper disable CheckNamespace
namespace System

// ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Contains extension methods for
    ///     the <see cref="System.Nullable{T}">System.Nullable</see>&lt;<see cref="System.Boolean"/>&gt; type.
    /// </summary>
    public static class OmnifactotumNullableBooleanExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Gets the string representation of the specified nullable Boolean value.
        /// </summary>
        /// <param name="value">
        ///     The value to get the string representation of.
        /// </param>
        /// <param name="noValueString">
        ///     Specifies the string to return if <paramref name="value"/> is <b>null</b>, that is, has no inner value.
        /// </param>
        /// <param name="trueValueString">
        ///     Specifies the string to return if <paramref name="value"/> is <b>true</b>.
        /// </param>
        /// <param name="falseValueString">
        ///     Specifies the string to return if <paramref name="value"/> is <b>false</b>.
        /// </param>
        /// <returns>
        ///     The string representation of the specified nullable Boolean value.
        /// </returns>
        public static string ToString(
            this bool? value,
            string noValueString,
            string trueValueString,
            string falseValueString)
        {
            return value.HasValue ? (value.Value ? trueValueString : falseValueString) : noValueString;
        }

        #endregion
    }
}