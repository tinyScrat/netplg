namespace MyApp.Application.Features.User;

using MyApp.Application.Abstractions;

public sealed class UserProfileStore : IUserProfileStore
{
    private readonly AsyncState<UserProfile?> _state = new(null);

    public IObservable<UserProfile?> Changes => _state.Data.Changes;
    public UserProfile? Current => _state.Data.Value;
    public bool IsAuthenticated => Current != null;
    public bool IsAuthorizing => _state.Status.Value.IsLoading();

    public void SetUnauthenticated()
        => _state.Data.Update(_ => null);

    public void SetProfile(UserProfile profile)
        => _state.Data.Update(_ => profile);
}
