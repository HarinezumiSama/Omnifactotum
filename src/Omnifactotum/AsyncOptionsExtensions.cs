using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnifactotum.Annotations;

namespace Omnifactotum
{
    internal static class AsyncOptionsExtensions
    {
        #region Public Methods

        public static CancellationToken GetCancellationToken([CanBeNull] this AsyncOptions options)
        {
            return options == null ? CancellationToken.None : options.CancellationToken;
        }

        public static TaskScheduler GetTaskScheduler([CanBeNull] this AsyncOptions options)
        {
            return options == null ? TaskScheduler.Current : options.TaskScheduler;
        }

        #endregion
    }
}