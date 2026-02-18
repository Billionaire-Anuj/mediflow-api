using Mediflow.Domain.Common.Enum;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.AuditLogs;

public class AuditLogDto : BaseDto
{
    public UserDto? User { get; set; } = new();

    public ChangeType ChangeType { get; set; }

    public string? Remarks { get; set; } = string.Empty;

    public bool IsAutomation { get; set; }

    public DateTime AuditedDate { get; set; }

    public List<AuditLogHistoryDto> AuditLogHistories { get; set; } = [];
}