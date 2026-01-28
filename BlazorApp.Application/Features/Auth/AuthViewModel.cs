namespace BlazorApp.Application.Features.Auth;

using System.Reactive.Linq;

public sealed class AuthViewModel
{
    public IObservable<bool> IsAuthenticated { get; }
    public IObservable<string?> Username { get; }
    public IObservable<bool> IsLoading { get; }

    public AuthViewModel(AuthStore store)
    {
        IsAuthenticated = store.State.Select(s => s.IsAuthenticated);
        Username = store.State.Select(s => s.UserName);
        IsLoading = store.State.Select(s => s.IsLoading);
    }
}
