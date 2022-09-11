using System;

namespace Omnifactotum
{
    /// <summary>
    ///     The options specifying how <see cref="TemplatedStringResolver"/> should work.
    /// </summary>
    [Flags]
    public enum TemplatedStringResolverOptions
    {
        /// <summary>
        ///     <see cref="TemplatedStringResolver"/> works in the strict mode.
        /// </summary>
        None = 0,

        /// <summary>
        ///     <see cref="TemplatedStringResolver"/> should tolerate undefined template variables.
        /// </summary>
        /// <remarks>
        ///     An undefined template variable is resolved to <see cref="string.Empty"/>.
        /// </remarks>
        TolerateUndefinedVariables = 0b00000001,

        /// <summary>
        ///     <see cref="TemplatedStringResolver"/> should tolerate unexpected tokens.
        /// </summary>
        /// <remarks>
        ///     An unexpected token is resolved to itself.
        /// </remarks>
        TolerateUnexpectedTokens = 0b00000010
    }
}