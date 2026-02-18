using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Authentication;

namespace Mediflow.Application.Interfaces.Services;

public interface IAuthenticationService : ITransientService
{
    #region Login and 2FA Settings
    AuthenticationDto Login(LoginDto login);

    AuthenticationViaSpaDto LoginViaSpa(LoginDto login);

    TokenDto Login2FactorAuthentication(Login2FactorAuthenticationDto login);

    LoginSpaDto Login2FactorAuthenticationViaSpa(Login2FactorAuthenticationDto login);
    #endregion

    #region Forgot Password
    void ForgetPasswordConfirmation(ForgotPasswordConfirmationDto forgotPassword);

    void ForgotPasswordVerification(ForgotPasswordResetDto forgotPassword);
    #endregion

    #region Logout
    void Logout();
    #endregion
}