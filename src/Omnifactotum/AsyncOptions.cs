using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    /// <summary>
    ///     Represents options of execution of the <see cref="M:AsyncFactotum.ComputeAsync"/> and
    ///     <see cref="M:AsyncFactotum.ExecuteAsync"/> methods.
    /// </summary>
    public sealed class AsyncOptions
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the cancellation token used to run an asynchronous operation.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the task scheduler used to run an asynchronous operation.
        /// </summary>
        [CanBeNull]
        public TaskScheduler TaskScheduler
        {
            get;
            set;
        }

        #endregion
    }
}