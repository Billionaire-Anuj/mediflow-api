using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Profiles;

namespace Mediflow.Application.DTOs.Users;

public static class UserExtensionMethods
{
    public static UserDto ToUserDto(this User user)
    {
        return user.ToProfileDto();
    }
}