namespace MyApp.Application.Abstractions;

using System.Reactive.Disposables;

// UI → Dispatch(Command) → Effect → State update

public abstract class ViewModelBase : IDisposable
{
    private readonly CompositeDisposable _disposables = [];

    internal void AddDisposable(IDisposable disposable)
        => _disposables.Add(disposable);

    public virtual void Dispose()
        => _disposables.Dispose();
}

public static class DisposableExtensions
{
    public static T DisposeWith<T>(this T disposable, ViewModelBase vm)
        where T : IDisposable
    {
        vm.AddDisposable(disposable);
        return disposable;
    }
}
