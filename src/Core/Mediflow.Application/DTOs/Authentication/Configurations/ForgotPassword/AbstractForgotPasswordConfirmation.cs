namespace Mediflow.Application.DTOs.Authentication.Configurations.ForgotPassword;

public abstract class AbstractForgotPasswordConfirmation
{
    public string OneTimePassword { get; set; } = string.Empty;
    
    public bool IsVerified { get; set; }
}