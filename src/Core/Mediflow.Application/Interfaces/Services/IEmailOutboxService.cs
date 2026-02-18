using Mediflow.Domain.Common.Enum;
using Mediflow.Application.DTOs.Emails;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IEmailOutboxService : ITransientService
{
    List<EmailOutboxDto> GetAllEmailOutboxes(
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
        DateTime? maximumSentDate = null);

    List<EmailOutboxDto> GetAllEmailOutboxes(
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
        DateTime? maximumSentDate = null);

    EmailOutboxDto GetEmailOutboxById(Guid emailOutboxId);

    Task ProcessEmailOutboxAsync(Guid emailOutboxId);
}