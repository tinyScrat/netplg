namespace MyApp.Application.Features.Permission;

using System.Reactive.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyApp.Application.Abstractions;
using MyApp.Application.Features.Auth;

internal sealed class LoadPermissionEffect(
    ILogger<LoadPermissionEffect> logger,
    IAuthApi authApi) : IEffect<LoadPermissionCommand, IReadOnlySet<string>>
{
    public IObservable<IReadOnlySet<string>> Handle(LoadPermissionCommand command, CancellationToken ct)
    {
        logger.LogInformation("Loading permissions for current user");
        return authApi.GetPermissionsAsync().ToObservable();
    }
}
