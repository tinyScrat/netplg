namespace MyApp.WebUI.Components;

using System;
using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;

public abstract class RxComponentBase : ComponentBase, IDisposable
{
    private readonly CompositeDisposable _disposables = [];
    private bool _disposed;

    protected void Track(IDisposable disposable)
    {
        if (_disposed)
        {
            disposable.Dispose();
            return;
        }

        _disposables.Add(disposable);
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

        Track(sub);

        return sub;
    }

    public virtual void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _disposables.Dispose();

        GC.SuppressFinalize(this);
    }
}
