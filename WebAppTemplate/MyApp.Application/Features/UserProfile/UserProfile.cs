namespace MyApp.Application.Features.User;

public sealed record BusinessUnit(string Id, string Name);

public sealed record UserProfile(
    string Username,
    string DisplayName,
    string Email,
    BusinessUnit BusinessUnit,
    IReadOnlySet<string> Roles,
    IReadOnlySet<string> Permissions,
    bool IsInitialized
)
{
    public static UserProfile FromDto(UserProfileDto dto)
    {
        return new UserProfile(
            Username: dto.Username,
            DisplayName: dto.DisplayName,
            Email: dto.Email,
            BusinessUnit: new BusinessUnit(dto.BusinessUnitId, dto.BusinessUnitName),
            Roles: dto.Roles,
            Permissions: dto.Permissions,
            IsInitialized: true
        );
    }
}
