namespace MyApp.Application.Features.Auth;

using MyApp.Application.Abstractions;

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
    private readonly ReactiveState<AuthState> _state =
        new(AuthState.Anonymous);

    public IObservable<AuthState> State => _state.Changes;
    public AuthState Current => _state.Value;

    public void SetAuthenticated(
        string userName,
        IEnumerable<string> roles)
    {
        _state.Update( s => new AuthState(
            true,
            userName,
            [.. roles],
            false));
    }

    public void SetAnonymous()
        => _state.Update(s => AuthState.Anonymous with
        {
            IsLoading = false
        });
}
