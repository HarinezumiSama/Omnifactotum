#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnifactotum.Threading
{
    /// <summary>
    ///     Represents a <see cref="SemaphoreSlim"/> based lock that limits the number of threads that can access a resource or pool of
    ///     resources concurrently.
    /// </summary>
    /// <example>
    ///     <code>
    ///         private readonly SemaphoreSlimBasedLock _lock;
    ///         // ...
    ///         using (_lock.Acquire())
    ///         {
    ///             // Execute thread-safe/task-safe operation(s)
    ///         }
    ///     </code>
    /// </example>
    /// <example>
    ///     <code>
    ///         private readonly SemaphoreSlimBasedLock _lock;
    ///         // ...
    ///         using (await _lock.AcquireAsync(cancellationToken))
    ///         {
    ///             // Execute thread-safe/task-safe operation(s)
    ///         }
    ///     </code>
    /// </example>
    /// <seealso cref="IDisposable"/>
    public sealed class SemaphoreSlimBasedLock : IDisposable
    {
        private const int SingleThreadAccessCount = 1;

        private readonly Action _releaseCachedDelegate;

        private SemaphoreSlim? _semaphoreSlim;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreSlimBasedLock"/> class, specifying the number of requests that
        ///     can be granted concurrently.
        /// </summary>
        /// <param name="count">
        ///     The number of requests that can be granted concurrently.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="count"/> is equal to or less than 0.
        /// </exception>
        public SemaphoreSlimBasedLock(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, @"The value must be greater than zero.");
            }

            _semaphoreSlim = new SemaphoreSlim(count, count);
            Count = count;
            _releaseCachedDelegate = Release;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SemaphoreSlimBasedLock"/> class, specifying that only a single request
        ///     can be granted concurrently.
        /// </summary>
        public SemaphoreSlimBasedLock()
            : this(SingleThreadAccessCount)
        {
            // Nothing to do
        }

        /// <summary>
        ///     Gets the number of requests that can be granted concurrently.
        /// </summary>
        public int Count { get; }

        private SemaphoreSlim UnderlyingSemaphore => _semaphoreSlim ?? throw new ObjectDisposedException(GetType().FullName);

        /// <summary>
        ///     Acquires the lock by blocking the current thread until it can can access a resource or pool of resources, while observing
        ///     a <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken"/> to observe.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IDisposable"/> that the caller disposes of to release the lock.
        /// </returns>
        public IDisposable Acquire(CancellationToken cancellationToken = default)
        {
            UnderlyingSemaphore.Wait(cancellationToken);
            return new LockHolder(_releaseCachedDelegate);
        }

        /// <summary>
        ///     Asynchronously acquires access to a resource or pool of resources, while observing a <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="cancellationToken">
        ///     The <see cref="CancellationToken"/> to observe.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IDisposable"/> that the caller disposes of to release the lock.
        /// </returns>
        public async Task<IDisposable> AcquireAsync(CancellationToken cancellationToken = default)
        {
            await UnderlyingSemaphore.WaitAsync(cancellationToken);
            return new LockHolder(_releaseCachedDelegate);
        }

        /// <summary>
        ///     Disposes of the underlying <see cref="SemaphoreSlim"/>.
        /// </summary>
        public void Dispose() => Interlocked.Exchange(ref _semaphoreSlim, null)?.Dispose();

        private void Release()
        {
            try
            {
                _semaphoreSlim?.Release();
            }
            catch (ObjectDisposedException)
            {
                // The underlying semaphore was disposed of right after a reference was obtained but before its Release method was called
            }
        }

        /// <summary>
        ///     Provides means to release the lock acquired by the respective <see cref="SemaphoreSlimBasedLock"/>, using
        ///     the <see cref="IDisposable"/> pattern.
        /// </summary>
        private sealed class LockHolder : IDisposable
        {
            private Action? _onDispose;

            internal LockHolder(Action onDispose) => _onDispose = onDispose ?? throw new ArgumentNullException(nameof(onDispose));

            /// <summary>
            ///     Releases the lock acquired by the respective <see cref="SemaphoreSlimBasedLock"/>.
            /// </summary>
            public void Dispose() => Interlocked.Exchange(ref _onDispose, null)?.Invoke();
        }
    }
}