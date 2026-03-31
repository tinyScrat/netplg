namespace Lpc.Application.Features.User;

public sealed class UserProfileDto
{
    public required string Username { get; init; }
    public required string DisplayName { get; init; }
    public required string Email { get; init; }
    public required string BusinessUnitId { get; init; }
    public required string BusinessUnitName { get; init; }
    public required IReadOnlyCollection<string> Roles { get; init; }
    public required IReadOnlyCollection<string> Permissions { get; init; }
}

public interface IUserProfileApi
{
    Task<UserProfileDto> GetUserProfileAsync();
}
