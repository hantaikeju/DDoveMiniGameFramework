using System;
using System.Collections.Generic;
using UnityEngine;

namespace DDoveFramework.Extension.DDovePool
{
    internal sealed class DDoveGameObjectPool<T> : IDDoveGameObjectPool<T> where T : class, IDDoveGameObjectPoolObject
    {
        private readonly Stack<T> _items;
        private readonly HashSet<T> _inPool;
        private readonly Func<T> _factory;
        private readonly int _capacity;
        private readonly Transform _globalPoolRoot;
        private GameObject _poolRoot;

        public DDoveGameObjectPool(Func<T> factory, Transform globalPoolRoot, int capacity = DDovePoolKit.DefaultCapacity)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _globalPoolRoot = globalPoolRoot ?? throw new ArgumentNullException(nameof(globalPoolRoot));
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
            var go = instance.GetGameObject();
            if (_items.Count >= _capacity)
            {
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                }

                return;
            }

            Park(instance, go, GetOrAddGameObjectPoolRoot().transform);
        }

        public void Prewarm(int count)
        {
            var toCreate = Math.Min(count, _capacity - _items.Count);
            var root = GetOrAddGameObjectPoolRoot().transform;
            for (var i = 0; i < toCreate; i++)
            {
                var instance = _factory();
                instance.OnRelease();
                Park(instance, instance.GetGameObject(), root);
            }
        }

        public void Clear()
        {
            while (_items.Count > 0)
            {
                var instance = _items.Pop();
                var go = instance.GetGameObject();
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                }
            }

            _inPool.Clear();
            if (_poolRoot != null)
            {
                UnityEngine.Object.Destroy(_poolRoot);
                _poolRoot = null;
            }
        }

        public GameObject GetOrAddGameObjectPoolRoot()
        {
            if (_poolRoot != null)
            {
                return _poolRoot;
            }

            _poolRoot = new GameObject("[Pool] " + typeof(T).Name);
            var transform = _poolRoot.transform;
            transform.SetParent(_globalPoolRoot);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            return _poolRoot;
        }

        private void Park(T instance, GameObject go, Transform root)
        {
            if (go != null)
            {
                go.SetActive(false);
                go.transform.SetParent(root);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
            }

            _inPool.Add(instance);
            _items.Push(instance);
        }
    }
}
