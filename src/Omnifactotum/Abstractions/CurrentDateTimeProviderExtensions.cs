using System;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Omnifactotum.Abstractions;

/// <summary>
///     Contains extension methods for the <see cref="ICurrentDateTimeProvider"/> interface.
/// </summary>
public static class CurrentDateTimeProviderExtensions
{
    /// <summary>
    ///     Gets a <see cref="DateTime"/> object that is set to the current <b>local</b> date and time.
    /// </summary>
    /// <returns>
    ///     A <see cref="DateTime"/> object that is set to the current <b>local</b> date and time.
    /// </returns>
    [Pure]
    [Omnifactotum.Annotations.Pure]
    public static DateTime GetLocalTime(this ICurrentDateTimeProvider currentDateTimeProvider)
    {
        if (currentDateTimeProvider is null)
        {
            throw new ArgumentNullException(nameof(currentDateTimeProvider));
        }

        var utcTime = currentDateTimeProvider.GetUtcTime();
        if (utcTime.Kind != DateTimeKind.Utc)
        {
            throw new InvalidOperationException($@"{currentDateTimeProvider.GetType().GetFullName()} returned the non-UTC {nameof(DateTime)} value.");
        }

        return utcTime.ToLocalTime();
    }
}