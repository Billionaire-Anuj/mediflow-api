using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Authentication;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class AuthenticationController(IAuthenticationService authenticationService) : BaseController<AuthenticationController>
{
    [AllowAnonymous]
    [HttpPost("login")]
    [Documentation("Login", "Login using credentials.")]
    public ResponseDto<AuthenticationDto> Login([FromBody] LoginDto login)
    {
        var result = authenticationService.Login(login);

        return new ResponseDto<AuthenticationDto>(
            (int)HttpStatusCode.OK,
            result.IsTwoFactorRequired ? "Login verification successful, please proceed with your OTP verification." : "Successfully logged in.",
            result
        );
    }
    
    [AllowAnonymous]
    [HttpPost("login/spa")]
    [Documentation("LoginViaSpa", "Login via SPA using credentials.")]
    public ResponseDto<AuthenticationViaSpaDto> LoginViaSpa([FromBody] LoginDto login)
    {
        var result = authenticationService.LoginViaSpa(login);

        return new ResponseDto<AuthenticationViaSpaDto>(
            (int)HttpStatusCode.OK,
            result.IsTwoFactorRequired ? "Login verification successful, please proceed with your OTP verification." : "Successfully logged in.",
            result
        );
    }

    [AllowAnonymous]
    [HttpPost("login/2fa")]
    [Documentation("Login2FactorAuthentication", "Login with 2FA Credentials using credentials.")]
    public ResponseDto<TokenDto> Login2FactorAuthentication([FromBody] Login2FactorAuthenticationDto login)
    {
        var result = authenticationService.Login2FactorAuthentication(login);

        return new ResponseDto<TokenDto>(
            (int)HttpStatusCode.OK,
            "Successfully logged in.",
            result
        );
    }
    
    [AllowAnonymous]
    [HttpPost("login/2fa/spa")]
    [Documentation("Login2FactorAuthenticationViaSpa", "Login with 2FA Credentials via SPA using credentials.")]
    public ResponseDto<LoginSpaDto> Login2FactorAuthenticationViaSpa([FromBody] Login2FactorAuthenticationDto login)
    {
        var result = authenticationService.Login2FactorAuthenticationViaSpa(login);

        return new ResponseDto<LoginSpaDto>(
            (int)HttpStatusCode.OK,
            "Successfully logged in.",
            result
        );
    }

    [AllowAnonymous]
    [HttpPost("forgot/password")]
    [Documentation("ForgetPasswordConfirmation", "Triggers an email address for forgot password confirmation.")]
    public ResponseDto<bool> ForgetPasswordConfirmation([FromBody] ForgotPasswordConfirmationDto forgotPassword)
    {
        authenticationService.ForgetPasswordConfirmation(forgotPassword);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Confirmation email successfully sent.",
            true
        );
    }

    [AllowAnonymous]
    [HttpPost("forgot/password/verification")]
    [Documentation("ForgotPasswordVerification", "Verifies the OTP and password confirmation.")]
    public ResponseDto<bool> ForgotPasswordVerification([FromBody] ForgotPasswordResetDto forgotPassword)
    {
        authenticationService.ForgotPasswordVerification(forgotPassword);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Password successfully changed.",
            true
        );
    }

    [HttpPost("logout")]
    [Documentation("Logout", "Logout from the application.")]
    public ResponseDto<bool> Logout()
    {
        authenticationService.Logout();

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Successfully logged in.",
            true
        );
    }
}