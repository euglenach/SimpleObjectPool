using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SimpleObjectPool.Async{
    public class AsyncObjectPool<T> : IUniTaskAsyncDisposable where T : IAsyncPooled{
        private Func<CancellationToken,UniTask<T>> createInstance;
        private ObjectPoolParameter parameter;
        private Queue<T> pool = new Queue<T>();
        public int Count => pool?.Count ?? 0;
        public int MaxPoolCount => parameter?.maxPoolCount ?? 0;
        private bool disposed;

        public AsyncObjectPool(Func<CancellationToken,UniTask<T>> createInstance, ObjectPoolParameter parameter = null){
            this.createInstance = createInstance;
            this.parameter = parameter;
        }

        /// <summary>
        /// You can resetting parameter.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public UniTask PreLoadAsync(ObjectPoolParameter parameter = null,CancellationToken cancellationToken = default){
            if(disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if(pool.Count != 0) throw new InvalidOperationException("The object pool has already been initialized.");

            if(parameter != null) this.parameter = parameter;
            if(parameter == null) throw new NullReferenceException("Parameter undefined.");

            var count = this.parameter.preLoadCount;
            
            return UniTask.WhenAll(Enumerable.Range(0, count).Select(_ => createInstance(cancellationToken)));
        }

        /// <summary>
        /// Return instances to the pool.
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async UniTask ReleaseAsync(T instance,CancellationToken cancellationToken = default){
            if(disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if(instance == null) throw new ArgumentNullException();

            if(pool == null) pool = new Queue<T>();

            if(parameter.maxPoolCount < 1 || parameter.maxPoolCount < pool.Count + 1){
                throw new InvalidOperationException("Reached max pool size.");
            }

            await instance.OnReleaseAsync(cancellationToken);
            pool.Enqueue(instance);
        }

        /// <summary>
        /// Take instances from the pool.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public async UniTask<T> TakeAsync(CancellationToken cancellationToken = default){
            if(disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");

            if(pool == null) pool = new Queue<T>();

            var instance = pool.Count > 0? pool.Dequeue() : await createInstance(cancellationToken);
            instance.OnTakeAsync(cancellationToken);
            return instance;
        }

        /// <summary>
        /// Empty the pool.
        /// </summary>
        /// <exception cref="ObjectDisposedException"></exception>
        public async UniTask ClearAsync(CancellationToken cancellationToken = default){
            if(disposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
            if(pool == null) return;
            
            await UniTask.WhenAll(pool.Select(item => item.OnClearAsync(cancellationToken)));
            pool.Clear();
        }

        public async UniTask DisposeAsync(){
            await ClearAsync();
            disposed = true;
        }
    }
}
