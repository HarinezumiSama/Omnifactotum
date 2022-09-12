using System;

namespace Omnifactotum.Abstractions;

/// <summary>
///     Provides the current <see cref="DateTime"/>. This interface is typically used to avoid dependency on
///     <see cref="DateTime.Now"/> and <see cref="DateTime.UtcNow"/>.
/// </summary>
public interface ICurrentDateTimeProvider
{
    /// <summary>
    ///     Gets a <see cref="DateTime"/> object that is set to the current <b>UTC</b> date and time.
    /// </summary>
    /// <returns>
    ///     A <see cref="DateTime"/> object that is set to the current <b>UTC</b> date and time.
    /// </returns>
    DateTime GetUtcTime();
}