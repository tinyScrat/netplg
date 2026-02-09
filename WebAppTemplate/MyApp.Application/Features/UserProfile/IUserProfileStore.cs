namespace MyApp.Application.Features.User;

public interface IUserProfileStore
{
    IObservable<UserProfile?> Changes { get; }
    UserProfile? Current { get; }
    // void Set(UserProfile profile);
    // void Reset();
    bool IsAuthenticated { get; }
    bool IsAuthorizing { get; }
}
