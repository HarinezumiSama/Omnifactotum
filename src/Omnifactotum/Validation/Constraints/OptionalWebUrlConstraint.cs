using System;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type may be <see langword="null"/>, but otherwise
///     should be an absolute URI using the <see cref="Uri.UriSchemeHttp"/> or <see cref="Uri.UriSchemeHttps"/> scheme.
/// </summary>
/// <seealso cref="NotNullWebUrlConstraint"/>
public sealed class OptionalWebUrlConstraint : CommonWebUrlConstraint
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OptionalWebUrlConstraint" /> class.
    /// </summary>
    public OptionalWebUrlConstraint()
        : base(true)
    {
        // Nothing to do
    }
}