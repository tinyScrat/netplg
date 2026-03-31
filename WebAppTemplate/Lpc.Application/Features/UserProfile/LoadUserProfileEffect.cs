namespace Lpc.Application.Features.User;

using System.Reactive.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Lpc.Application.Abstractions;

// 1. command
internal sealed record LoadUserProfileCmd(ClaimsPrincipal User) : ICommand<UserProfile>;

// 2. command key provider for idempotency
internal sealed class LoadUserProfileCmdKey : ICommandKey<LoadUserProfileCmd>
{
    public string GetKey(LoadUserProfileCmd cmd)
    {
        var userId = cmd.User.FindFirst("uid")?.Value
             ?? cmd.User.Identity?.Name
             ?? Guid.NewGuid().ToString("N");

        return $"userprofile:{userId}".ToLowerInvariant();
    }
}

// 3. effect that performs the side effect of the command (e.g. API call)
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

// 4. result handler that takes the result of the effect and do something with it (e.g. update the store)
internal sealed class UserProfileResultHandler(UserProfileStore store) : ICommandResultHandler<UserProfile>
{
    public Task HandleAsync(UserProfile result, CancellationToken ct)
    {
        store.SetProfile(result);
        return Task.CompletedTask;
    }
}
