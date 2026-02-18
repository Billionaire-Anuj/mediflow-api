using Mediflow.Domain.Common.Enum;

namespace Mediflow.Application.DTOs.Emails;

public class EmailDto
{
    #region SMTP Settings
    public required EmailSmtpDto Smtp { get; set; }
    #endregion

    #region Required Fields
    public string FullName { get; set; } = string.Empty;
    
    public string ToEmailAddress { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public EmailProcess Process { get; set; }

    public string Remarks { get; set; } = string.Empty;
    #endregion

    #region Calculated Fields
    public string Body { get; set; } = string.Empty;

    public List<KeyValuePair<string, string>> PlaceHolders { get; set; } = [];
    #endregion

    #region Attachment Files
    public string? FileUrl { get; set; }

    public string? FileName { get; set; }
    #endregion

    #region Optional Fields
    public string? Cc { get; set; }
    #endregion

    #region Application Fields
    public string VacancyTitle { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public string CandidateName { get; set; } = string.Empty;

    public string CandidateEmailAddress { get; set; } = string.Empty;

    public string CandidatePhoneNumber { get; set; } = string.Empty;

    public string AttachmentUrl { get; set; } = string.Empty;
    #endregion
    
    #region Reset Password Fields
    public string Username { get; set; } = string.Empty;
    
    public string ApplicationUserName { get; set; } = string.Empty;

    public string ApplicationRoleName { get; set; } = string.Empty;
    
    public string UserCredentials { get; set; } = string.Empty;
    
    public string TemporaryPassword { get; set; } = string.Empty;
    
    public string SupportEmail { get; set; } = string.Empty;
    #endregion

    #region Forgot Password Fields
    public string UserEmailAddress { get; set; } = string.Empty;

    public int OtpExpiryMinutes { get; set; }

    public string Otp { get; set; } = string.Empty;
    #endregion
}