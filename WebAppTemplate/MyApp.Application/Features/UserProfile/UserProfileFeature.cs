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

        services.AddIdempotentCommand<
            LoadUserProfileCmd,
            LoadUserProfileCmdKey,
            LoadUserProfileEffect,
            UserProfile,
            UserProfileResultHandler>();

        // Subscriber is singleton (long-lived event subscription)
        services.AddSingleton<UserProfileAuthStateSubscriber>();

        return services;
    }

    public static IServiceProvider UseUserProfileFeature(this IServiceProvider sp)
    {
        _ = sp.GetRequiredService<UserProfileAuthStateSubscriber>();

        return sp;
    }
}
