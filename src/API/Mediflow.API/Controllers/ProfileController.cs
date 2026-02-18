using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.DTOs.Profiles;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Authentication.Configurations._2FA;

namespace Mediflow.API.Controllers;

public class ProfileController(IProfileService profileService) : BaseController<ProfileController>
{
    [HttpGet]
    [Documentation("GetProfile", "Retrieve the logged in user's respective profile details.")]
    public ResponseDto<ProfileDto> GetProfile()
    {
        var result = profileService.GetProfile();

        return new ResponseDto<ProfileDto>(
            (int)HttpStatusCode.OK,
            "Successfully fetched user's profile details.",
            result);
    }

    [HttpGet("assigned/role")]
    [Documentation("GetAssignedRole", "Retrieves the logged in user's assigned roles.")]
    public ResponseDto<RoleDto> GetAssignedRole()
    {
        var result = profileService.GetAssignedRole();

        return new ResponseDto<RoleDto>(
            (int)HttpStatusCode.OK,
            "Successfully fetched user's profile details.",
            result);
    }
    
    [HttpPut]
    [Documentation("UpdateProfile", "Update the logged in user's profile details.")]
    public ResponseDto<bool> UpdateProfile([FromBody] UpdateProfileDto profile)
    {
        profileService.UpdateProfile(profile);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Profile details updated successfully.",
            true);
    }
    
    [HttpPut("image")]
    [Documentation("UpdateProfileImage", "Update the logged in user's profile image.")]
    public ResponseDto<bool> UpdateProfileImage([FromForm] UpdateProfileImageDto profileImage)
    {
        profileService.UpdateProfileImage(profileImage);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Profile image updated successfully.",
            true);
    }
    
    [HttpPut("change/password")]
    [Documentation("ChangePassword", "Change the logged in user's password.")]
    public ResponseDto<bool> ChangePassword([FromBody] ChangePasswordDto changePassword)
    {
        profileService.ChangePassword(changePassword);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Password changed successfully.",
            true);
    }

    #region 2FA Configurations
    [HttpGet("configuration/2fa")]
    [Documentation("GetTwoFactorStatus", "Retrieve the logged in user's 2FA status.")]
    public ResponseDto<TwoFactorStatusDto> GetTwoFactorStatus()
    {
        var result = profileService.GetTwoFactorStatus();

        return new ResponseDto<TwoFactorStatusDto>(
            (int)HttpStatusCode.OK,
            "Successfully fetched user's 2FA status.",
            result);
    }

    [HttpPost("configuration/2fa/enable")]
    [Documentation("EnableTwoFactorAuthentication", "Enables the logged in user's 2FA status.")]
    public ResponseDto<TwoFactorSetupDto> EnableTwoFactorAuthentication()
    {
        var result = profileService.EnableTwoFactorAuthentication();

        return new ResponseDto<TwoFactorSetupDto>(
            (int)HttpStatusCode.OK,
            "Successfully enabled user's 2FA status.",
            result);
    }

    [HttpPost("configuration/2fa/confirm")]
    [Documentation("ConfirmTwoFactorAuthentication", "Confirms the logged in user's 2FA status.")]
    public ResponseDto<bool> ConfirmTwoFactorAuthentication([FromBody] TwoFactorVerificationDto twoFactorVerification)
    {
        profileService.ConfirmTwoFactorAuthentication(twoFactorVerification);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Successfully confirmed user's 2FA status.",
            true);
    }

    [HttpDelete("configuration/2fa/disable")]
    [Documentation("DisableTwoFactorAuthentication", "Disables the logged in user's 2FA status.")]
    public ResponseDto<bool> DisableTwoFactorAuthentication()
    {
        profileService.DisableTwoFactorAuthentication();

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Successfully disabled user's 2FA status.",
            true);
    }
    #endregion
}