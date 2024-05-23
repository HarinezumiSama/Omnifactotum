using System;
using System.Collections.Generic;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     The common base class for the Web URL constraints.
/// </summary>
public abstract class CommonWebUrlConstraint : TypedMemberConstraintBase<string?>
{
    private readonly bool _isOptional;

    private static readonly HashSet<string> AllowedSchemes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            Uri.UriSchemeHttp,
            Uri.UriSchemeHttps
        };

    private protected CommonWebUrlConstraint(bool isOptional) => _isOptional = isOptional;

    /// <inheritdoc />
    protected sealed override void ValidateTypedValue(MemberConstraintValidationContext memberContext, string? value)
    {
        if (value is null && _isOptional)
        {
            return;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || !AllowedSchemes.Contains(uri.Scheme))
        {
            AddError(memberContext, $"The value {FormatValue(value)} is not a valid Web URL.");
        }
    }
}