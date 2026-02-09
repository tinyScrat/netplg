namespace MyApp.Application.Features.User;

using MyApp.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public static class UserProfileFeatures
{
    public static IServiceCollection AddUserProfileFeature(this IServiceCollection services)
    {
        services.AddSingleton<UserProfileStore>();
        services.AddSingleton<IUserProfileStore>(sp =>
            sp.GetRequiredService<UserProfileStore>());

        services.AddSingleton<IEffect<LoadUserProfileCmd, UserProfile>, LoadUserProfileEffect>();

        services.AddSingleton<ICommandKey<LoadUserProfileCmd>, LoadUserProfileCmdKey>();

        services.AddSingleton<ICommandPipeline<LoadUserProfileCmd>>(sp =>
            {
                return new IdempotentCommandPipeline<LoadUserProfileCmd, UserProfile>(
                    sp.GetRequiredService<IEffect<LoadUserProfileCmd, UserProfile>>(),
                        profile =>
                            sp.GetRequiredService<UserProfileStore>()
                                .SetProfile(profile),
                    sp.GetRequiredService<ICommandKey<LoadUserProfileCmd>>());
            });

        services.AddSingleton<UserProfileAuthStateSubscriber>();

        return services;
    }
}
