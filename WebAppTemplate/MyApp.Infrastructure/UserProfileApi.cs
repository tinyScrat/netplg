namespace MyApp.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using MyApp.Application.Features.User;

public sealed class UserProfileApi(HttpClient http, ILogger<UserProfileApi> logger) : IUserProfileApi
{
    public async Task<UserProfileDto> GetUserProfileAsync()
    {
        await Task.Delay(1000); // Simulate network delay

        logger.LogInformation("User profile loaded");

        var dto = new UserProfileDto
        {
            Username = "johndoe",
            DisplayName = "John Doe",
            Email = "john.doe@example.com",
            BusinessUnitId = "bu-123",
            BusinessUnitName = "Sales",
            Roles = new HashSet<string> { "User", "Admin" },
            Permissions = new HashSet<string> { "Order.View", "Order.Create" }
        };

        return dto;
    }
}
