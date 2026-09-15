using System;
using UnityEngine;

namespace DDoveFramework.Extension.DDovePool
{
    public interface IDDovePoolObject
    {
        void OnAcquire();
        void OnRelease();
    }

    public interface IDDoveGameObjectPoolObject : IDDovePoolObject
    {
        GameObject GetGameObject();
    }

    public interface IDDoveObjectPool
    {
        Type ItemType { get; }
        void Clear();
    }

    public interface IDDoveObjectPool<T> : IDDoveObjectPool where T : class, IDDovePoolObject
    {
        T Get();
        void Release(T instance);
        int Count { get; }
        int Capacity { get; }
        void Prewarm(int count);
    }

    public interface IDDoveGameObjectPool<T> : IDDoveObjectPool<T> where T : class, IDDoveGameObjectPoolObject
    {
        GameObject GetOrAddGameObjectPoolRoot();
    }
}
