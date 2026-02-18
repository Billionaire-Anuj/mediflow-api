using Mediflow.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Common.Base;

public class AuditableEntity<TPrimaryKey> : BaseEntity<TPrimaryKey>
{
    [ForeignKey(nameof(CreatedUser))]
    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(LastModifiedUser))]
    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedAt { get; set; }

    [ForeignKey(nameof(DeletedUser))]
    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? CreatedUser { get; set; }

    public virtual User? LastModifiedUser { get; set; }

    public virtual User? DeletedUser { get; set; }

    public void AssignCreatedBy(Guid createdBy)
    {
        CreatedBy = createdBy;
    }

    public void AssignLastModifiedBy(Guid lastModifiedBy)
    {
        LastModifiedBy = lastModifiedBy;
    }
}