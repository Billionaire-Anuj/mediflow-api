using System.ComponentModel.DataAnnotations;

namespace mediflow.Entities;

public sealed class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(120)]
    public string FullName { get; set; } = "";

    [MaxLength(180)]
    public string Email { get; set; } = "";

    [MaxLength(30)]
    public string? Phone { get; set; }

    [MaxLength(400)]
    public string PasswordHash { get; set; } = "";

    [MaxLength(500)]
    public string? ProfilePictureUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int PointsBalance { get; set; } = 0;

    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<PendingPointPurchases> PendingPointPurchases { get; set; } = new List<PendingPointPurchases>();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}