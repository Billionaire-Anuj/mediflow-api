using Mediflow.Application.DTOs.Profiles;

namespace Mediflow.Application.DTOs.Authentication;

public class AuthenticationViaSpaDto
{
    public bool IsTwoFactorRequired { get; set; }

    public ProfileDto? Profile { get; set; }
}
