namespace MyApp.Application.Features.User;

public sealed class UserProfileDto
{
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public required string Email { get; init; }
    public required string BusinessUnitId { get; init; }
    public required string BusinessUnitName { get; init; }
    public required IReadOnlySet<string> Roles { get; init; }
    public required IReadOnlySet<string> Permissions { get; init; }
}

public interface IUserProfileApi
{
    Task<UserProfileDto> GetUserProfileAsync();
}
