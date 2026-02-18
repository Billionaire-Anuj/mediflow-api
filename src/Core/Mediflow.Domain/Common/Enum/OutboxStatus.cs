namespace Mediflow.Domain.Common.Enum;

public enum OutboxStatus
{
    Pending,
    Sending,
    Sent,
    Failed,
    Dead
}