using System.ComponentModel.DataAnnotations;

namespace Mediflow.Domain.Common.Base;

public class BaseEntity<TPrimaryKey>
{
    [Key]
    public TPrimaryKey Id { get; private set; } = default!;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Default Constructor: Auto-Generated Identifier for GUID-based Entities (UUID v7).
    /// </summary>
    protected BaseEntity()
    {
    }

    /// <summary>
    /// Explicit Constructor: Manually Generated or Supplied Identifier.
    /// </summary>
    protected BaseEntity(TPrimaryKey id)
    {
        Id = id;
    }

    public void ActivateDeactivateEntity()
    {
        IsActive = !IsActive;
    }

    public void AssignIdentifier(TPrimaryKey identifier)
    {
        Id = identifier;
    }
}