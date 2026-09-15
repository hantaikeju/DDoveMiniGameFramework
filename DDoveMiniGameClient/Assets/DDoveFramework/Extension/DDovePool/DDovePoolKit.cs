using System;
using System.Collections.Generic;
using DDoveFramework.Core;
using UnityEngine;

namespace DDoveFramework.Extension.DDovePool
{
    public static class DDovePoolKit
    {
        public const string LogTitle = "DDovePool";
        public const int DefaultCapacity = 30;

        private static readonly Dictionary<Type, IDDoveObjectPool> Pools = new Dictionary<Type, IDDoveObjectPool>();
        private static readonly Dictionary<Type, IDDoveObjectPool> GameObjectPools = new Dictionary<Type, IDDoveObjectPool>();

        private static Transform _poolRoot;
        private static bool _ownsRoot;

        public static bool IsInitialized => _poolRoot != null;

        public static void Initialize()
        {
            if (_poolRoot != null)
            {
                return;
            }

            var go = new GameObject("[DDovePool]");
            UnityEngine.Object.DontDestroyOnLoad(go);
            _poolRoot = go.transform;
            _ownsRoot = true;
            go.AddComponent<DDovePoolLifetime>();
        }

        public static void Initialize(Transform poolRoot)
        {
            if (poolRoot == null)
            {
                throw new ArgumentNullException(nameof(poolRoot));
            }

            if (_poolRoot != null)
            {
                DDoveDebug.LogWarning(LogTitle, ("reason", "already initialized"));
                return;
            }

            _poolRoot = poolRoot;
            _ownsRoot = false;
        }

        public static void Shutdown()
        {
            ClearAll(Pools);
            ClearAll(GameObjectPools);

            if (_ownsRoot && _poolRoot != null)
            {
                UnityEngine.Object.Destroy(_poolRoot.gameObject);
            }

            _poolRoot = null;
            _ownsRoot = false;
        }

        public static T Get<T>() where T : class, IDDovePoolObject, new()
        {
            return GetOrCreatePool<T>(() => new T()).Get();
        }

        public static void Release<T>(T instance) where T : class, IDDovePoolObject, new()
        {
            if (instance == null)
            {
                return;
            }

            GetOrCreatePool<T>(() => new T()).Release(instance);
        }

        public static void Configure<T>(Func<T> factory, int capacity) where T : class, IDDovePoolObject
        {
            Pools[typeof(T)] = new DDoveObjectPool<T>(factory, capacity);
        }

        public static void Prewarm<T>(int count) where T : class, IDDovePoolObject, new()
        {
            GetOrCreatePool<T>(() => new T()).Prewarm(count);
        }

        public static void Clear<T>() where T : class, IDDovePoolObject
        {
            if (!Pools.TryGetValue(typeof(T), out var pool))
            {
                return;
            }

            pool.Clear();
            Pools.Remove(typeof(T));
        }

        public static bool TryGetPool<T>(out IDDoveObjectPool<T> pool) where T : class, IDDovePoolObject
        {
            if (Pools.TryGetValue(typeof(T), out var value) && value is IDDoveObjectPool<T> typed)
            {
                pool = typed;
                return true;
            }

            pool = null;
            return false;
        }

        public static T GetGameObject<T>() where T : class, IDDoveGameObjectPoolObject, new()
        {
            return GetOrCreateGameObjectPool<T>(() => new T()).Get();
        }

        public static void ReleaseGameObject<T>(T instance) where T : class, IDDoveGameObjectPoolObject, new()
        {
            if (instance == null)
            {
                return;
            }

            GetOrCreateGameObjectPool<T>(() => new T()).Release(instance);
        }

        public static void ConfigureGameObject<T>(Func<T> factory, int capacity) where T : class, IDDoveGameObjectPoolObject
        {
            GameObjectPools[typeof(T)] = new DDoveGameObjectPool<T>(factory, RequirePoolRoot(), capacity);
        }

        public static void PrewarmGameObject<T>(int count) where T : class, IDDoveGameObjectPoolObject, new()
        {
            GetOrCreateGameObjectPool<T>(() => new T()).Prewarm(count);
        }

        public static void ClearGameObject<T>() where T : class, IDDoveGameObjectPoolObject
        {
            if (!GameObjectPools.TryGetValue(typeof(T), out var pool))
            {
                return;
            }

            pool.Clear();
            GameObjectPools.Remove(typeof(T));
        }

        public static bool TryGetGameObjectPool<T>(out IDDoveGameObjectPool<T> pool)
            where T : class, IDDoveGameObjectPoolObject
        {
            if (GameObjectPools.TryGetValue(typeof(T), out var value) && value is IDDoveGameObjectPool<T> typed)
            {
                pool = typed;
                return true;
            }

            pool = null;
            return false;
        }

        public static GameObject GetPoolGameObjectRoot<T>() where T : class, IDDoveGameObjectPoolObject
        {
            if (GameObjectPools.TryGetValue(typeof(T), out var value) && value is IDDoveGameObjectPool<T> goPool)
            {
                return goPool.GetOrAddGameObjectPoolRoot();
            }

            return null;
        }

        private static DDoveObjectPool<T> GetOrCreatePool<T>(Func<T> factory) where T : class, IDDovePoolObject
        {
            if (Pools.TryGetValue(typeof(T), out var existing))
            {
                return (DDoveObjectPool<T>)existing;
            }

            var pool = new DDoveObjectPool<T>(factory, DefaultCapacity);
            Pools[typeof(T)] = pool;
            return pool;
        }

        private static DDoveGameObjectPool<T> GetOrCreateGameObjectPool<T>(Func<T> factory)
            where T : class, IDDoveGameObjectPoolObject
        {
            if (GameObjectPools.TryGetValue(typeof(T), out var existing))
            {
                return (DDoveGameObjectPool<T>)existing;
            }

            var pool = new DDoveGameObjectPool<T>(factory, RequirePoolRoot(), DefaultCapacity);
            GameObjectPools[typeof(T)] = pool;
            return pool;
        }

        private static Transform RequirePoolRoot()
        {
            if (_poolRoot != null)
            {
                return _poolRoot;
            }

            DDoveDebug.LogError(LogTitle, ("reason", "not initialized"));
            throw new InvalidOperationException("DDovePoolKit 未初始化，请先调用 DDovePoolKit.Initialize()");
        }

        private static void ClearAll(Dictionary<Type, IDDoveObjectPool> pools)
        {
            foreach (var pair in pools)
            {
                pair.Value.Clear();
            }

            pools.Clear();
        }

        private sealed class DDovePoolLifetime : MonoBehaviour
        {
            private void OnApplicationQuit()
            {
                Shutdown();
            }
        }
    }
}
