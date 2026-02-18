using Mediflow.Helper;
using Mediflow.Domain.Common;

namespace Mediflow.API.Middleware;

public class TokenCookieMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment, ILogger<TokenCookieMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            logger.LogDebug("No Authorization Header Set - Checking for Cookies.");
            
            if (context.Request.Cookies.ContainsKey(Constants.Cookie.TokenPayload) && context.Request.Cookies.ContainsKey(Constants.Cookie.TokenSignature))
            {
                logger.LogDebug("Cookies Found. Stitching Them Together...");
                
                var tokenSignature = context.Request.Cookies["X-GVAC-Token-Signature"];
                var tokenHeaderAndPayload = context.Request.Cookies["X-GVAC-Token-Payload"];
                
                var token = $"{tokenHeaderAndPayload}.{tokenSignature}";
                
                logger.LogDebug($"Token: {token}.");
                
                context.Request.Headers.Append("Authorization", $"Bearer {token}");

                var isProduction = webHostEnvironment.IsProduction();

                var expires = DateTime.Now.AddMinutes(30);
                
                var expirationPeriod = expires.ToUnixTimeMilliSeconds().ToString();
                
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = expires,
                    Secure = isProduction, 
                    SameSite = isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
                };
                
                if (tokenHeaderAndPayload != null)
                {
                    context.Response.Cookies.Append
                    (
                        Constants.Cookie.TokenPayload, 
                        tokenHeaderAndPayload,
                        cookieOptions
                    );
                }
                
                context.Response.Cookies.Append
                (
                    Constants.Cookie.TokenExpiration, 
                    expirationPeriod,
                    cookieOptions
                );
            }
        }

        await next(context);
    }
}
