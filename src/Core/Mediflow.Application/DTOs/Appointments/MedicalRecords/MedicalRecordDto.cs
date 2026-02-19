using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Appointments.MedicalRecords;

public class MedicalRecordDto : BaseDto
{
    public string Diagnosis { get; set; } = string.Empty;

    public string Treatment { get; set; } = string.Empty;

    public string Prescriptions { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
}