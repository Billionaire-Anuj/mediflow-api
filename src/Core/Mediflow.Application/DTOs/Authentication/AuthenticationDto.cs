namespace Mediflow.Application.DTOs.Authentication;

public class AuthenticationDto
{
    public bool IsTwoFactorRequired { get; set; }

    public TokenDto? Token { get; set; }
}
