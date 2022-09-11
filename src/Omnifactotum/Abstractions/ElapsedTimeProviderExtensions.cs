using System;

namespace Omnifactotum.Abstractions
{
    /// <summary>
    ///     Contains extension methods for the <see cref="IElapsedTimeProvider"/> interface.
    /// </summary>
    public static class ElapsedTimeProviderExtensions
    {
        /// <summary>
        ///     Stops time interval measurement, resets the elapsed time to zero, and starts measuring elapsed time.
        /// </summary>
        public static void Restart(this IElapsedTimeProvider elapsedTimeProvider)
        {
            if (elapsedTimeProvider is null)
            {
                throw new ArgumentNullException(nameof(elapsedTimeProvider));
            }

            elapsedTimeProvider.Reset();
            elapsedTimeProvider.Start();
        }
    }
}