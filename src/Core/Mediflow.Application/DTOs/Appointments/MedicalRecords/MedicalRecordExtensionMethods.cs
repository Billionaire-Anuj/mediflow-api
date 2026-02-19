using Mediflow.Domain.Entities;

namespace Mediflow.Application.DTOs.Appointments.MedicalRecords;

public static class MedicalRecordExtensionMethods
{
    public static MedicalRecordDto ToMedicalRecordDto(this MedicalRecord medicalRecord)
    {
        return new MedicalRecordDto
        {
            Id = medicalRecord.Id,
            Notes = medicalRecord.Notes,
            IsActive = medicalRecord.IsActive,
            Diagnosis = medicalRecord.Diagnosis,
            Treatment = medicalRecord.Treatment,
            Prescriptions = medicalRecord.Prescriptions
        };
    }
}