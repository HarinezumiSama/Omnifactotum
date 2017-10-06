using System;

namespace Omnifactotum.Tests.Auxiliary
{
    public struct DisposableStruct : IDisposable
    {
        public event Action OnDispose;

        public void Dispose()
        {
            var onDispose = this.OnDispose;
            if (onDispose != null)
            {
                onDispose();
            }
        }
    }
}