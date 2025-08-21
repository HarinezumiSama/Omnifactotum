using System;
using System.Text.RegularExpressions;
using Omnifactotum.Annotations;

#if NET7_0_OR_GREATER
using StringSyntaxAttribute = System.Diagnostics.CodeAnalysis.StringSyntaxAttribute;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="String"/> type should not be <see langword="null"/> and
///     should match the specified regular expression.
/// </summary>
/// <seealso cref="Regex"/>
/// <seealso cref="RegexOptions"/>
/// <seealso cref="OptionalRegexStringConstraintBase"/>
public abstract class NotNullRegexStringConstraintBase : CommonRegexStringConstraintBase
{
    /// <summary>
    ///     The default <see cref="RegexOptions"/> used in the <see cref="NotNullRegexStringConstraintBase"/> constructor.
    /// </summary>
    [PublicAPI]
    public const RegexOptions DefaultRegexOptions = CommonDefaultRegexOptions;

    /// <summary>
    ///     The default regular expression match evaluation timeout, in milliseconds.
    /// </summary>
    [PublicAPI]
    public const int DefaultRegexTimeoutMilliseconds = CommonDefaultRegexTimeoutMilliseconds;

    /// <summary>
    ///     The default regular expression match evaluation timeout.
    /// </summary>
    [PublicAPI]
    public static readonly TimeSpan DefaultRegexTimeout = CommonDefaultRegexTimeout;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullRegexStringConstraintBase" /> class using the specified regular expression.
    /// </summary>
    /// <param name="regex">
    ///     The regular expression that the annotated member should satisfy.
    /// </param>
    protected NotNullRegexStringConstraintBase([NotNull] Regex regex)
        : base(regex, false)
    {
        // Nothing to do
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullRegexStringConstraintBase" /> class
    ///     using the specified regular expression pattern, options, and timeout.
    /// </summary>
    /// <param name="pattern">
    ///     The regular expression pattern that the annotated member should satisfy.
    /// </param>
    /// <param name="options">
    ///     The <see cref="RegexOptions"/> to pass to <see cref="Regex"/> used for validation.
    /// </param>
    /// <param name="timeout">
    ///     The regular expression match evaluation timeout, or <see langword="null"/> to use <see cref="DefaultRegexTimeout"/>.
    /// </param>
    /// <seealso cref="DefaultRegexOptions"/>
    /// <seealso cref="DefaultRegexTimeout"/>
    protected NotNullRegexStringConstraintBase(
#if NET7_0_OR_GREATER
        [StringSyntax(StringSyntaxAttribute.Regex)]
#endif
        [NotNull] [RegexPattern] string pattern,
        RegexOptions options = DefaultRegexOptions,
        TimeSpan? timeout = null)
        : base(pattern, options, timeout, false)
    {
        // Nothing to do
    }
}