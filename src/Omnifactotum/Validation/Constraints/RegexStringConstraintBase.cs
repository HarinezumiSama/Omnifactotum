using System;
using System.Text.RegularExpressions;
using Omnifactotum.Annotations;

#if NET7_0_OR_GREATER
using StringSyntaxAttribute = System.Diagnostics.CodeAnalysis.StringSyntaxAttribute;
#endif

//// ReSharper disable RedundantNullnessAttributeWithNullableReferenceTypes
//// ReSharper disable AnnotationRedundancyInHierarchy

namespace Omnifactotum.Validation.Constraints;

/// <inheritdoc cref="NotNullRegexStringConstraintBase"/>
/// <seealso cref="NotNullRegexStringConstraintBase"/>
[Obsolete($"Use '{nameof(NotNullRegexStringConstraintBase)}' instead.")]
public abstract class RegexStringConstraintBase : LegacyTypedMemberConstraintBase<string?, NotNullRegexStringConstraintBase>
{
    /// <summary>
    ///     The default <see cref="RegexOptions"/> used in the <see cref="RegexStringConstraintBase"/> constructor.
    /// </summary>
    [PublicAPI]
    public const RegexOptions DefaultRegexOptions = NotNullRegexStringConstraintBase.DefaultRegexOptions;

    /// <summary>
    ///     The default regular expression match evaluation timeout.
    /// </summary>
    [PublicAPI]
    public static readonly TimeSpan DefaultRegexTimeout = NotNullRegexStringConstraintBase.DefaultRegexTimeout;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RegexStringConstraintBase" /> class
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
    protected RegexStringConstraintBase(
#if NET7_0_OR_GREATER
        [StringSyntax(StringSyntaxAttribute.Regex)]
#endif
        [NotNull] [RegexPattern] string pattern,
        RegexOptions options = DefaultRegexOptions,
        TimeSpan? timeout = null)
        : base(new NotNullRegexStringConstraint(pattern, options, timeout))
    {
        // Nothing to do
    }

    private protected override Type ActualConstraintType => typeof(NotNullRegexStringConstraint);

    private sealed class NotNullRegexStringConstraint : NotNullRegexStringConstraintBase
    {
        public NotNullRegexStringConstraint(
            string pattern,
            RegexOptions options = DefaultRegexOptions,
            TimeSpan? timeout = null)
            : base(pattern, options, timeout)
        {
            // Nothing to do
        }
    }
}