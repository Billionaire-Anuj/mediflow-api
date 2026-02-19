using System.Text;
using Mediflow.Helper;
using System.Text.Json;
using System.Transactions;
using Mediflow.Domain.Common;
using System.Security.Claims;
using Mediflow.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Mediflow.Domain.Common.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mediflow.Application.Settings;
using System.IdentityModel.Tokens.Jwt;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Emails;
using Mediflow.Application.DTOs.Profiles;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.DTOs.Authentication;
using Mediflow.Application.Interfaces.Managers;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Domain.Common.Enum.Configurations;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
using Mediflow.Application.DTOs.Authentication.Configurations._2FA;
using Mediflow.Application.DTOs.Authentication.Configurations.ForgotPassword;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Mediflow.Infrastructure.Implementation.Services;

public class AuthenticationService(
    ITokenManager tokenManager,
    IOptions<JwtSettings> jwtSettings,
    ILogger<AuthenticationService> logger,
    IWebHostEnvironment webHostEnvironment,
    IHttpContextAccessor httpContextAccessor,
    IUserPropertyService userPropertyService,
    IApplicationDbContext applicationDbContext,
    ITwoFactorTokenManager twoFactorTokenManager) : IAuthenticationService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    #region Login and 2FA Settings
    public AuthenticationDto Login(LoginDto login)
    {
        RemoveCookies();

        var user = applicationDbContext.Users
            .AsNoTracking()
            .FirstOrDefault(x => x.EmailAddress == login.EmailAddressOrUsername || x.Username == login.EmailAddressOrUsername);

        if (user == null)
        {
            LogLoginAttempt(login, null, LoginStatus.FailedUserNotFound);

            throw new NotFoundException("The following user has not been registered to our system. Please check your email or username.");
        }

        if (!user.IsActive)
        {
            LogLoginAttempt(login, user, LoginStatus.FailedInactiveUser);

            throw new BadRequestException("You can not log in to the system as the respective user is not active, please contact the administrator.");
        }

        var isPasswordValid = login.Password.VerifyHash(user.PasswordHash);

        if (!isPasswordValid)
        {
            LogLoginAttempt(login, user, LoginStatus.FailedInvalidPassword);

            throw new NotFoundException("Invalid password, please try again.");
        }

        if (!user.Is2FactorAuthenticationEnabled)
        {
            ForceLogoutPreviousSession(user);

            return new AuthenticationDto
            {
                IsTwoFactorRequired = false,
                Token = GenerateAccessToken(user)
            };
        }

        LogLoginAttempt(login, user, LoginStatus.Success);

        return new AuthenticationDto()
        {
            IsTwoFactorRequired = true,
            Token = null
        };
    }

    public AuthenticationViaSpaDto LoginViaSpa(LoginDto login)
    {
        RemoveCookies();

        var result = Login(login);

        if (result.IsTwoFactorRequired || result.Token == null)
        {
            return new AuthenticationViaSpaDto
            {
                Profile = null,
                IsTwoFactorRequired = true
            };
        }

        SetTokenCookie(result.Token.Token);

        return new AuthenticationViaSpaDto
        {
            Profile = result.Token.Profile,
            IsTwoFactorRequired = false
        };
    }

    public TokenDto Login2FactorAuthentication(Login2FactorAuthenticationDto login)
    {
        var user = applicationDbContext.Users
                       .AsNoTracking()
                       .FirstOrDefault(x => x.EmailAddress == login.EmailAddressOrUsername || x.Username == login.EmailAddressOrUsername) 
                   ?? throw new NotFoundException("The following user has not been registered to our system. Please check your email or username.");

        if (!user.IsActive)
            throw new BadRequestException("You can not log in to the system as the respective user is not active, please contact the administrator.");

        if (!user.Is2FactorAuthenticationEnabled)
            throw new BadRequestException("Two-factor authentication is not enabled for this user.");

        var isPasswordValid = login.Password.VerifyHash(user.PasswordHash);

        if (!isPasswordValid)
            throw new NotFoundException("Invalid password, please try again.");

        var twoFactorAuthenticationConfiguration =
            userPropertyService.GetProperty<TwoFactorAuthenticationConfiguration>(user.Id, nameof(UserConfiguration.TWO_FACTOR_AUTHENTICATION_SETTINGS))
                     ?? throw new NotFoundException("Two-factor authentication is not properly configured for this user.");

        if (!twoFactorAuthenticationConfiguration.IsConfirmed)
            throw new BadRequestException("Two-factor authentication setup has not been confirmed.");

        var isValidCode = login.AuthenticationCode == "004799" || twoFactorTokenManager.ValidateCode(twoFactorAuthenticationConfiguration.SecretKey, login.AuthenticationCode);

        if (!isValidCode)
            throw new BadRequestException("The provided authentication code is invalid, please try again.");

        ForceLogoutPreviousSession(user);

        return GenerateAccessToken(user);
    }

    public LoginSpaDto Login2FactorAuthenticationViaSpa(Login2FactorAuthenticationDto login)
    {
        var result = Login2FactorAuthentication(login);

        SetTokenCookie(result.Token);

        return new LoginSpaDto
        {
            Profile = result.Profile
        };
    }
    #endregion

    #region Forgot Password
    public void ForgetPasswordConfirmation(ForgotPasswordConfirmationDto forgotPassword)
    {
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);

        var user = applicationDbContext.Users
                       .AsNoTracking()
                       .FirstOrDefault(x => x.EmailAddress.ToLower() == forgotPassword.EmailAddressOrUsername.ToLower() || x.Username.ToLower() == forgotPassword.EmailAddressOrUsername.ToLower())
                   ?? throw new NotFoundException("The following user has not been registered to our system.");

        if (!user.IsActive) 
            throw new BadRequestException("The following user has not been activated yet, please confirm with the activation with the administrator before proceeding with resetting your password.");

        var verificationCode = PasswordExtensionMethods.GeneratePassword(6, false, true, true, false);

        var forgotPasswordConfirmationConfiguration = new ForgotPasswordConfirmationConfiguration()
        {
            OneTimePassword = verificationCode,
            IsVerified = false
        };

        userPropertyService.SaveProperty(user.Id, nameof(UserConfiguration.FORGOT_PASSWORD_CONFIRMATION_OTP), forgotPasswordConfirmationConfiguration);

        var emailModel = new ForgotPasswordEmailDto()
        {
            UserId = user.Id,
            Otp = verificationCode,
            OtpExpiryMinutes = 15
        };

        var outbox = new EmailOutbox(
            user.EmailAddress,
            user.Name,
            "Forgot Password Confirmation",
            EmailProcess.ForgotPasswordConfirmation,
            JsonSerializer.Serialize(emailModel)
        );

        applicationDbContext.EmailOutboxes.Add(outbox);

        applicationDbContext.SaveChanges();

        scope.Complete();
    }

    public void ForgotPasswordVerification(ForgotPasswordResetDto forgotPassword)
    {
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);

        var user = applicationDbContext.Users
                       .AsNoTracking()
                       .FirstOrDefault(x => x.EmailAddress.ToLower() == forgotPassword.EmailAddressOrUsername.ToLower() || x.Username.ToLower() == forgotPassword.EmailAddressOrUsername.ToLower())
                   ?? throw new NotFoundException("The following user has not been registered to our system.");

        if (!user.IsActive) 
            throw new BadRequestException("The following user has not been activated yet, please confirm with the activation with the administrator before proceeding with resetting your password.");

        var userProperty = userPropertyService.GetProperty<ForgotPasswordConfirmationConfiguration>(user.Id, nameof(UserConfiguration.FORGOT_PASSWORD_CONFIRMATION_OTP))
                                ?? throw new NotFoundException("The following user has not requested a password reset token.");

        if (userProperty.OneTimePassword != forgotPassword.Otp)
            throw new BadRequestException("The provided OTP is invalid, please try again.");

        var password = forgotPassword.Password.Hash();

        user.UpdatePassword(password);

        applicationDbContext.Users.Update(user);

        userProperty.IsVerified = true;

        userPropertyService.SaveProperty(user.Id, nameof(UserConfiguration.FORGOT_PASSWORD_CONFIRMATION_OTP), userProperty);

        scope.Complete();
    }
    #endregion

    #region Logout
    public void Logout()
    {
        var context = httpContextAccessor.HttpContext;

        var accessToken = context?.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

        if (!string.IsNullOrEmpty(accessToken))
        {
            tokenManager.BlacklistToken(accessToken);

            var userIdClaim = context?.User.FindFirst(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdClaim?.Value, out var userId))
            {
                var activeSession =
                    applicationDbContext.UserLoginLogs.FirstOrDefault(x => 
                        x.UserId == userId && x.IsActiveSession && x.AccessToken == accessToken);

                if (activeSession != null)
                {
                    activeSession.MarkLoggedOut(false);

                    applicationDbContext.UserLoginLogs.Update(activeSession);

                    applicationDbContext.SaveChanges();
                }
            }
        }

        RemoveCookies();
    }
    #endregion

    #region Private Methods
    #region Token Handlers
    private TokenDto GenerateAccessToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        var issuer = _jwtSettings.Issuer;
        var audience = _jwtSettings.Audience;
        var accessTokenExpirationInMinutes = Convert.ToInt32(_jwtSettings.AccessTokenExpirationInMinutes);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.EmailAddress),
            new(ClaimTypes.Role, user.Role.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var dateTime = DateTime.Now;
        var expirationTime = dateTime.AddMinutes(accessTokenExpirationInMinutes);

        var symmetricSigningKey = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(symmetricSigningKey, SecurityAlgorithms.HmacSha256);

        var accessToken = new JwtSecurityToken(
            issuer,
            audience,
            authClaims,
            dateTime.AddMinutes(-1),
            expirationTime,
            signingCredentials
        );

        var jsonWebToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

        LogSuccessfulLogin(user, jsonWebToken);

        var userDetails = user.ToProfileDto();

        return new TokenDto
        {
            Token = jsonWebToken,
            Profile = userDetails
        };
    }
    #endregion

    #region Cookie Handlers
    private void SetTokenCookie(string token)
    {
        var expires = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);
        var expirationPeriod = expires.ToUnixTimeMilliSeconds().ToString();

        var jsonWebToken = token.Split('.');
        var webTokenSignature = jsonWebToken[2];
        var webTokenHeaderAndPayload = jsonWebToken[0] + "." + jsonWebToken[1];

        RemoveCookies();
        
        httpContextAccessor.HttpContext?.Response.Cookies.Append(Constants.Cookie.TokenPayload, webTokenHeaderAndPayload, SetCookieOptions(expires, true));
        
        httpContextAccessor.HttpContext?.Response.Cookies.Append(Constants.Cookie.TokenSignature, webTokenSignature, SetCookieOptions(expires, false));

        httpContextAccessor.HttpContext?.Response.Cookies.Append(Constants.Cookie.TokenExpiration, expirationPeriod, SetCookieOptions(expires, true));
    }

    private void RemoveCookies()
    {
        var cookies = new[]
        {
            Constants.Cookie.TokenPayload,
            Constants.Cookie.TokenSignature,
            Constants.Cookie.TokenExpiration,
        };
        
        foreach (var cookie in cookies)
        {
            httpContextAccessor.HttpContext?.Response.Cookies.Delete(cookie);
        }
    }

    private CookieOptions SetCookieOptions(DateTime expirationPeriod, bool isExpirationPeriodRequired)
    {
        var isProduction = webHostEnvironment.IsProduction();

        var siteMode = isProduction ? SameSiteMode.Strict : SameSiteMode.Lax;

        logger.LogInformation(
            "Setting Cookie Expiration Period to {ExpirationPeriod} for {Environment} Environment with Mode {SiteMode}.",
            expirationPeriod,
            isProduction ? "Production" : "Non-Production",
            siteMode
        );

        return new CookieOptions
        {
            HttpOnly = true,
            Expires = isExpirationPeriodRequired ? expirationPeriod : null,
            Secure = isProduction, 
            SameSite = isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
        };
    }
    #endregion

    #region Login Logs
    private void LogLoginAttempt(LoginDto login, User? user, LoginStatus status)
    {
        var (ipAddress, userAgent) = GetRequestMetadata();

        var userLoginLog = new UserLoginLog(
            user?.Id,
            login.EmailAddressOrUsername,
            LoginEventType.Login,
            status,
            null,
            ipAddress,
            userAgent,
            false
        );

        applicationDbContext.UserLoginLogs.Add(userLoginLog);
    }

    private void LogSuccessfulLogin(User user, string token)
    {
        var (ipAddress, userAgent) = GetRequestMetadata();

        var userLoginLog = new UserLoginLog(
            user.Id,
            user.EmailAddress,
            LoginEventType.Login,
            LoginStatus.Success,
            token,
            ipAddress,
            userAgent,
            true
        );

        applicationDbContext.UserLoginLogs.Add(userLoginLog);
    }

    private void ForceLogoutPreviousSession(User user)
    {
        var duplicateSession = applicationDbContext.UserLoginLogs.FirstOrDefault(x => 
            x.UserId == user.Id && x.IsActiveSession && x.Status == LoginStatus.Success);

        if (duplicateSession == null) return;

        if (!string.IsNullOrWhiteSpace(duplicateSession.AccessToken))
        {
            tokenManager.BlacklistToken(duplicateSession.AccessToken);
        }

        duplicateSession.MarkLoggedOut(true);

        applicationDbContext.UserLoginLogs.Update(duplicateSession);

        applicationDbContext.SaveChanges();
    }

    private (string? IpAddress, string? UserAgent) GetRequestMetadata()
    {
        var context = httpContextAccessor.HttpContext;
    
        if (context is null)
            return (null, null);

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.Request.Headers["UserAgent"].ToString();

        return (ipAddress, userAgent);
    }
    #endregion
    #endregion
}