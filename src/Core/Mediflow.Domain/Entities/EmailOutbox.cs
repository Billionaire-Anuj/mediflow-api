using Mediflow.Domain.Common.Enum;
using Mediflow.Domain.Common.Base;

namespace Mediflow.Domain.Entities;

public class EmailOutbox(
    string toEmail,
    string name,
    string subject,
    EmailProcess process,
    string payloadJson
) : BaseEntity<Guid>
{
    public string ToEmail { get; private set; } = toEmail;

    public string Name { get; private set; } = name;

    public string Subject { get; private set; } = subject;

    public EmailProcess Process { get; private set; } = process;

    public string PayloadJson { get; private set; } = payloadJson ?? "{}";

    public int AttemptCount { get; private set; } = 0;

    public DateTime NextAttemptDate { get; private set; } = DateTime.Now;

    public OutboxStatus Status { get; private set; } = OutboxStatus.Pending;

    public DateTime ScheduledDate { get; private set; } = DateTime.Now;

    public DateTime? SentDate { get; private set; }

    public string? LastError { get; private set; }

    #region Behavioral Methods
    public void MarkAsSending()
    {
        Status = OutboxStatus.Sending;
    }

    public void MarkAsSent()
    {
        Status = OutboxStatus.Sent;
        SentDate = DateTime.Now;
        LastError = null;
    }

    public void MarkAsFailed(string errorMessage, int maxRetries = 5)
    {
        AttemptCount++;
        LastError = errorMessage;
        Status = AttemptCount >= maxRetries ? OutboxStatus.Dead : OutboxStatus.Pending;

        var delays = new[]
        {
            TimeSpan.FromSeconds(10),
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15),
            TimeSpan.FromHours(1)
        };

        NextAttemptDate = DateTime.Now + delays[Math.Min(AttemptCount - 1, delays.Length - 1)];
    }
    #endregion
}
