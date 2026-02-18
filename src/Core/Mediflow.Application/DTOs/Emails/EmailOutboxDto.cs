using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Emails;

// TODO: Shall we provide an update method for a set of following properties?
public class EmailOutboxDto : BaseDto
{
    public string ToEmail { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public EmailProcess Process { get; set; }

    public string PayloadJson { get; set; } = string.Empty;

    public int AttemptCount { get; set; } = 0;

    public DateTime NextAttemptDate { get; set; }

    public OutboxStatus Status { get; set; }

    public DateTime ScheduledDate { get; set; }

    public DateTime? SentDate { get; set; }

    public string? LastError { get; set; }
}