using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.DTOs.Assets;

namespace Mediflow.Application.DTOs.Profiles;

public static class ProfileExtensionMethods
{
    public static ProfileDto ToProfileDto(this User user)
    {
        return new ProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Gender = user.Gender,
            Address = user.Address,
            IsActive = user.IsActive,
            Username = user.Username,
            PhoneNumber = user.PhoneNumber,
            EmailAddress = user.EmailAddress,
            ProfileImage = user.ProfileImage?.ToAssetDto(),
            Role = (user.Role ?? Role.Default).ToRoleDto(),
            Is2FactorAuthenticationEnabled = user.Is2FactorAuthenticationEnabled
        };
    }
}