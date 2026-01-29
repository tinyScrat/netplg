namespace BlazorApp.Application.Features.Auth;

using System.Reactive.Linq;

public sealed class AuthViewModel(AuthStore store)
{
    public IObservable<bool> IsAuthenticated { get; } = store.State.Select(s => s.IsAuthenticated);
    public IObservable<string?> Username { get; } = store.State.Select(s => s.UserName);
    public IObservable<bool> IsLoading { get; } = store.State.Select(s => s.IsLoading);
}
