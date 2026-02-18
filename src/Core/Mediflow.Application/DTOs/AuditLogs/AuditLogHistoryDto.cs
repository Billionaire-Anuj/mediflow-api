using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.AuditLogs;

public class AuditLogHistoryDto : BaseDto
{
    public string FieldName { get; set; } = string.Empty;

    public FieldDataType FieldDataType { get; set; }

    public object? OldValue { get; set; } = string.Empty;

    public object? NewValue { get; set; } = string.Empty;

    public string? Remarks { get; set; } = string.Empty;
}