using System;
using System.Collections.Generic;

namespace DDoveFramework.Core
{
    /// <summary>
    /// 按注册时的泛型类型存一份实例。不是构造注入容器。
    /// </summary>
    public class IOCContainer
    {
        private readonly Dictionary<Type, object> mInstances = new Dictionary<Type, object>();

        public void Register<T>(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            mInstances[typeof(T)] = instance;
        }

        public T Get<T>() where T : class
        {
            if (TryGet<T>(out var instance))
            {
                return instance;
            }

            throw new InvalidOperationException($"[IOC] 未注册: {typeof(T).FullName}");
        }

        public bool TryGet<T>(out T instance) where T : class
        {
            if (mInstances.TryGetValue(typeof(T), out var obj) && obj is T typed)
            {
                instance = typed;
                return true;
            }

            instance = null;
            return false;
        }

        public bool Contains<T>()
        {
            return mInstances.ContainsKey(typeof(T));
        }

        public void Unregister<T>()
        {
            mInstances.Remove(typeof(T));
        }

        public void Clear()
        {
            mInstances.Clear();
        }
    }
}
