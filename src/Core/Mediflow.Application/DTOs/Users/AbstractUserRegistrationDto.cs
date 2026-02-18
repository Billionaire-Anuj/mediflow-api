using Mediflow.Domain.Common.Enum;

namespace Mediflow.Application.DTOs.Users;

public abstract class AbstractUserRegistrationDto
{
    public Guid RoleId { get; set; }

    public Gender Gender { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;
}