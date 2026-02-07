namespace MyApp.Application.Features.Permission;

using Microsoft.Extensions.DependencyInjection;

public static class PermissionFeatures
{
    public static IServiceCollection AddPermissionFeature(this IServiceCollection services)
    {
        services.AddSingleton<PermissionStore>();
        services.AddSingleton<IPermissionStore>(sp =>
            sp.GetRequiredService<PermissionStore>());

        return services;
    }
}
