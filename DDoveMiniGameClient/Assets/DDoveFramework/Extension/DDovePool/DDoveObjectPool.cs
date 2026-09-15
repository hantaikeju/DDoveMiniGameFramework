using System;
using System.Collections.Generic;

namespace DDoveFramework.Extension.DDovePool
{
    internal sealed class DDoveObjectPool<T> : IDDoveObjectPool<T> where T : class, IDDovePoolObject
    {
        private readonly Stack<T> _items;
        private readonly HashSet<T> _inPool;
        private readonly Func<T> _factory;
        private readonly int _capacity;

        public DDoveObjectPool(Func<T> factory, int capacity = DDovePoolKit.DefaultCapacity)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _capacity = capacity;
            _items = new Stack<T>(capacity);
            _inPool = new HashSet<T>(capacity);
        }

        public int Count => _items.Count;
        public int Capacity => _capacity;
        public Type ItemType => typeof(T);

        public T Get()
        {
            T instance;
            if (_items.Count > 0)
            {
                instance = _items.Pop();
                _inPool.Remove(instance);
            }
            else
            {
                instance = _factory();
            }

            instance.OnAcquire();
            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null)
            {
                return;
            }

            if (_inPool.Contains(instance))
            {
                return;
            }

            instance.OnRelease();
            if (_items.Count >= _capacity)
            {
                return;
            }

            _inPool.Add(instance);
            _items.Push(instance);
        }

        public void Prewarm(int count)
        {
            var toCreate = Math.Min(count, _capacity - _items.Count);
            for (var i = 0; i < toCreate; i++)
            {
                var instance = _factory();
                instance.OnRelease();
                _inPool.Add(instance);
                _items.Push(instance);
            }
        }

        public void Clear()
        {
            _items.Clear();
            _inPool.Clear();
        }
    }
}
