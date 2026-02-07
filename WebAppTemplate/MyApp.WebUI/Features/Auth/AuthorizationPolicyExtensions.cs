namespace MyApp.WebUI.Features.Auth;

using Microsoft.AspNetCore.Authorization;

public static class Policies
{
    public const string OrderView = "Order.View";
    public const string OrderEdit = "Order.Edit";
    public const string AdminAccess = "Admin.Access";
}

public sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}

public static class AuthorizationPolicyExtensions
{
    public static void AddPermissionPolicies(
        this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.OrderView, policy =>
            policy.Requirements.Add(
                new PermissionRequirement("order:view")));

        options.AddPolicy(Policies.OrderEdit, policy =>
            policy.Requirements.Add(
                new PermissionRequirement("order:edit")));

        options.AddPolicy(Policies.AdminAccess, policy =>
            policy.Requirements.Add(
                new PermissionRequirement("admin:access")));
    }
}
