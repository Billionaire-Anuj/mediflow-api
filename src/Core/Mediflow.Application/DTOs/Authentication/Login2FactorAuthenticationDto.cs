namespace Mediflow.Application.DTOs.Authentication;

public class Login2FactorAuthenticationDto : LoginDto
{
    public string AuthenticationCode { get; set; } = string.Empty;
}