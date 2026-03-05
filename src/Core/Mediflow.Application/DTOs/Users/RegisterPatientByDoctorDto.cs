using Mediflow.Domain.Common.Enum;
using Microsoft.AspNetCore.Http;

namespace Mediflow.Application.DTOs.Users;

public class RegisterPatientByDoctorDto
{
    public Gender Gender { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public IFormFile? ProfileImage { get; set; }
}
