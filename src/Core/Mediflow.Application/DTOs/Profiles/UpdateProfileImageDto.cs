using Microsoft.AspNetCore.Http;

namespace Mediflow.Application.DTOs.Profiles;

public class UpdateProfileImageDto
{
    public required IFormFile ProfileImage { get; set; }
}
