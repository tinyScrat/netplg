namespace MyApp.Application.Features.Auth;

public interface IAuthApi
{
    Task GetUserProfileAsync();
    Task GetPermissionsAsync();
}
