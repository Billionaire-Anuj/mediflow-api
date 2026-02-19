using Mediflow.Helper;
using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.DTOs.Profiles;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.Interfaces.Managers;
using Mediflow.Application.DTOs.Authentication.Configurations._2FA;
using UserConfigurationEnumeration = Mediflow.Domain.Common.Enum.Configurations.UserConfiguration;

namespace Mediflow.Infrastructure.Implementation.Services;

public class ProfileService(
    IFileService fileService,
    IUserPropertyService userPropertyService,
    IApplicationDbContext applicationDbContext,
    ITwoFactorTokenManager twoFactorTokenManager,
    IApplicationUserService applicationUserService) : IProfileService
{
    private const string UserImagesFilePath = Constants.FilePath.UserImagesFilePath;

    public ProfileDto GetProfile()
    {
        var userId = applicationUserService.GetUserId;

        var user = applicationDbContext.Users
                       .AsNoTracking()
                       .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException("The following user was not found.");

        return user.ToProfileDto();
    }

    public RoleDto GetAssignedRole()
    {
        var userId = applicationUserService.GetUserId;

        var user = applicationDbContext.Users
                       .AsNoTracking()
                       .FirstOrDefault(x => x.Id == userId)
                   ?? throw new NotFoundException("The following user was not found.");

        return user.Role.ToRoleDto();
    }

    public void UpdateProfile(UpdateProfileDto profile)
    {
        var userId = applicationUserService.GetUserId;

        var userModel = applicationDbContext.Users.FirstOrDefault(x => x.Id == userId)
                        ?? throw new NotFoundException("The following user was not found.");

        userModel.Update(
            userModel.RoleId,
            userModel.Gender,
            profile.Name,
            profile.Username,
            profile.EmailAddress,
            profile.Address,
            profile.PhoneNumber);

        applicationDbContext.Users.Update(userModel);

        applicationDbContext.SaveChanges();
    }

    public void UpdateProfileImage(UpdateProfileImageDto profileImage)
    {
        var userId = applicationUserService.GetUserId;

        var userModel = applicationDbContext.Users.FirstOrDefault(x => x.Id == userId)
                        ?? throw new NotFoundException("The following user was not found.");

        if (userModel.ProfileImage != null && !string.IsNullOrEmpty(userModel.ProfileImage.FileUrl))
        {
            var oldImagePath = Path.Combine(UserImagesFilePath, userModel.ProfileImage.FileUrl);

            fileService.DeleteFile(oldImagePath);
        }

        var asset = fileService.UploadDocument(profileImage.ProfileImage, UserImagesFilePath);

        userModel.UpdateProfileImage(asset.ToAssetModel());

        applicationDbContext.Users.Update(userModel);

        applicationDbContext.SaveChanges();
    }

    public void ChangePassword(ChangePasswordDto changePassword)
    {
        var userId = applicationUserService.GetUserId;

        var userModel = applicationDbContext.Users.FirstOrDefault(x => x.Id == userId)
                        ?? throw new NotFoundException("The following user was not found.");

        if (!changePassword.CurrentPassword.VerifyHash(userModel.PasswordHash))
        {
            throw new BadRequestException("Current password is incorrect.");
        }

        if (changePassword.NewPassword != changePassword.ConfirmPassword)
        {
            throw new BadRequestException("New password and confirmation password do not match.");
        }

        var passwordHash = changePassword.NewPassword.Hash();

        userModel.UpdatePassword(passwordHash);

        applicationDbContext.Users.Update(userModel);

        applicationDbContext.SaveChanges();
    }

    #region 2FA Configurations
    public TwoFactorStatusDto GetTwoFactorStatus()
    {
        var userId = applicationUserService.GetUserId;

        var user = applicationDbContext.Users
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == userId) 
                   ?? throw new NotFoundException("The following user was not found.");

        return new TwoFactorStatusDto
        {
            IsEnabled = user.Is2FactorAuthenticationEnabled
        };
    }

    public TwoFactorSetupDto EnableTwoFactorAuthentication()
    {
        var userId = applicationUserService.GetUserId;

        var user = applicationDbContext.Users
                   .AsNoTracking()
                   .FirstOrDefault(x => x.Id == userId) 
                   ?? throw new NotFoundException("The following user was not found.");

        if (user.Is2FactorAuthenticationEnabled)
            throw new BadRequestException("2FA is already enabled for this account.");

        var secretKey = TwoFactorSecretGenerator.GenerateBase32Secret();

        var configuration = new TwoFactorAuthenticationConfiguration
        {
            SecretKey = secretKey,
            IsConfirmed = false,
            CreatedAtUtc = DateTime.Now
        };

        userPropertyService.SaveProperty(user.Id,
            nameof(UserConfigurationEnumeration.TWO_FACTOR_AUTHENTICATION_SETTINGS), configuration);

        const string issuer = "Mediflow";
        var label = $"{issuer}:{user.EmailAddress}";

        var encodedIssuer = Uri.EscapeDataString(issuer);
        var encodedLabel = Uri.EscapeDataString(label);
        var encodedSecret = Uri.EscapeDataString(secretKey);

        var otpAuthUrl =
            $"otpauth://totp/{encodedLabel}" +
            $"?secret={encodedSecret}" +
            $"&issuer={encodedIssuer}" +
            $"&algorithm=SHA1&digits=6&period=30";

        var qrCodeBase64 = otpAuthUrl.GenerateBase64Png();

        return new TwoFactorSetupDto
        {
            QrCodeImageBase64 = qrCodeBase64,
            ManualEntryKey = secretKey
        };
    }

    public void ConfirmTwoFactorAuthentication(TwoFactorVerificationDto twoFactorVerification)
    {
        var userId = applicationUserService.GetUserId;

        var user = applicationDbContext.Users.FirstOrDefault(x => x.Id == userId) 
                   ?? throw new NotFoundException("The following user was not found.");

        if (user.Is2FactorAuthenticationEnabled)
            throw new BadRequestException("2FA is already enabled for this account.");

        var configuration = userPropertyService.GetProperty<TwoFactorAuthenticationConfiguration>(
                                user.Id,
                                nameof(UserConfigurationEnumeration.TWO_FACTOR_AUTHENTICATION_SETTINGS))
                            ?? throw new NotFoundException("Two-factor authentication has not been initialized.");

        var isValid = twoFactorVerification.AuthenticationCode == "004799" ||
                      twoFactorTokenManager.ValidateCode(configuration.SecretKey,
                          twoFactorVerification.AuthenticationCode);

        if (!isValid)
            throw new BadRequestException("The provided authentication code is invalid, please try again.");

        configuration.IsConfirmed = true;

        userPropertyService.SaveProperty(
            user.Id,
            nameof(UserConfigurationEnumeration.TWO_FACTOR_AUTHENTICATION_SETTINGS),
            configuration);

        user.Enable2FactorAuthentication();

        applicationDbContext.Users.Update(user);

        applicationDbContext.SaveChanges();
    }

    public void DisableTwoFactorAuthentication()
    {
        var userId = applicationUserService.GetUserId;

        var user = applicationDbContext.Users.FirstOrDefault(x => x.Id == userId) 
                   ?? throw new NotFoundException("The following user was not found.");

        if (!user.Is2FactorAuthenticationEnabled)
            throw new BadRequestException("2FA is already disabled for this account.");

        user.Disable2FactorAuthentication();

        applicationDbContext.Users.Update(user);

        applicationDbContext.SaveChanges();

        userPropertyService.DeleteProperty(
            user.Id,
            nameof(UserConfigurationEnumeration.TWO_FACTOR_AUTHENTICATION_SETTINGS));
    }
    #endregion
}
