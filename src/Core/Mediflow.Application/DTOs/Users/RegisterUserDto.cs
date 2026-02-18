using Microsoft.AspNetCore.Http;

namespace Mediflow.Application.DTOs.Users;

public class RegisterUserDto : AbstractUserRegistrationDto
{
    public string Password { get; set; } = string.Empty;

    public IFormFile? ProfileImage { get; set; }
}