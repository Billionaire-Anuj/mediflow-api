namespace Mediflow.Application.DTOs.Authentication;

public class TokenDto :  LoginSpaDto
{
    public string Token { get; set; } = string.Empty;
}