using System.Runtime.CompilerServices;
using Omnifactotum;
using Omnifactotum.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes

//// ReSharper disable once CheckNamespace :: Namespace is intentionally named so in order to simplify usage of extension methods
namespace System;

/// <summary>
///     Contains extension methods for the <b>nullable</b> <see cref="char"/> structure.
/// </summary>
public static class OmnifactotumNullableCharExtensions
{
    /// <summary>
    ///     Converts the specified nullable <see cref="char"/> value to its UI representation using
    ///     <see cref="OmnifactotumCharExtensions.ToUIString"/> if the value is not <see langword="null"/>;
    ///     otherwise, returns <see cref="OmnifactotumRepresentationConstants.NullValueRepresentation"/>.
    /// </summary>
    /// <param name="value">
    ///     The nullable character value to convert.
    /// </param>
    /// <returns>
    ///     A UI string representation of the specified nullable character.
    /// </returns>
    /// <example>
    ///     <code>
    /// <![CDATA[
    ///         char? value0 = null;
    ///         Console.WriteLine("Value is {0}.", value0.ToUIString()); // Output: Value is null.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         char? value1 = ' ';
    ///         Console.WriteLine("Value is {0}.", value1.ToUIString()); // Output: Value is ' '.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         char? value2 = 'H';
    ///         Console.WriteLine("Value is {0}.", value2.ToUIString()); // Output: Value is 'H'.
    /// ]]>
    ///     </code>
    ///     <code>
    /// <![CDATA[
    ///         char? value3 = '\'';
    ///         Console.WriteLine("Value is {0}.", value3.ToUIString()); // Output: Value is ''''.
    /// ]]>
    ///     </code>
    /// </example>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    [MethodImpl(OmnifactotumConstants.MethodOptimizationOptions.Standard)]
    [NotNull]
    public static string ToUIString(this char? value)
        => value is null ? OmnifactotumRepresentationConstants.NullValueRepresentation : value.Value.ToUIString();
}