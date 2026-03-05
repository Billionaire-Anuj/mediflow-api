using Mediflow.Domain.Common.Enum;
using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.DTOs.Patients;
using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Appointments.Diagnostics;
using Mediflow.Application.DTOs.Appointments.Medications;
using Mediflow.Application.DTOs.Appointments.MedicalRecords;
using Mediflow.Application.DTOs.Doctors.Schedules.Timeslots;

namespace Mediflow.Application.DTOs.Appointments;

public class AppointmentDto : BaseDto
{
    public DoctorProfileDto Doctor { get; set; } = new();

    public PatientProfileDto Patient { get; set; } = new();

    public DateTime BookedDate { get; set; }

    public TimeslotDto Timeslot { get; set; } = new();

    public DateTime? CancelledDate { get; set; }

    public AppointmentStatus Status { get; set; }

    public string? Notes { get; set; } = string.Empty;

    public string? Symptoms { get; set; } = string.Empty;

    public decimal Fee { get; set; }

    public bool IsPaidViaGateway { get; set; }

    public bool IsPaidViaOfflineMedium { get; set; }

    public string? CancellationReason { get; set; }

    public bool HasReview { get; set; }

    public int? ReviewRating { get; set; }

    public MedicalRecordDto MedicalRecords { get; set; } = new();

    public List<AppointmentMedicationsDto> Medications { get; set; } = new();

    public List<AppointmentDiagnosticsDto> Diagnostics { get; set; } = new();
}
