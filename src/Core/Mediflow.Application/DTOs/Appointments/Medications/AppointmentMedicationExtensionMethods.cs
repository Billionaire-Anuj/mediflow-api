using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.DTOs.Medicines;

namespace Mediflow.Application.DTOs.Appointments.Medications;

public static class AppointmentMedicationExtensionMethods
{
    public static AppointmentMedicationsDto ToAppointmentMedicationsDto(this AppointmentMedications appointmentMedications)
    {
        return new AppointmentMedicationsDto()
        {
            Id = appointmentMedications.Id,
            Notes = appointmentMedications.Notes,
            Status = appointmentMedications.Status,
            IsActive = appointmentMedications.IsActive,
            CompletedDate = appointmentMedications.CompletedDate,
            Pharmacist = appointmentMedications.Pharmacist?.ToUserDto(),
            Drugs = appointmentMedications.Drugs.Select(x => x.ToAppointmentMedicationDrugsDto()).ToList()
        };
    }

    private static AppointmentMedicationDrugsDto ToAppointmentMedicationDrugsDto(this AppointmentMedicationDrugs appointmentMedicationDrugs)
    {
        return new AppointmentMedicationDrugsDto()
        {
            Id = appointmentMedicationDrugs.Id,
            Dose =  appointmentMedicationDrugs.Dose,
            IsActive = appointmentMedicationDrugs.IsActive,
            Duration = appointmentMedicationDrugs.Duration,
            Frequency = appointmentMedicationDrugs.Frequency,
            Instructions = appointmentMedicationDrugs.Instructions,
            Medicine = (appointmentMedicationDrugs.Medicine ?? Medicine.Default).ToMedicineDto()
        };
    } 
}