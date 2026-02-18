using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Emails;

public static class EmailOutboxExtensionMethods
{
    public static EmailOutboxDto ToEmailOutboxDto(this EmailOutbox emailOutbox)
    {
        return new EmailOutboxDto()
        {
            Id = emailOutbox.Id,
            IsActive = emailOutbox.IsActive,
            Subject = emailOutbox.Subject,
            ToEmail = emailOutbox.ToEmail,
            Name = emailOutbox.Name,
            Process = emailOutbox.Process,
            PayloadJson = emailOutbox.PayloadJson,
            AttemptCount = emailOutbox.AttemptCount,
            NextAttemptDate = emailOutbox.NextAttemptDate,
            Status = emailOutbox.Status,
            ScheduledDate = emailOutbox.ScheduledDate,
            SentDate = emailOutbox.SentDate,
            LastError = emailOutbox.LastError
        };
    }
}