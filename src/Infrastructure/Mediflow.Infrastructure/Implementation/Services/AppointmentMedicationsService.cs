using Mediflow.Domain.Common;
using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Appointments.Medications;

namespace Mediflow.Infrastructure.Implementation.Services;

public class AppointmentMedicationsService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService,
    INotificationService notificationService) : IAppointmentMedicationsService
{
    public List<AppointmentDto> GetAllAppointmentMedications(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? appointmentId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? pharmacistId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        DiagnosticStatus[]? statuses = null)
    {
        var statusIdentifiers = statuses != null ? new HashSet<DiagnosticStatus>(statuses) : null;

        var appointmentMedicationsModel = applicationDbContext.AppointmentMedications
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Notes.ToLower().Contains(globalSearch.Trim().ToLower())
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(globalSearch.Trim().ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (pharmacistId == null || x.PharmacistId == pharmacistId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)));

        var appointmentIds = appointmentMedicationsModel
            .Select(x => x.AppointmentId)
            .Distinct();

        var appointmentModels = applicationDbContext.Appointments
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.Role)
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.DoctorProfile)
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.Schedules)
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.DoctorSpecializations)
                .ThenInclude(x => x.Specialization)
            .Include(x => x.Patient)
                .ThenInclude(x => x!.Role)
            .Include(x => x.Patient)
                .ThenInclude(x => x!.Credit)
            .Include(x => x.Timeslot)
            .Include(x => x.MedicalRecord)
            .Include(x => x.AppointmentDiagnostics)
                .ThenInclude(x => x.LabTechnician)
                    .ThenInclude(x => x!.Role)
            .Include(x => x.AppointmentDiagnostics)
                .ThenInclude(x => x.DiagnosticTests)
                    .ThenInclude(x => x.DiagnosticTest)
                        .ThenInclude(x => x!.DiagnosticType)
            .Include(x => x.AppointmentDiagnostics)
                .ThenInclude(x => x.DiagnosticTests)
                    .ThenInclude(x => x.AppointmentDiagnosticTestResult)
            .Include(x => x.AppointmentMedications)
                .ThenInclude(x => x.Pharmacist)
                    .ThenInclude(x => x!.Role)
            .Include(x => x.AppointmentMedications)
                .ThenInclude(x => x.Drugs)
                    .ThenInclude(x => x.Medicine)
                        .ThenInclude(x => x!.MedicationType)
            .AsNoTracking()
            .Where(x => appointmentIds.Contains(x.Id));

        rowCount = appointmentModels.Count();

        var appointments = appointmentModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return appointments.Select(x => x.ToAppointmentDto()).ToList();
    }

    public List<AppointmentDto> GetAllAppointmentMedications(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? appointmentId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? pharmacistId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        DiagnosticStatus[]? statuses = null)
    {
                var statusIdentifiers = statuses != null ? new HashSet<DiagnosticStatus>(statuses) : null;

        var appointmentMedicationsModel = applicationDbContext.AppointmentMedications
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Notes.ToLower().Contains(globalSearch.Trim().ToLower())
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(globalSearch.Trim().ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (pharmacistId == null || x.PharmacistId == pharmacistId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)));

        var appointmentIds = appointmentMedicationsModel
            .Select(x => x.AppointmentId)
            .Distinct();

        var appointmentModels = applicationDbContext.Appointments
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.Role)
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.DoctorProfile)
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.Schedules)
            .Include(x => x.Doctor)
                .ThenInclude(x => x!.DoctorSpecializations)
                .ThenInclude(x => x.Specialization)
            .Include(x => x.Patient)
                .ThenInclude(x => x!.Role)
            .Include(x => x.Patient)
                .ThenInclude(x => x!.Credit)
            .Include(x => x.Timeslot)
            .Include(x => x.MedicalRecord)
            .Include(x => x.AppointmentDiagnostics)
                .ThenInclude(x => x.LabTechnician)
                    .ThenInclude(x => x!.Role)
            .Include(x => x.AppointmentDiagnostics)
                .ThenInclude(x => x.DiagnosticTests)
                    .ThenInclude(x => x.DiagnosticTest)
                        .ThenInclude(x => x!.DiagnosticType)
            .Include(x => x.AppointmentDiagnostics)
                .ThenInclude(x => x.DiagnosticTests)
                    .ThenInclude(x => x.AppointmentDiagnosticTestResult)
            .Include(x => x.AppointmentMedications)
                .ThenInclude(x => x.Pharmacist)
                    .ThenInclude(x => x!.Role)
            .Include(x => x.AppointmentMedications)
                .ThenInclude(x => x.Drugs)
                    .ThenInclude(x => x.Medicine)
                        .ThenInclude(x => x!.MedicationType)
            .AsNoTracking()
            .Where(x => appointmentIds.Contains(x.Id));

        return appointmentModels.Select(x => x.ToAppointmentDto()).ToList();
    }

    public AppointmentMedicationsDto GetAppointmentMedicationsById(Guid appointmentMedicationsId)
    {
        var medicationsModel = applicationDbContext.AppointmentMedications
            .Include(x => x.Pharmacist)
                .ThenInclude(x => x!.Role)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Doctor)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Patient)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Timeslot)
            .Include(x => x.Drugs)
                .ThenInclude(x => x.Medicine)
                    .ThenInclude(x => x!.MedicationType)
            .FirstOrDefault(x => x.Id == appointmentMedicationsId)
            ?? throw new NotFoundException($"Appointment medications with the identifier of {appointmentMedicationsId} could not be found.");

        return medicationsModel.ToAppointmentMedicationsDto();
    }

    public void DispenseAppointmentMedications(Guid appointmentMedicationsId)
    {
        var userId = applicationUserService.GetUserId;

        var pharmacist = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"Pharmacist with the identifier of {userId} could not be found.");

        if (pharmacist.Role?.Id.ToString() != Constants.Roles.Pharmacist.Id)
            throw new BadRequestException("Only pharmacists can dispense medications.");

        var medicationsModel = applicationDbContext.AppointmentMedications
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Doctor)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Patient)
            .FirstOrDefault(x => x.Id == appointmentMedicationsId)
            ?? throw new NotFoundException($"Appointment medications with the identifier of {appointmentMedicationsId} could not be found.");

        if (medicationsModel.Status == DiagnosticStatus.Collected)
            throw new BadRequestException("The medications have already been dispensed.");

        if (medicationsModel.PharmacistId != null && medicationsModel.PharmacistId != userId)
            throw new BadRequestException("This medications record is assigned to another pharmacist.");

        medicationsModel.MarkCollected(userId, DateTime.Now);

        applicationDbContext.SaveChanges();

        if (medicationsModel.Appointment != null)
        {
            var doctorName = medicationsModel.Appointment.Doctor?.Name ?? "your doctor";
            var patientName = medicationsModel.Appointment.Patient?.Name ?? "your patient";

            notificationService.QueueNotification(
                medicationsModel.Appointment.PatientId,
                NotificationType.Prescription,
                "Prescription dispensed",
                $"Your prescription from {doctorName} has been marked as dispensed.",
                $"/patient/appointments/{medicationsModel.AppointmentId}",
                $"patient-prescription-dispensed:{medicationsModel.Id:N}");

            notificationService.QueueNotification(
                medicationsModel.Appointment.DoctorId,
                NotificationType.Prescription,
                "Prescription dispensed",
                $"Medication prescribed for {patientName} has been dispensed by the pharmacy.",
                $"/doctor/appointments/{medicationsModel.AppointmentId}",
                $"doctor-prescription-dispensed:{medicationsModel.Id:N}");
        }

        notificationService.QueueNotification(
            userId,
            NotificationType.Prescription,
            "Prescription completed",
            "You marked this prescription as dispensed.",
            $"/pharmacist/prescription/{medicationsModel.Id}",
            $"pharmacist-prescription-dispensed:{medicationsModel.Id:N}:{userId:N}");

        applicationDbContext.SaveChanges();
    }
}
