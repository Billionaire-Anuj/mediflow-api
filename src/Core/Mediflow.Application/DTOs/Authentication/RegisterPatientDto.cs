using Mediflow.Domain.Common.Enum;
using Microsoft.AspNetCore.Http;

namespace Mediflow.Application.DTOs.Authentication;

public class RegisterPatientDto
{
    public string Password { get; set; } = string.Empty;

    public IFormFile? ProfileImage { get; set; }

    public Gender Gender { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;
}
