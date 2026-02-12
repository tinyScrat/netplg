namespace MyApp.WebUI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MyApp.WebUI.Abstractions;

public abstract class RxComponentBase : ComponentBase, IDisposable
{
    private readonly CompositeDisposable _disposables = [];
    private bool _disposed;
    private readonly List<ViewModelBase> _boundViewModels = [];

    #region Binding ViewModels

    /// <summary>
    /// Bind one or more ViewModels. Auto-subscribes all public IObservable&lt;T&gt; properties.
    /// Must be called explicitly in OnInitialized (trim-safe).
    /// </summary>
    protected void Bind(params ViewModelBase[] viewModels)
    {
        foreach (var vm in viewModels)
        {
            if (vm == null) throw new ArgumentNullException(nameof(vm));
            if (_boundViewModels.Contains(vm)) continue;

            _boundViewModels.Add(vm);
            vm.StateChanged += OnViewModelStateChanged;

            AutoSubscribeObservables(vm);
        }
    }

    /// <summary>
    /// Automatically subscribes all public IObservable&lt;T&gt; properties in a ViewModel.
    /// </summary>
    private void AutoSubscribeObservables(ViewModelBase vm)
    {
        var observableProps = vm.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType &&
                        typeof(IObservable<>).IsAssignableFrom(p.PropertyType.GetGenericTypeDefinition()));

        foreach (var prop in observableProps)
        {
            var obs = prop.GetValue(vm);
            if (obs == null) continue;

            var observableType = prop.PropertyType.GetGenericArguments()[0];
            var subscribeMethod = typeof(RxComponentBase)
                .GetMethod(nameof(SubscribeDynamic), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(observableType);

            subscribeMethod.Invoke(this, [obs]);
        }
    }

    /// <summary>
    /// Internal helper to subscribe dynamically to IObservable&lt;T&gt;.
    /// </summary>
    private void SubscribeDynamic<T>(IObservable<T> source)
    {
        var sub = source.Subscribe(_ => InvokeAsync(StateHasChanged));
        Track(sub);
    }

    private void OnViewModelStateChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    #endregion

    #region Manual Subscriptions

    /// <summary>
    /// Manually subscribe to an observable with optional onNext and onError callbacks.
    /// </summary>
    protected IDisposable Subscribe<T>(
        IObservable<T> source,
        Action<T>? onNext = null,
        Action<Exception>? onError = null)
    {
        var sub = source.Subscribe(
            value => InvokeAsync(() =>
            {
                onNext?.Invoke(value);
                StateHasChanged();
            }),
            ex => onError?.Invoke(ex));

        return Track(sub);
    }

    /// <summary>
    /// Manually subscribe to an observable with async handler, safe cancellation, and error handling.
    /// </summary>
    protected IDisposable SubscribeAsync<T>(
        IObservable<T> source,
        Func<T, CancellationToken, Task> handler,
        Action<Exception>? onError = null)
    {
        var cts = new CancellationTokenSource();

        var sub = source
            .Select(v => Observable.FromAsync(ct => handler(v, ct)))
            .Switch() // cancels previous async if new value arrives
            .Subscribe(
                _ => InvokeAsync(StateHasChanged),
                ex =>
                {
                    onError?.Invoke(ex);
                    InvokeAsync(StateHasChanged);
                });

        return Track(Disposable.Create(() =>
        {
            cts.Cancel();
            cts.Dispose();
            sub.Dispose();
        }));
    }

    #endregion

    #region Disposal & Tracking

    /// <summary>
    /// Tracks disposables to be automatically disposed when the component is disposed.
    /// </summary>
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

    public virtual void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        foreach (var vm in _boundViewModels)
        {
            vm.StateChanged -= OnViewModelStateChanged;
        }

        _disposables.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}
