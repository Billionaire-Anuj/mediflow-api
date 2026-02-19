using MimeKit;
using MailKit.Security;
using Mediflow.Domain.Common;
using Mediflow.Domain.Common.Enum;
using Microsoft.AspNetCore.Hosting;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Emails;
using Mediflow.Application.Interfaces.Services;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Mediflow.Infrastructure.Implementation.Services;

public class EmailService(IWebHostEnvironment webHostEnvironment) : IEmailService
{
    private const string EmailTemplatesFilePath = Constants.FilePath.EmailTemplatesFilePath;

    public async Task SendEmailAsync(EmailDto email)
    {
        try
        {
            using var emailMessage = new MimeMessage();

            #region Senders and Receivers
            var emailFrom = new MailboxAddress("Mediflow", email.Smtp.Username);
            var emailTo = new MailboxAddress(email.FullName, email.ToEmailAddress);
            var emailBcc = new MailboxAddress("Mediflow", "mediflow@root.com");

            emailMessage.From.Add(emailFrom);
            emailMessage.To.Add(emailTo);
            emailMessage.Bcc.Add(emailBcc);

            if (!string.IsNullOrEmpty(email.Cc))
            {
                var emailCc = new MailboxAddress(email.Cc, email.Cc);
                emailMessage.Cc.Add(emailCc);
            }
            #endregion

            #region Mail Content and Details
            emailMessage.Subject = email.Subject;

            email.PlaceHolders = GetPlaceHolders(email);
            email.Body = PrepareTemplate(email);

            var emailBodyBuilder = new BodyBuilder()
            {
                HtmlBody = email.Body
            };
            #endregion

            #region Mail Attachments
            if (!string.IsNullOrWhiteSpace(email.FileUrl))
            {
                if (!File.Exists(email.FileUrl))
                    throw new BadRequestException("Attachment file not found on server.");

                var entity = await emailBodyBuilder.Attachments.AddAsync(email.FileUrl);

                if (!string.IsNullOrWhiteSpace(email.FileName))
                    entity.ContentDisposition.FileName = email.FileName;
            }
            #endregion

            #region Email Finalization
            emailMessage.Body = emailBodyBuilder.ToMessageBody();
            #endregion

            #region Fire and Trigger Mail
            using var mailClient = new SmtpClient();

            mailClient.CheckCertificateRevocation = false;

            await mailClient.ConnectAsync(email.Smtp.Host, email.Smtp.Port, SecureSocketOptions.StartTls);
            await mailClient.AuthenticateAsync(email.Smtp.Username, email.Smtp.Password);
            await mailClient.SendAsync(emailMessage);
            await mailClient.DisconnectAsync(true);
            #endregion
        }
        catch (Exception exception)
        {
            throw new BadRequestException($"An email could not be triggered to the respective email address, due to the following reason(s): {exception.Message}.");
        }
    }
    
    private static List<KeyValuePair<string, string>> GetPlaceHolders(EmailDto email)
    {
        var result = new List<KeyValuePair<string, string>>();

        switch (email.Process)
        {
            case EmailProcess.ForgotPasswordConfirmation:
                result.Add(new KeyValuePair<string, string>("{$date}", DateTime.Now.ToString("dd MMMM yyyy")));
                result.Add(new KeyValuePair<string, string>("{$year}", DateTime.Now.ToString("yyyy")));
                result.Add(new KeyValuePair<string, string>("{$userName}", email.Username));
                result.Add(new KeyValuePair<string, string>("{$otpExpiryMinutes}", email.OtpExpiryMinutes.ToString()));
                result.Add(new KeyValuePair<string, string>("{$otpCode}", email.Otp));
                result.Add(new KeyValuePair<string, string>("{$userEmailAddress}", email.UserEmailAddress));
                result.Add(new KeyValuePair<string, string>("{$supportEmail}", email.SupportEmail));
                break;

            case EmailProcess.PasswordResetConfirmation:
                result.Add(new KeyValuePair<string, string>("{$date}", DateTime.Now.ToString("dd MMMM yyyy")));
                result.Add(new KeyValuePair<string, string>("{$year}", DateTime.Now.ToString("yyyy")));
                result.Add(new KeyValuePair<string, string>("{$userName}", email.Username));
                result.Add(new KeyValuePair<string, string>("{$applicationUserName}", email.ApplicationUserName));
                result.Add(new KeyValuePair<string, string>("{$applicationRoleName}", email.ApplicationRoleName));
                result.Add(new KeyValuePair<string, string>("{$userCredentials}", email.UserCredentials));
                result.Add(new KeyValuePair<string, string>("{$temporaryPassword}", email.TemporaryPassword)); 
                result.Add(new KeyValuePair<string, string>("{$supportEmail}", email.SupportEmail));
                break;
        }

        return result;
    }

    private static string UpdatePlaceHolders(string text, IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        if (string.IsNullOrEmpty(text)) return text;

        foreach (var placeholder in keyValuePairs.Where(placeholder => text.Contains(placeholder.Key)))
        {
            text = text.Replace(placeholder.Key, placeholder.Value);
        }

        return text;
    }

    private string PrepareTemplate(EmailDto email)
    {
        return UpdatePlaceHolders(GetEmailBody(email.Process.ToString()), email.PlaceHolders);
    }

    private string GetEmailBody(string templateName)
    {           
        return File.ReadAllText(Path.Combine(webHostEnvironment.WebRootPath, EmailTemplatesFilePath, $"{templateName}.html"));
    }
}
