using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medical.GrpcService.Entities;

public class PendingPointPurchases
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string PatientId { get; set; }

    [ForeignKey(nameof(PatientId))]
    public Patient Patient { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Points { get; set; }

    [Required]
    public string Pidx { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}