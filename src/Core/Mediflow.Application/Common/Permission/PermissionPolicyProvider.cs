using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;

namespace Mediflow.Application.Common.Permission;

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private const string Prefix = "Permissions.";

    private static readonly ConcurrentDictionary<string, AuthorizationPolicy> PolicyCache = new(StringComparer.Ordinal);

    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.AsSpan().StartsWith(Prefix, StringComparison.Ordinal))
            return _fallback.GetPolicyAsync(policyName);
        
        if (PolicyCache.TryGetValue(policyName, out var cached))
            return Task.FromResult<AuthorizationPolicy?>(cached);
        
        if (!TryParse(policyName, out var resource, out var action))
            return Task.FromResult<AuthorizationPolicy?>(null);
        
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(PermissionRequirementPool.Get(resource, action))
            .Build();

        PolicyCache[policyName] = policy;

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallback.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallback.GetFallbackPolicyAsync();

    private static bool TryParse(string policyName, out string resource, out string action)
    {
        resource = action = string.Empty;

        var span = policyName.AsSpan();
        
        var start = Prefix.Length;
        if (span.Length <= start) return false;
        
        var prefix = span[start..];
        var firstDot = prefix.IndexOf('.');
        if (firstDot <= 0) return false;                
        var resourceSpan = prefix[..firstDot];
        
        var suffix = prefix[(firstDot + 1)..];
        var secondDot = suffix.IndexOf('.');
        if (secondDot != -1) return false;

        if (suffix.IsEmpty) return false;

        resource = new string(resourceSpan);
        action = new string(suffix);
        return true;
    }
}

internal static class PermissionRequirementPool
{
    private static readonly ConcurrentDictionary<(string resource, string action), PermissionRequirement> _pool = new();

    public static PermissionRequirement Get(string resource, string action)
        => _pool.GetOrAdd((resource, action),
            static k => new PermissionRequirement(k.resource, k.action));
}
