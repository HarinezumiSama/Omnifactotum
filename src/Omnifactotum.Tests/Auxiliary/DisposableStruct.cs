using System;

namespace Omnifactotum.Tests.Auxiliary
{
    internal struct DisposableStruct : IDisposable
    {
        public event Action? OnDispose;

        public void Dispose() => OnDispose?.Invoke();
    }
}