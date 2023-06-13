using System;
using System.Collections.Generic;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="String"/> type should be an absolute URI using
///     the <see cref="Uri.UriSchemeHttp"/> or <see cref="Uri.UriSchemeHttps"/> scheme.
/// </summary>
public sealed class WebUrlConstraint : TypedMemberConstraintBase<string?>
{
    private static readonly HashSet<string> AllowedSchemes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            Uri.UriSchemeHttp,
            Uri.UriSchemeHttps
        };

    /// <inheritdoc />
    protected override void ValidateTypedValue(
        ObjectValidatorContext validatorContext,
        MemberConstraintValidationContext memberContext,
        string? value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || !AllowedSchemes.Contains(uri.Scheme))
        {
            AddError(validatorContext, memberContext, $@"The value {FormatValue(value)} is not a valid Web URL.");
        }
    }
}