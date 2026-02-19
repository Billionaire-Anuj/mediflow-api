using System.Text.Json;
using System.Globalization;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Settings;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Emails;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mediflow.Infrastructure.Implementation.Services;

public class EmailOutboxService(
    IServiceProvider serviceProvider,
    ILogger<EmailOutboxService> logger,
    IOptions<SmtpSettings> smtpSettings) : BackgroundService, IEmailOutboxService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    #region Background Service Implementation
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessPendingEmailsAsync();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
    #endregion

    public List<EmailOutboxDto> GetAllEmailOutboxes(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? toEmail = null,
        string? name = null,
        string? subject = null,
        List<EmailProcess>? emailProcess = null,
        int? minimumAttemptCount = null,
        int? maximumAttemptCount = null,
        DateTime? minimumNextAttemptDate = null,
        DateTime? maximumNextAttemptDate = null,
        List<OutboxStatus>? outboxStatuses = null,
        DateTime? minimumScheduledDate = null,
        DateTime? maximumScheduledDate = null,
        DateTime? minimumSentDate = null,
        DateTime? maximumSentDate = null)
    {
        using var scope = serviceProvider.CreateScope();
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var emailProcessIdentifiers = emailProcess != null ? new HashSet<EmailProcess>(emailProcess) : null;
        var outboxStatusesIdentifiers = outboxStatuses != null ? new HashSet<OutboxStatus>(outboxStatuses) : null;

        // TODO: Use of the following normalization for global search throughout the application.
        var normalizedName = name?.Trim().ToLower();
        var normalizedToEmail = toEmail?.Trim().ToLower();
        var normalizedSubject = subject?.Trim().ToLower();
        var normalizedGlobalSearch = globalSearch?.Trim().ToLower();

        var emailOutboxModels = applicationDbContext.EmailOutboxes
            .Where(x => 
                (string.IsNullOrEmpty(normalizedGlobalSearch)
                    || x.Name.ToLower().Contains(normalizedGlobalSearch)
                    || x.ToEmail.ToLower().Contains(normalizedGlobalSearch)
                    || x.Subject.ToLower().Contains(normalizedGlobalSearch)
                    || x.Process.ToString().ToLower().Contains(normalizedGlobalSearch)
                    // TODO: Use of CultureInfo.InvariantCulture for all numeric comparisons and searched.
                    || x.AttemptCount.ToString(CultureInfo.InvariantCulture).ToLower().Contains(normalizedGlobalSearch)
                    || x.NextAttemptDate.ToString("dd-MM-yyyy").ToLower().Contains(normalizedGlobalSearch)
                    || x.Status.ToString().ToLower().Contains(normalizedGlobalSearch)
                    || x.ScheduledDate.ToString("dd-MM-yyyy").ToLower().Contains(normalizedGlobalSearch)
                    || (x.SentDate != null && x.SentDate.Value.ToString("dd-MM-yyyy").ToLower().Contains(normalizedGlobalSearch)))
                && (isActive == null || isActive.Contains(x.IsActive))
                && (string.IsNullOrEmpty(normalizedToEmail) || x.ToEmail.ToLower().Contains(normalizedToEmail))
                && (string.IsNullOrEmpty(normalizedName) || x.Name.ToLower().Contains(normalizedName))
                && (string.IsNullOrEmpty(normalizedSubject) || x.Subject.ToLower().Contains(normalizedSubject))
                && (emailProcessIdentifiers == null || emailProcessIdentifiers.Contains(x.Process))
                && (minimumAttemptCount == null || x.AttemptCount >= minimumAttemptCount)
                && (maximumAttemptCount == null || x.AttemptCount <= maximumAttemptCount)
                && (minimumNextAttemptDate == null || x.NextAttemptDate >= minimumNextAttemptDate)
                && (maximumNextAttemptDate == null || x.NextAttemptDate <= maximumNextAttemptDate)
                && (outboxStatusesIdentifiers == null || outboxStatusesIdentifiers.Contains(x.Status))
                && (minimumScheduledDate == null || x.ScheduledDate >= minimumScheduledDate)
                && (maximumScheduledDate == null || x.ScheduledDate <= maximumScheduledDate)
                && (minimumSentDate == null || (x.SentDate != null && x.SentDate >= minimumSentDate))
                && (maximumSentDate == null || (x.SentDate != null && x.SentDate <= maximumSentDate)))
            .OrderBy(x => orderBys)
            .AsNoTracking();

        rowCount = emailOutboxModels.Count();

        return emailOutboxModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToEmailOutboxDto())
            .ToList();
    }

    public List<EmailOutboxDto> GetAllEmailOutboxes(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        string? toEmail = null,
        string? name = null,
        string? subject = null,
        List<EmailProcess>? emailProcess = null,
        int? minimumAttemptCount = null,
        int? maximumAttemptCount = null,
        DateTime? minimumNextAttemptDate = null,
        DateTime? maximumNextAttemptDate = null,
        List<OutboxStatus>? outboxStatuses = null,
        DateTime? minimumScheduledDate = null,
        DateTime? maximumScheduledDate = null,
        DateTime? minimumSentDate = null,
        DateTime? maximumSentDate = null)
    {
        using var scope = serviceProvider.CreateScope();
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var emailProcessIdentifiers = emailProcess != null ? new HashSet<EmailProcess>(emailProcess) : null;
        var outboxStatusesIdentifiers = outboxStatuses != null ? new HashSet<OutboxStatus>(outboxStatuses) : null;

        var normalizedName = name?.Trim().ToLower();
        var normalizedToEmail = toEmail?.Trim().ToLower();
        var normalizedSubject = subject?.Trim().ToLower();
        var normalizedGlobalSearch = globalSearch?.Trim().ToLower();

        var emailOutboxModels = applicationDbContext.EmailOutboxes
            .Where(x => 
                (string.IsNullOrEmpty(normalizedGlobalSearch)
                    || x.Name.ToLower().Contains(normalizedGlobalSearch)
                    || x.ToEmail.ToLower().Contains(normalizedGlobalSearch)
                    || x.Subject.ToLower().Contains(normalizedGlobalSearch)
                    || x.Process.ToString().ToLower().Contains(normalizedGlobalSearch)
                    // TODO: Use of CultureInfo.InvariantCulture for all numeric comparisons and searched.
                    || x.AttemptCount.ToString(CultureInfo.InvariantCulture).ToLower().Contains(normalizedGlobalSearch)
                    || x.NextAttemptDate.ToString("dd-MM-yyyy").ToLower().Contains(normalizedGlobalSearch)
                    || x.Status.ToString().ToLower().Contains(normalizedGlobalSearch)
                    || x.ScheduledDate.ToString("dd-MM-yyyy").ToLower().Contains(normalizedGlobalSearch)
                    || (x.SentDate != null && x.SentDate.Value.ToString("dd-MM-yyyy").ToLower().Contains(normalizedGlobalSearch)))
                && (isActive == null || isActive.Contains(x.IsActive))
                && (string.IsNullOrEmpty(normalizedToEmail) || x.ToEmail.ToLower().Contains(normalizedToEmail))
                && (string.IsNullOrEmpty(normalizedName) || x.Name.ToLower().Contains(normalizedName))
                && (string.IsNullOrEmpty(normalizedSubject) || x.Subject.ToLower().Contains(normalizedSubject))
                && (emailProcessIdentifiers == null || emailProcessIdentifiers.Contains(x.Process))
                && (minimumAttemptCount == null || x.AttemptCount >= minimumAttemptCount)
                && (maximumAttemptCount == null || x.AttemptCount <= maximumAttemptCount)
                && (minimumNextAttemptDate == null || x.NextAttemptDate >= minimumNextAttemptDate)
                && (maximumNextAttemptDate == null || x.NextAttemptDate <= maximumNextAttemptDate)
                && (outboxStatusesIdentifiers == null || outboxStatusesIdentifiers.Contains(x.Status))
                && (minimumScheduledDate == null || x.ScheduledDate >= minimumScheduledDate)
                && (maximumScheduledDate == null || x.ScheduledDate <= maximumScheduledDate)
                && (minimumSentDate == null || (x.SentDate != null && x.SentDate >= minimumSentDate))
                && (maximumSentDate == null || (x.SentDate != null && x.SentDate <= maximumSentDate)))
            .OrderBy(x => orderBys)
            .AsNoTracking();

        return emailOutboxModels.Select(x => x.ToEmailOutboxDto()).ToList();
    }

    public EmailOutboxDto GetEmailOutboxById(Guid emailOutboxId)
    {
        using var scope = serviceProvider.CreateScope();
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var emailOutboxModel = applicationDbContext.EmailOutboxes
                                   .AsNoTracking()
                                   .FirstOrDefault(x => x.Id == emailOutboxId) 
                               ?? throw new NotFoundException($"Email outbox with the identifier of '{emailOutboxId}' could not be found.");

        return emailOutboxModel.ToEmailOutboxDto();
    }

    public async Task ProcessEmailOutboxAsync(Guid emailOutboxId)
    {
        using var scope = serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var emailOutboxModel = await applicationDbContext.EmailOutboxes.FindAsync(emailOutboxId)
                               ?? throw new NotFoundException($"Email outbox with the identifier of '{emailOutboxId}' could not be found.");

        if (emailOutboxModel.Status == OutboxStatus.Sent)
            throw new BadRequestException("The email outbox is already sent.");

        await ProcessSingleEmailOutboxAsync(emailOutboxModel, applicationDbContext, emailService);
    }

    #region Private Methods
    /// <summary>
    /// The following method will process all pending emails and will not be exposed as an REST API Endpoint.
    /// </summary>
    private async Task ProcessPendingEmailsAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var emailOutboxes = applicationDbContext.EmailOutboxes
            .Where(x => x.Status == OutboxStatus.Pending)
            .OrderBy(x => x.ScheduledDate);

        foreach (var emailOutbox in emailOutboxes)
        {
            await ProcessSingleEmailOutboxAsync(emailOutbox, applicationDbContext, emailService);
        }
    }

    private async Task ProcessSingleEmailOutboxAsync(EmailOutbox emailOutbox, IApplicationDbContext applicationDbContext, IEmailService emailService)
    {
        try
        {
            emailOutbox.MarkAsSending();

            applicationDbContext.EmailOutboxes.Update(emailOutbox);

            var emailModel = new EmailDto()
            {
                Smtp = new EmailSmtpDto()
                {
                    Host = _smtpSettings.Host,
                    Port = _smtpSettings.Port,
                    Username = _smtpSettings.Username,
                    Password = _smtpSettings.Password
                }
            };

            switch (emailOutbox.Process)
            {
                case EmailProcess.ForgotPasswordConfirmation:
                {
                    var forgotPasswordEmail = JsonSerializer.Deserialize<ForgotPasswordEmailDto>(emailOutbox.PayloadJson);

                    if (forgotPasswordEmail != null)
                    {
                        var userModel = await applicationDbContext.Users.FindAsync(forgotPasswordEmail.UserId) 
                                            ?? throw new NotFoundException($"User with identifier '{forgotPasswordEmail.UserId}' not found.");

                        emailModel.FullName = emailOutbox.Name;
                        emailModel.ToEmailAddress = emailOutbox.ToEmail;
                        emailModel.Subject = emailOutbox.Subject;
                        emailModel.Process = EmailProcess.ForgotPasswordConfirmation;

                        emailModel.Username = userModel.Username;
                        emailModel.Otp = forgotPasswordEmail.Otp;
                        emailModel.UserEmailAddress = userModel.EmailAddress;
                        emailModel.OtpExpiryMinutes = forgotPasswordEmail.OtpExpiryMinutes;
                        emailModel.SupportEmail = "support@mediflow.com";
                    }

                    break;
                }
                case EmailProcess.PasswordResetConfirmation:
                {
                    var resetPasswordEmail = JsonSerializer.Deserialize<ResetPasswordEmailDto>(emailOutbox.PayloadJson);

                    if (resetPasswordEmail != null)
                    {
                        var userModel = await applicationDbContext.Users.FindAsync(resetPasswordEmail.UserId) 
                                            ?? throw new NotFoundException($"User with identifier '{resetPasswordEmail.UserId}' not found.");

                        var applicationUserModel = await applicationDbContext.Users.FindAsync(resetPasswordEmail.ApplicationUserId)
                                                        ?? throw new NotFoundException($"Application user with identifier '{resetPasswordEmail.ApplicationUserId}' not found.");

                        var applicationRoleModel = await applicationDbContext.Roles.FindAsync(applicationUserModel.RoleId)
                                                   ?? throw new NotFoundException($"Application role with identifier '{applicationUserModel.RoleId}' not found.");

                        emailModel.FullName = emailOutbox.Name;
                        emailModel.ToEmailAddress = emailOutbox.ToEmail;
                        emailModel.Subject = emailOutbox.Subject;
                        emailModel.Process = EmailProcess.PasswordResetConfirmation;

                        emailModel.Username = userModel.Username;
                        emailModel.ApplicationUserName = applicationUserModel.Name;
                        emailModel.ApplicationRoleName = applicationRoleModel.Name;
                        emailModel.UserCredentials = $"{userModel.Username} or {userModel.EmailAddress}";
                        emailModel.TemporaryPassword = resetPasswordEmail.Password;
                        emailModel.SupportEmail = "support@mediflow.com";
                    }

                    break;
                }

                default:
                    throw new NotSupportedException($"Email process '{emailOutbox.Process}' is not supported in the outbox handler.");
            }

            await emailService.SendEmailAsync(emailModel);

            emailOutbox.MarkAsSent();

            applicationDbContext.EmailOutboxes.Update(emailOutbox);
        }
        catch (Exception exception)
        {
            emailOutbox.MarkAsFailed(exception.Message);

            applicationDbContext.EmailOutboxes.Update(emailOutbox);

            logger.LogError($"An exception occured while sending email to {emailOutbox.ToEmail} due to the following reason(s): {exception.Message}.");
        }

        applicationDbContext.SaveChanges();
    }
    #endregion
}