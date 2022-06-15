using System;
using System.Collections.Generic;

namespace SimpleObjectPool
{
    public class ObjectPool<T> : IDisposable where T : IPooled
    {
        private Func<T> createInstance;
        private ObjectPoolParameter parameter;
        private Queue<T> pool;
        public int Count => pool?.Count ?? 0;
        public int MaxPoolCount => parameter?.maxPoolCount ?? 0;
        private bool disposed;

        public ObjectPool(Func<T> createInstance,ObjectPoolParameter parameter = null)
        {
            this.createInstance = createInstance;
            this.parameter = parameter;
        }

        /// <summary>
        /// You can resetting parameter.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void PreLoad(ObjectPoolParameter parameter = null)
        {
            if (disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if(pool.Count != 0) throw new InvalidOperationException("The object pool has already been initialized.");

            if(parameter != null) this.parameter = parameter;
			if(parameter == null) throw new NullReferenceException("Parameter undefined.");

            var count = this.parameter.preLoadCount;
            for(var i = 0; i < count; i++)
            {
               var instance = createInstance();
               Release(instance);
            }
        }

        /// <summary>
        /// Return instances to the pool.
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Release(T instance)
        {
            if (disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (instance == null) throw new ArgumentNullException();
            
            if (pool == null) pool = new Queue<T>();
            
            if (parameter.maxPoolCount < 1 || parameter.maxPoolCount == pool.Count + 1)
            {
                throw new InvalidOperationException("Reached max pool size.");
            }
            
            instance.OnRelease();
            pool.Enqueue(instance);
        }

        /// <summary>
        /// Take instances from the pool.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public T Take()
        {
            if (disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            
            if (pool == null) pool = new Queue<T>();
            
            var instance = pool.Count > 0 ? pool.Dequeue() : createInstance();
            instance.OnTake();
            return instance;
        }

        /// <summary>
        /// Empty the pool.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public void Clear()
        {
            if (disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if (pool == null) return;
            
            while (pool.Count != 0)
            {
                var instance = pool.Dequeue();
                instance.OnClear();
            }
        }

        public void Dispose()
        {
            Clear();
            disposed = true;
        }
    }
}
