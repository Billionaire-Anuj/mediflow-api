using Mediflow.Application.DTOs.Users;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IUserService : ITransientService
{
    List<UserDto> GetAllUsers(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? username = null,
        string? emailAddress = null,
        string? address = null,
        string? phoneNumber = null,
        List<Guid>? roleIds = null  
    );

    List<UserDto> GetAllUsers(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? username = null,
        string? emailAddress = null,
        string? address = null,
        string? phoneNumber = null,
        List<Guid>? roleIds = null    
    );

    UserDto GetUserById(Guid userId);

    void RegisterUser(RegisterUserDto user);

    void RegisterUserByAdmin(RegisterUserByAdminDto user);

    void UpdateUser(Guid userId, UpdateUserDto user);
    
    void ResetPassword(Guid userId);

    void ActivateDeactivateUser(Guid userId);
}
