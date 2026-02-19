using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities.Audits;

public class AuditLog(
    string entityName,
    string entityId,
    ChangeType changeType,
    string? remarks = null,
    bool isAutomation = false,
    Guid? auditorId = null) : BaseEntity<Guid>
{
    public string EntityName { get; private set; } = entityName;

    public string EntityId { get; private set; } = entityId;

    public ChangeType ChangeType { get; private set; } = changeType;

    public string? Remarks { get; private set; } = remarks;

    public bool IsAutomation { get; private set; } = isAutomation;

    public DateTime AuditedDate { get; private set; } = DateTime.Now;

    [ForeignKey(nameof(Auditor))]
    public Guid? AuditorId { get; private set; } = auditorId;

    public static AuditLog Default => new AuditLog(string.Empty, string.Empty, ChangeType.Created);

    public virtual User? Auditor { get; set; }

    public virtual ICollection<AuditLogHistory> AuditLogHistories { get; set; } = new List<AuditLogHistory>();
}