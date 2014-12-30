using System;
using System.Linq;

namespace Omnifactotum.Tests.Auxiliary
{
    public struct DisposableStruct : IDisposable
    {
        #region Events

        public event Action OnDispose;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            var onDispose = this.OnDispose;
            if (onDispose != null)
            {
                onDispose();
            }
        }

        #endregion
    }
}