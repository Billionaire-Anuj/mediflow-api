using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Mediflow.Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Mediflow.Infrastructure.Implementation.Services;

public class TokenService(IDistributedCache cache, IHttpContextAccessor httpContextAccessor) : ITokenService
{
    public async Task<bool> IsCurrentActiveTokenAsync() => await IsActiveAsync(GetCurrentAsync());

    public async Task DeactivateCurrentAsync() => await DeactivateAsync(GetCurrentAsync());

    public async Task<bool> IsActiveAsync(string token) => await cache.GetStringAsync(GetKey(token)) == null;

    public async Task DeactivateAsync(string token) 
        => await cache.SetStringAsync(GetKey(token), " ", 
            new DistributedCacheEntryOptions 
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });

    private string GetCurrentAsync()
    {
        if (httpContextAccessor.HttpContext == null) return string.Empty;
        
        var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];

        return authorizationHeader == StringValues.Empty
            ? string.Empty
            : authorizationHeader.Single()?.Split(" ").Last() ?? string.Empty;
    }
    
    private static string GetKey(string token) => $"tokens:{token}:deactivated";
}