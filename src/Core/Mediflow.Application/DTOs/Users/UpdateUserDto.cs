using Microsoft.AspNetCore.Http;

namespace Mediflow.Application.DTOs.Users;

public class UpdateUserDto : AbstractUserRegistrationDto
{
    public Guid Id { get; set; }
    
    public IFormFile? ProfileImage { get; set; }
}