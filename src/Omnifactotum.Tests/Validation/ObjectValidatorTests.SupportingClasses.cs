using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Omnifactotum.NUnit;

namespace Omnifactotum.Tests.Validation;

internal sealed partial class ObjectValidatorTests
{
    private sealed class CustomReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly T[] _items;

        public CustomReadOnlyList(params T[] items) => _items = items.AssertNotNull().ToArray();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator() => throw new NotSupportedException();

        int IReadOnlyCollection<T>.Count => _items.Length;

        public T this[int index] => _items[index];
    }

    private sealed class CustomList : IList
    {
        private readonly object?[] _items;

        public CustomList(params object?[] items) => _items = items.AssertNotNull().ToArray();

        int ICollection.Count => _items.Length;

        public object? this[int index]
        {
            get => _items[index];
            set => throw new NotSupportedException();
        }

        public bool IsSynchronized => throw new NotSupportedException();

        public object SyncRoot => throw new NotSupportedException();

        public bool IsFixedSize => throw new NotSupportedException();

        public bool IsReadOnly => throw new NotSupportedException();

        public IEnumerator GetEnumerator() => throw new NotSupportedException();

        public void CopyTo(Array array, int index) => throw new NotSupportedException();

        public int Add(object? value) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(object? value) => throw new NotSupportedException();

        public int IndexOf(object? value) => throw new NotSupportedException();

        public void Insert(int index, object? value) => throw new NotSupportedException();

        public void Remove(object? value) => throw new NotSupportedException();

        public void RemoveAt(int index) => throw new NotSupportedException();
    }

    private sealed class CustomGenericList<T> : IList<T>
    {
        private readonly T[] _items;

        public CustomGenericList(params T[] items) => _items = items.AssertNotNull().ToArray();

        public int Count => _items.Length;

        public bool IsReadOnly => throw new NotSupportedException();

        public T this[int index]
        {
            get => _items[index];
            set => throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator() => throw new NotSupportedException();

        public void Add(T item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(T item) => throw new NotSupportedException();

        public void CopyTo(T[] array, int arrayIndex) => throw new NotSupportedException();

        public bool Remove(T item) => throw new NotSupportedException();

        public int IndexOf(T item) => throw new NotSupportedException();

        public void Insert(int index, T item) => throw new NotSupportedException();

        public void RemoveAt(int index) => throw new NotSupportedException();
    }

    private sealed class CustomEnumerable : IEnumerable
    {
        private readonly object?[] _items;

        public CustomEnumerable(params object?[] items) => _items = items.AssertNotNull().ToArray();

        public IEnumerator GetEnumerator() => _items.GetEnumerator();
    }

    private sealed class CustomGenericEnumerable<T> : IEnumerable<T>
    {
        private readonly T[] _items;

        public CustomGenericEnumerable(params T[] items) => _items = items.AssertNotNull().ToArray();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();
    }
}