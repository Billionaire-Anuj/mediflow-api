using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.Common.Response;
using Mediflow.Domain.Common.Enum;

namespace Mediflow.Application.DTOs.Users;

public class UserDto : BaseDto
{
    public string Name { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Address { get; set; }

    public bool Is2FactorAuthenticationEnabled { get; set; }

    public Gender Gender { get; set; }

    public RoleDto Role { get; set; } = new();

    public AssetDto? ProfileImage { get; set; }
}