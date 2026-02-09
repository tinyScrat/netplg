namespace MyApp.WebUI.Components;

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;

public abstract class RxComponentBase : ComponentBase, IDisposable
{
    private readonly CompositeDisposable _disposables = [];
    private bool _disposed;

    protected IDisposable Track(IDisposable disposable)
    {
        if (_disposed)
        {
            disposable.Dispose();
            return disposable;
        }

        _disposables.Add(disposable);
        return disposable;
    }

    protected IDisposable Subscribe<T>(IObservable<T> source, Action<T>? onNext = null)
    {
        var sub = source.Subscribe(value =>
        {
            // Always marshal back to the Blazor sync context
            InvokeAsync(() =>
            {
                onNext?.Invoke(value);
                StateHasChanged();
            });
        });

        return Track(sub);
    }

    protected IDisposable SubscribeAsync<T>(
        IObservable<T> source,
        Func<T, Task> handler)
    {
        var sub = source
            .SelectMany(v => Observable.FromAsync(() => handler(v)))
            .Subscribe(_ => InvokeAsync(StateHasChanged));

        return Track(sub);
    }


    public virtual void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _disposables.Dispose();

        GC.SuppressFinalize(this);
    }
}
