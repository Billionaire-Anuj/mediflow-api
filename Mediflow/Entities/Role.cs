using System.ComponentModel.DataAnnotations;

namespace mediflow.Entities;

public sealed class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(50)]
    public string Name { get; set; } = "";

    [MaxLength(200)]
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}