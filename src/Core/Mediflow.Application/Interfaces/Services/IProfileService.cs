using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.DTOs.Profiles;
using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Authentication.Configurations._2FA;

namespace Mediflow.Application.Interfaces.Services;

public interface IProfileService : ITransientService
{
    ProfileDto GetProfile();

    RoleDto GetAssignedRole();
    
    void UpdateProfile(UpdateProfileDto profile);

    void UpdateProfileImage(UpdateProfileImageDto profileImage);
    
    void ChangePassword(ChangePasswordDto changePassword);

    #region 2FA Configurations
    TwoFactorStatusDto GetTwoFactorStatus();

    TwoFactorSetupDto EnableTwoFactorAuthentication();

    void ConfirmTwoFactorAuthentication(TwoFactorVerificationDto twoFactorVerification);

    void DisableTwoFactorAuthentication();
    #endregion
}