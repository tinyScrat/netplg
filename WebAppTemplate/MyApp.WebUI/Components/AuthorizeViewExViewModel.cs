namespace MyApp.WebUI.Components;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MyApp.Application.Features.User;
using MyApp.WebUI.Abstractions;

internal sealed class AuthorizeViewExViewModel(IUserProfileStore userProfileStore) : ViewModelBase
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _initialized;

    private Task<AuthenticationState>? _authenticationStateTask;

    // parameters
    private string? _roles;
    private string? _policy;
    private string? _policies;
    private bool _requireAll = true;

    public ClaimsPrincipal? User { get; private set; }
    public bool IsAuthorized { get; private set; }
    public bool IsAuthorizing { get; private set; } = true;

    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        // Re-evaluate whenever the user profile store changes (permissions/profile loaded, reset on logout, etc.)
        SubscribeAsync(userProfileStore.Changes, (_, ct) => EvaluateAsync(ct));
    }

    public void SetAuthenticationStateTask(Task<AuthenticationState>? task)
    {
        _authenticationStateTask = task;
    }

    public void SetRequirements(string? roles, string? policy, string? policies, bool requireAll)
    {
        _roles = roles;
        _policy = policy;
        _policies = policies;
        _requireAll = requireAll;
    }

    public async Task EvaluateAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_authenticationStateTask is null)
            {
                IsAuthorizing = true;
                IsAuthorized = false;
                User = null;
                RaiseStateChanged();
                return;
            }

            var authState = await _authenticationStateTask.ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var user = authState.User;
            User = user;

            if (user.Identity?.IsAuthenticated != true)
            {
                IsAuthorizing = false;
                IsAuthorized = false;
                RaiseStateChanged();
                return;
            }

            // Authenticated principal exists, but app profile/permissions might not be loaded yet
            if (!userProfileStore.IsAuthenticated)
            {
                IsAuthorizing = true;
                IsAuthorized = false;
                RaiseStateChanged();
                return;
            }

            IsAuthorizing = false;
            IsAuthorized = CheckAuthorization(user);
            RaiseStateChanged();
        }
        finally
        {
            _gate.Release();
        }
    }

    private bool CheckAuthorization(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true)
            return false;

        // === Role check (AuthorizeView semantics) ===
        var requiredRoles = ParseRoles();
        if (requiredRoles.Count > 0 && !requiredRoles.Any(user.IsInRole))
            return false;

        // === Permission check ===
        var requiredPolicies = ParsePolicies();
        if (requiredPolicies.Count == 0)
            return true; // roles passed, no permission constraint

        var permissions = userProfileStore.Current?.Permissions
            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return _requireAll
            ? requiredPolicies.All(permissions.Contains)
            : requiredPolicies.Any(permissions.Contains);
    }

    private HashSet<string> ParseRoles()
    {
        if (string.IsNullOrWhiteSpace(_roles))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return _roles
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private HashSet<string> ParsePolicies()
    {
        if (!string.IsNullOrWhiteSpace(_policy))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase) { _policy };

        if (string.IsNullOrWhiteSpace(_policies))
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        return _policies
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
