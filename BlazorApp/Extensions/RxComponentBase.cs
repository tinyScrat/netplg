namespace BlazorApp.Extensions;

using System.Reactive.Disposables;
using Microsoft.AspNetCore.Components;

public abstract class RxComponentBase : ComponentBase, IDisposable
{
    protected readonly CompositeDisposable Disposables = new();

    public void Dispose() => Disposables.Dispose();
}
