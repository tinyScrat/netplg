namespace BlazorApp.Application.Features.Auth;

using System.Reactive.Subjects;

public sealed record AuthState(
    bool IsAuthenticated,
    string? UserName,
    IReadOnlyList<string> Roles,
    bool IsLoading)
{
    public static AuthState Anonymous =>
        new(false, null, [], true);
}


public sealed class AuthStore
{
    private readonly BehaviorSubject<AuthState> _state =
        new(AuthState.Anonymous);

    public IObservable<AuthState> State => _state;

    public void SetAuthenticated(
        string userName,
        IEnumerable<string> roles)
    {
        _state.OnNext(new AuthState(
            true,
            userName,
            roles.ToList(),
            false));
    }

    public void SetAnonymous()
        => _state.OnNext(AuthState.Anonymous with
        {
            IsLoading = false
        });
}
