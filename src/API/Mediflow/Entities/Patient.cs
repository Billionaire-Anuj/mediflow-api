using System.ComponentModel.DataAnnotations;
using Mediflow.Entities.Enums;

namespace mediflow.Entities;

public sealed class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }
    public Gender? Gender { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(30)]
    public string? BloodGroup { get; set; }

    [MaxLength(80)]
    public string? EmergencyContactName { get; set; }

    [MaxLength(30)]
    public string? EmergencyContactPhone { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
}