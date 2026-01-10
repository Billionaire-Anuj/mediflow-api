using System.ComponentModel.DataAnnotations;

namespace mediflow.Entities;

public sealed class Doctor
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [MaxLength(120)]
    public string Specialty { get; set; } = "";

    [MaxLength(50)]
    public string? LicenseNumber { get; set; }

    [MaxLength(2000)]
    public string? Bio { get; set; }

    public int DefaultSlotMinutes { get; set; } = 15;

    public decimal? ConsultationFee { get; set; }

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecordsAuthored { get; set; } = new List<MedicalRecord>();
    public ICollection<LabResult> LabResultsOrdered { get; set; } = new List<LabResult>();
    public ICollection<LabResult> LabResultsReviewed { get; set; } = new List<LabResult>();
}