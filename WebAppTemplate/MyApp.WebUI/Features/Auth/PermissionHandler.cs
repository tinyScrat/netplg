namespace MyApp.WebUI.Features.Auth;

using Microsoft.AspNetCore.Authorization;
using MyApp.Application.Features.Permission;

public sealed class PermissionHandler(
    IPermissionStore permissionStore ,
    ILogger<PermissionHandler> logger) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Not authenticated → fail fast
        if (context.User.Identity?.IsAuthenticated != true)
            return Task.CompletedTask;

        // Permissions not loaded yet
        // IMPORTANT: do NOT succeed and do NOT throw
        if (!permissionStore.IsInitialized)
        {
            logger.LogDebug(
                "User is authenticated but permissions are not initialized yet. " +
                "Authorization will be re-evaluated once permissions are loaded.");
            return Task.CompletedTask;
        }

        logger.LogInformation(
                "User needs {Permission}. User has permissions: {UserPermissions}",
                requirement.Permission,
                string.Join(", ", permissionStore.Permissions));

        if (permissionStore.Permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
