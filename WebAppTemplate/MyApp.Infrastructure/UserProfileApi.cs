namespace MyApp.Infrastructure.Services;

using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using MyApp.Application.Features.User;

public sealed class UserProfileApi(HttpClient http, ILogger<UserProfileApi> logger) : IUserProfileApi
{
    public async Task<UserProfileDto> GetUserProfileAsync()
    {
        var dto = await http.GetFromJsonAsync<UserProfileDto>("/data/profile.json"); // Simulate API call

        if (dto == null)
        {
            logger.LogWarning("Failed to load user profile");
            throw new InvalidOperationException("Failed to load user profile");
        }

        logger.LogInformation("User profile loaded");

        return dto;
    }
}
