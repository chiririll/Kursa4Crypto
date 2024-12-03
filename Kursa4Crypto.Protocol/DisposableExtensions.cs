using System.Reactive.Disposables;

namespace Kursa4Crypto.Protocol;

public static class DisposableExtensions
{
    public static void AddTo(this IDisposable disposable, CompositeDisposable disp) => disp.Add(disposable);
}