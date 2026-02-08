namespace MyApp.Application.Features.Permission;

using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Abstractions;

public static class PermissionFeatures
{
    public static IServiceCollection AddPermissionFeature(this IServiceCollection services)
    {
        services.AddSingleton<PermissionStore>();
        services.AddSingleton<IPermissionStore>(sp =>
            sp.GetRequiredService<PermissionStore>());

        services.AddSingleton<
            IEffect<LoadPermissionCommand, IReadOnlySet<string>>,
            LoadPermissionEffect>();

        services.AddSingleton<ICommandKey<LoadPermissionCommand>, PermissionCommandKey>();
        
        services.AddSingleton<ICommandPipeline<LoadPermissionCommand>>(sp =>
            {
                return new IdempotentCommandPipeline<LoadPermissionCommand, IReadOnlySet<string>>(
                    sp.GetRequiredService<IEffect<LoadPermissionCommand, IReadOnlySet<string>>>(),
                        perms =>
                            sp.GetRequiredService<PermissionStore>()
                                .SetPermissions(perms),
                    sp.GetRequiredService<ICommandKey<LoadPermissionCommand>>());
            });

        services.AddSingleton<AuthPermissionSyncSubscriber>();

        return services;
    }
}
