using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.DTOs.Patients;
using Mediflow.Application.DTOs.Appointments.Diagnostics;
using Mediflow.Application.DTOs.Appointments.Medications;
using Mediflow.Application.DTOs.Appointments.MedicalRecords;
using Mediflow.Application.DTOs.Doctors.Schedules.Timeslots;

namespace Mediflow.Application.DTOs.Appointments;

public static class AppointmentExtensionMethods
{
    public static AppointmentDto ToAppointmentDto(this Appointment appointment)
    {
        return new AppointmentDto()
        {
            Id = appointment.Id,
            Fee = appointment.Fee,
            Notes = appointment.Notes,
            Status = appointment.Status,
            Symptoms = appointment.Symptoms,
            IsActive = appointment.IsActive,
            BookedDate = appointment.BookedDate,
            CancelledDate = appointment.CancelledDate,
            IsPaidViaGateway = appointment.IsPaidViaGateway,
            CancellationReason = appointment.CancellationReason,
            IsPaidViaOfflineMedium = appointment.IsPaidViaOfflineMedium,
            HasReview = appointment.Review != null,
            ReviewRating = appointment.Review?.Rating,
            Doctor = (appointment.Doctor ?? User.Default).ToDoctorProfileDto(),
            Timeslot = (appointment.Timeslot ?? Timeslot.Default).ToTimeslotDto(),
            Patient = (appointment.Patient ?? User.Default).ToPatientProfileDto(),
            MedicalRecords = (appointment.MedicalRecord ?? MedicalRecord.Default).ToMedicalRecordDto(),
            Medications = appointment.AppointmentMedications.Select(x => x.ToAppointmentMedicationsDto()).ToList(),
            Diagnostics = appointment.AppointmentDiagnostics.Select(x => x.ToAppointmentDiagnosticsDto()).ToList(),
        };
    }
}
