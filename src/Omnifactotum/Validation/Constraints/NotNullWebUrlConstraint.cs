using System;

namespace Omnifactotum.Validation.Constraints;

/// <summary>
///     Specifies that the annotated member of the <see cref="string"/> type should not be <see langword="null"/> and
///     should be an absolute URI using the <see cref="Uri.UriSchemeHttp"/> or <see cref="Uri.UriSchemeHttps"/> scheme.
/// </summary>
/// <seealso cref="OptionalWebUrlConstraint"/>
public sealed class NotNullWebUrlConstraint : CommonWebUrlConstraint
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NotNullWebUrlConstraint" /> class.
    /// </summary>
    public NotNullWebUrlConstraint()
        : base(false)
    {
        // Nothing to do
    }
}