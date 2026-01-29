namespace BlazorApp.Features.Auth;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

public sealed class CustomPrincipalFactory(IAccessTokenProviderAccessor accessor)
        : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor)
{
    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);

        if (user.Identity is not ClaimsIdentity identity ||
            !identity.IsAuthenticated)
        {
            return user;
        }

        // ----- Name -----
        identity.AddClaim(new Claim(
            ClaimTypes.Name,
            account.AdditionalProperties.TryGetValue("name", out var name)
                ? name!.ToString()!
                : account.AdditionalProperties.TryGetValue("preferred_username", out var username)
                    ? username!.ToString()!
                    : identity.FindFirst(ClaimTypes.NameIdentifier)!.Value));

        // ----- Email -----
        if (account.AdditionalProperties.TryGetValue("email", out var email))
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, email!.ToString()!));
        }

        // ----- Roles (Ping) -----
        if (account.AdditionalProperties.TryGetValue("roles", out var roles) &&
            roles is IEnumerable<object> roleValues)
        {
            foreach (var role in roleValues)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()!));
            }
        }

        // ----- Groups (optional) -----
        if (account.AdditionalProperties.TryGetValue("groups", out var groups) &&
            groups is IEnumerable<object> groupValues)
        {
            foreach (var group in groupValues)
            {
                identity.AddClaim(new Claim("group", group.ToString()!));
            }
        }

        return user;
    }
}
