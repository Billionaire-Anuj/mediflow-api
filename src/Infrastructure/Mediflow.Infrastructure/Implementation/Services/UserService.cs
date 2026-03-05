using Mediflow.Helper;
using System.Text.Json;
using System.Transactions;
using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.DTOs.Emails;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class UserService(
    IFileService fileService,
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService) : IUserService
{
    private const string UserImagesFilePath = Constants.FilePath.UserImagesFilePath;

    public List<UserDto> GetAllUsers(
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
        List<Guid>? roleIds = null)
    {
        var roleIdentifiers = roleIds != null ? new HashSet<Guid>(roleIds) : null;

        var userModels = applicationDbContext.Users
            .Where(x =>
                x.Role!.IsDisplayed &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.ToLower().Contains(globalSearch.ToLower())
                    || x.Username.ToLower().Contains(globalSearch.ToLower())
                    || x.EmailAddress.ToLower().Contains(globalSearch.ToLower())
                    || (x.Address != null && x.Address.ToLower().Contains(globalSearch.ToLower()))
                    || x.PhoneNumber.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (username == null || x.Username.ToLower().Contains(username.ToLower())) &&
                (emailAddress == null || x.EmailAddress.ToLower().Contains(emailAddress.ToLower())) &&
                (address == null || (x.Address != null && x.Address.ToLower().Contains(address.ToLower()))) &&
                (phoneNumber == null || x.PhoneNumber.ToLower().Contains(phoneNumber.ToLower())) &&
                (roleIdentifiers == null || roleIdentifiers.Contains(x.RoleId)))
            .Include(x => x.Role)
            .OrderBy(x => orderBys);

        rowCount = userModels.Count();

        return userModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToUserDto())
            .ToList();
    }

    public List<UserDto> GetAllUsers(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? name = null,
        string? username = null,
        string? emailAddress = null,
        string? address = null,
        string? phoneNumber = null,
        List<Guid>? roleIds = null)
    {
        var roleIdentifiers = roleIds != null ? new HashSet<Guid>(roleIds) : null;

        var userModels = applicationDbContext.Users
            .Where(x => 
                x.Role!.IsDisplayed &&
                (string.IsNullOrEmpty(globalSearch) 
                    || x.Name.ToLower().Contains(globalSearch.ToLower())
                    || x.Username.ToLower().Contains(globalSearch.ToLower())
                    || x.EmailAddress.ToLower().Contains(globalSearch.ToLower())
                    || (x.Address != null && x.Address.ToLower().Contains(globalSearch.ToLower()))
                    || x.PhoneNumber.ToLower().Contains(globalSearch.ToLower())) && 
                (isActive == null || isActive.Contains(x.IsActive)) && 
                (name == null || x.Name.ToLower().Contains(name.ToLower())) &&
                (username == null || x.Username.ToLower().Contains(username.ToLower())) &&
                (emailAddress == null || x.EmailAddress.ToLower().Contains(emailAddress.ToLower())) &&
                (address == null || (x.Address != null && x.Address.ToLower().Contains(address.ToLower()))) &&
                (phoneNumber == null || x.PhoneNumber.ToLower().Contains(phoneNumber.ToLower())) &&
                (roleIdentifiers == null || roleIdentifiers.Contains(x.RoleId)))
            .Include(x => x.Role)
            .OrderBy(x => orderBys);

        return userModels.Select(x => x.ToUserDto()).ToList();
    }

    public UserDto GetUserById(Guid userId)
    {
        var userModel = applicationDbContext.Users
                .FirstOrDefault(x => x.Id == userId)
                        ?? throw new NotFoundException($"User with identifier '{userId}' was not found.");

        return userModel.ToUserDto();
    }

    public void RegisterUser(RegisterUserDto user)
    {
        var duplicateUser = applicationDbContext.Users.FirstOrDefault(x => x.Username == user.Username || x.EmailAddress == user.EmailAddress || x.PhoneNumber == user.PhoneNumber);

        if (duplicateUser != null)
        {
            throw new BadRequestException("The following user with the specified username, phone number or email address already exists.");
        }

        var role = applicationDbContext.Roles
                   .FirstOrDefault(x => x.Id == user.RoleId)
                   ?? throw new NotFoundException("The respective role with the specified identifier was not found.");

        if (role.Id.ToString() != Constants.Roles.Patient.Id)
        {
            throw new BadRequestException("Only patients can self-register. Please contact an administrator for staff accounts.");
        }

        if (!role.IsRegisterable)
        {
            throw new BadRequestException("A new user with the respective role cannot be be registered.");
        }

        var passwordHash = user.Password.Hash();

        var asset = user.ProfileImage != null ? fileService.UploadDocument(user.ProfileImage, UserImagesFilePath) : null;

        var userModel = new User(
            role.Id,
            user.Gender,
            user.Name,
            user.Username,
            user.EmailAddress,
            user.Address,
            asset?.ToAssetModel(),
            passwordHash,
            user.PhoneNumber);

        if (role.Id.ToString() == Constants.Roles.Doctor.Id || role.Id.ToString() == Constants.Roles.LabTechnician.Id || role.Id.ToString() == Constants.Roles.Pharmacist.Id)
        {
            userModel.IsActive = false;
        }

        applicationDbContext.Users.Add(userModel);

        applicationDbContext.SaveChanges();
    }

    public void RegisterUserByAdmin(RegisterUserByAdminDto user)
    {
        var duplicateUser = applicationDbContext.Users.FirstOrDefault(x =>
            x.Username == user.Username ||
            x.EmailAddress == user.EmailAddress ||
            x.PhoneNumber == user.PhoneNumber);

        if (duplicateUser != null)
        {
            throw new BadRequestException("The following user with the specified username, phone number or email address already exists.");
        }

        var role = applicationDbContext.Roles
                   .FirstOrDefault(x => x.Id == user.RoleId)
                   ?? throw new NotFoundException("The respective role with the specified identifier was not found.");

        var generatedPassword = PasswordExtensionMethods.GeneratePassword();
        var passwordHash = generatedPassword.Hash();

        var asset = user.ProfileImage != null ? fileService.UploadDocument(user.ProfileImage, UserImagesFilePath) : null;

        var userModel = new User(
            role.Id,
            user.Gender,
            user.Name,
            user.Username,
            user.EmailAddress,
            user.Address,
            asset?.ToAssetModel(),
            passwordHash,
            user.PhoneNumber);

        applicationDbContext.Users.Add(userModel);

        applicationDbContext.SaveChanges();

        var applicationUserIdentifier = applicationUserService.GetUserId;

        var applicationUser = applicationDbContext.Users
                              .FirstOrDefault(x => x.Id == applicationUserIdentifier)
                              ?? throw new NotFoundException($"User with identifier '{applicationUserIdentifier}' was not found.");

        var emailModel = new AccountRegistrationEmailDto
        {
            UserId = userModel.Id,
            ApplicationUserId = applicationUser.Id,
            Password = generatedPassword
        };

        var outbox = new EmailOutbox(
            userModel.EmailAddress,
            userModel.Name,
            "Account Registered",
            EmailProcess.AccountRegistration,
            JsonSerializer.Serialize(emailModel)
        );

        applicationDbContext.EmailOutboxes.Add(outbox);

        applicationDbContext.SaveChanges();
    }

    public void UpdateUser(Guid userId, UpdateUserDto user)
    {
        if (userId != user.Id)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var userModel = applicationDbContext.Users.FirstOrDefault(x => x.Id == user.Id)
                            ?? throw new NotFoundException($"User with identifier '{userId}' was not found.");

        var duplicateUser = applicationDbContext.Users.FirstOrDefault(x => x.Username == user.Username || x.EmailAddress == user.EmailAddress || x.PhoneNumber == user.PhoneNumber);

        if (duplicateUser != null)
        {
            throw new BadRequestException("The following user with the specified username or email address already exists.");
        }

        var roleId = userModel.RoleId;

        if (userModel.RoleId != user.RoleId)
        {
            var role = applicationDbContext.Roles
                       .FirstOrDefault(x => x.Id == user.RoleId) 
                       ?? throw new NotFoundException("The respective role with the specified identifier was not found.");

            roleId = role.Id;
        }

        if (user.ProfileImage is not null)
        {
            if (userModel.ProfileImage is not null && !string.IsNullOrEmpty(userModel.ProfileImage.FileUrl))
            {
                var oldImagePath = Path.Combine(UserImagesFilePath, userModel.ProfileImage.FileUrl);
                
                fileService.DeleteFile(oldImagePath);
            }

            var profileImageAsset = fileService.UploadDocument(user.ProfileImage, UserImagesFilePath);
            
            userModel.UpdateProfileImage(profileImageAsset.ToAssetModel());
        }
        
        userModel.Update(
            roleId,
            user.Gender,
            user.Name,
            user.Username,
            user.EmailAddress,
            user.Address,
            user.PhoneNumber);

        applicationDbContext.Users.Update(userModel);

        applicationDbContext.SaveChanges();
    }

    public void ResetPassword(Guid userId)
    {
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        
        using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        
        var userModel = applicationDbContext.Users.FirstOrDefault(x => x.Id == userId)
                        ?? throw new NotFoundException($"User with identifier '{userId}' was not found.");

        var applicationUserIdentifier = applicationUserService.GetUserId;

        var applicationUser = applicationDbContext.Users
                              .FirstOrDefault(x => x.Id == applicationUserIdentifier)
                              ?? throw new NotFoundException($"User with identifier '{applicationUserIdentifier}' was not found.");

        var password = PasswordExtensionMethods.GeneratePassword();

        var hashedPassword = password.Hash();

        userModel.UpdatePassword(hashedPassword);

        applicationDbContext.Users.Update(userModel);

        var emailModel = new ResetPasswordEmailDto()
        {
            UserId = userId,
            Password = password,
            ApplicationUserId = applicationUser.Id
        };
        
        var outbox = new EmailOutbox(
            userModel.EmailAddress,
            userModel.Name,
            "Reset Password",
            EmailProcess.PasswordResetConfirmation,
            JsonSerializer.Serialize(emailModel)
        );

        applicationDbContext.EmailOutboxes.Add(outbox);

        applicationDbContext.SaveChanges();

        scope.Complete();
    }

    public void ActivateDeactivateUser(Guid userId)
    {
        var userModel = applicationDbContext.Users
                            .FirstOrDefault(x => x.Id == userId) 
                        ?? throw new NotFoundException($"User with identifier '{userId}' was not found.");

        if (userModel.Role?.IsRegisterable == false)
        {
            throw new BadRequestException("A user with the respective assigned role cannot be deactivated.");
        }

        userModel.ActivateDeactivateEntity();

        applicationDbContext.Users.Update(userModel);

        applicationDbContext.SaveChanges();
    }
}
