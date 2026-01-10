using System.ComponentModel.DataAnnotations;

namespace mediflow.Entities;

public sealed class MedicalRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public DateOnly VisitDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [MaxLength(500)]
    public string? ChiefComplaint { get; set; }

    [MaxLength(4000)]
    public string? Diagnosis { get; set; }

    [MaxLength(4000)]
    public string? TreatmentPlan { get; set; }

    [MaxLength(4000)]
    public string? Prescriptions { get; set; }

    [MaxLength(4000)]
    public string? Notes { get; set; }

    public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
}