using System.Threading;
using Cysharp.Threading.Tasks;

namespace SimpleObjectPool.Async{
    public interface IAsyncPooled{
        UniTask OnReleaseAsync(CancellationToken cancellationToken = default);
        UniTask OnTakeAsync(CancellationToken cancellationToken = default);
        UniTask OnClearAsync(CancellationToken cancellationToken = default);
    }
}
