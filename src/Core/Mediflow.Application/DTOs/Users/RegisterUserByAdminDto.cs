using Microsoft.AspNetCore.Http;

namespace Mediflow.Application.DTOs.Users;

public class RegisterUserByAdminDto : AbstractUserRegistrationDto
{
    public IFormFile? ProfileImage { get; set; }
}
