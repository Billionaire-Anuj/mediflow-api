using System.ComponentModel.DataAnnotations;
using Mediflow.Entities.Enums;

namespace mediflow.Entities;

public sealed class LabResult
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Guid? MedicalRecordId { get; set; }
    public MedicalRecord? MedicalRecord { get; set; }

    public Guid? OrderedByDoctorId { get; set; }
    public Doctor? OrderedByDoctor { get; set; }

    public Guid? ReviewedByDoctorId { get; set; }
    public Doctor? ReviewedByDoctor { get; set; }

    [MaxLength(150)]
    public string TestName { get; set; } = "";

    [MaxLength(4000)]
    public string? ResultSummary { get; set; }

    public DateOnly ResultDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [MaxLength(500)]
    public string? ReportFileUrl { get; set; }

    public LabResultStatus Status { get; set; } = LabResultStatus.Pending;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}