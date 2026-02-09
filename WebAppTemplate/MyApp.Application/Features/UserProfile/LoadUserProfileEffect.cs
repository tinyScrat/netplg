namespace MyApp.Application.Features.User;

using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;

internal sealed class LoadUserProfileEffect(
    ILogger<LoadUserProfileEffect> logger,
    IUserProfileApi userProfileApiApi) : IEffect<LoadUserProfileCmd, UserProfile>
{
    public IObservable<UserProfile> Handle(LoadUserProfileCmd command, CancellationToken ct)
    {
        logger.LogInformation("Loading user profile for current user");
        return Observable.FromAsync(() => userProfileApiApi.GetUserProfileAsync())
            .Select(UserProfile.FromDto);
    }
}
