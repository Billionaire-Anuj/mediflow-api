using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities.Audits;

public class AuditLogHistory(
    Guid auditLogId,
    string fieldName,
    FieldDataType fieldDataType,
    string? oldValue,
    string? newValue,
    string? remarks = null) : BaseEntity<Guid>
{
    [ForeignKey(nameof(AuditLog))]
    public Guid AuditLogId { get; private set; } = auditLogId;

    public string FieldName { get; private set; } = fieldName;

    public FieldDataType FieldDataType { get; private set; } = fieldDataType;

    public string? OldValue { get; private set; } = oldValue;

    public string? NewValue { get; private set; } = newValue;

    public string? Remarks { get; private set; } = remarks;

    public virtual AuditLog AuditLog { get; set; } = AuditLog.Default;
}