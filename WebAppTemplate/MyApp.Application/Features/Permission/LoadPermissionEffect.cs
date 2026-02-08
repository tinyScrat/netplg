namespace MyApp.Application.Features.Permission;

using System.Reactive.Threading.Tasks;
using MyApp.Application.Abstractions;
using MyApp.Application.Features.Auth;

internal sealed class LoadPermissionEffect(IAuthApi authApi) : IEffect<LoadPermissionCommand, IReadOnlySet<string>>
{
    public IObservable<IReadOnlySet<string>> Handle(LoadPermissionCommand command, CancellationToken ct)
    {
        return authApi.GetPermissionsAsync().ToObservable();
    }
}
