namespace BlazorApp.Application;

public interface ITokenStorage
{
    Task SaveAsync(string token, DateTimeOffset expiresAt);
    Task<(string?, DateTimeOffset?)> LoadAsync();
    Task ClearAsync();
}
