namespace MyApp.Infrastructure.Features.Auth;

using MyApp.Application.Features.Auth;

internal sealed class AuthApi : IAuthApi
{
    public Task GetUserProfileAsync()
    {
        // Implement the logic to get user profile from the API
        // endpoint: GET /auth/me
        return Task.CompletedTask;
    }

    public Task<IReadOnlySet<string>> GetPermissionsAsync()
    {
        // Implement the logic to get user permissions from the API
        // endpoint: GET /auth/me/permissions
        var permissions = new HashSet<string>
        {
            "Order.View",
            // "order:create",
            // "product:view",
            // Add more permissions as needed
        };
        return Task.FromResult<IReadOnlySet<string>>(permissions);
    }
}
