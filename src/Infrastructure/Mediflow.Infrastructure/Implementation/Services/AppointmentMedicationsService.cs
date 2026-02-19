using Mediflow.Domain.Common;
using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Appointments.Medications;

namespace Mediflow.Infrastructure.Implementation.Services;

public class AppointmentMedicationsService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService) : IAppointmentMedicationsService
{
    public List<AppointmentMedicationsDto> GetAllAppointmentMedications(
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
        var normalizedGlobalSearch = globalSearch?.Trim().ToLower();

        var medicationsModels = applicationDbContext.AppointmentMedications
            .Where(x =>
                (string.IsNullOrEmpty(normalizedGlobalSearch)
                    || x.Notes.ToLower().Contains(normalizedGlobalSearch)
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(normalizedGlobalSearch))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(normalizedGlobalSearch))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (pharmacistId == null || x.PharmacistId == pharmacistId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)))
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
            .OrderBy(x => orderBys);

        rowCount = medicationsModels.Count();

        return medicationsModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToAppointmentMedicationsDto())
            .ToList();
    }

    public List<AppointmentMedicationsDto> GetAllAppointmentMedications(
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
        var normalizedGlobalSearch = globalSearch?.Trim().ToLower();

        var medicationsModels = applicationDbContext.AppointmentMedications
            .Where(x =>
                (string.IsNullOrEmpty(normalizedGlobalSearch)
                    || x.Notes.ToLower().Contains(normalizedGlobalSearch)
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(normalizedGlobalSearch))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(normalizedGlobalSearch))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (pharmacistId == null || x.PharmacistId == pharmacistId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)))
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
            .OrderBy(x => orderBys);

        return medicationsModels.Select(x => x.ToAppointmentMedicationsDto()).ToList();
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
            .FirstOrDefault(x => x.Id == appointmentMedicationsId)
            ?? throw new NotFoundException($"Appointment medications with the identifier of {appointmentMedicationsId} could not be found.");

        if (medicationsModel.Status == DiagnosticStatus.Collected)
            throw new BadRequestException("The medications have already been dispensed.");

        if (medicationsModel.PharmacistId != null && medicationsModel.PharmacistId != userId)
            throw new BadRequestException("This medications record is assigned to another pharmacist.");

        medicationsModel.MarkCollected(userId, DateTime.Now);

        applicationDbContext.SaveChanges();
    }
}
