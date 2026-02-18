using Mediflow.Application.DTOs.Profiles;

namespace Mediflow.Application.DTOs.Authentication;

public class LoginSpaDto
{
    public ProfileDto Profile { get; set; } = new();
}