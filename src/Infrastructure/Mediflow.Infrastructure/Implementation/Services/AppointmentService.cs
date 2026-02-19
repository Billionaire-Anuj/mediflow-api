using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class AppointmentService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService) : IAppointmentService
{
    public List<AppointmentDto> GetAllAppointments(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        AppointmentStatus[]? statuses = null)
    {
        var statusIdentifiers = statuses != null ? new HashSet<AppointmentStatus>(statuses) : null;

        var appointmentModels = applicationDbContext.Appointments
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || (x.Doctor != null && x.Doctor.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Patient != null && x.Patient.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Notes != null && x.Notes.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Symptoms != null && x.Symptoms.ToLower().Contains(globalSearch.Trim().ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (doctorId == null || x.DoctorId == doctorId) &&
                (patientId == null || x.PatientId == patientId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Timeslot != null && x.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Timeslot != null && x.Timeslot.Date <= endDate.Value)))
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
            .OrderBy(x => orderBys);

        rowCount = appointmentModels.Count();

        return appointmentModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToAppointmentDto())
            .ToList();
    }

    public List<AppointmentDto> GetAllAppointments(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        AppointmentStatus[]? statuses = null)
    {
        var statusIdentifiers = statuses != null ? new HashSet<AppointmentStatus>(statuses) : null;
        
        var appointmentModels = applicationDbContext.Appointments
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || (x.Doctor != null && x.Doctor.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Patient != null && x.Patient.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Notes != null && x.Notes.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Symptoms != null && x.Symptoms.ToLower().Contains(globalSearch.Trim().ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (doctorId == null || x.DoctorId == doctorId) &&
                (patientId == null || x.PatientId == patientId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Timeslot != null && x.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Timeslot != null && x.Timeslot.Date <= endDate.Value)))
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
            .OrderBy(x => orderBys);

        return appointmentModels.Select(x => x.ToAppointmentDto()).ToList();
    }

    public AppointmentDto GetAppointmentById(Guid appointmentId)
    {
        var appointmentModel = applicationDbContext.Appointments
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
            .FirstOrDefault(x => x.Id == appointmentId)
            ?? throw new NotFoundException($"Appointment with the identifier of {appointmentId} could not be found.");

        return appointmentModel.ToAppointmentDto();
    }

    public void BookAppointment(CreateAppointmentDto appointment)
    {
        var userId = applicationUserService.GetUserId;

        var patient = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"User with the identifier of {userId} could not be found.");

        if (patient.Role?.Id.ToString() != Constants.Roles.Patient.Id)
            throw new BadRequestException("Only patients can book appointments.");

        var doctor = applicationDbContext.Users
            .Include(x => x.Role)
            .Include(x => x.DoctorProfile)
            .FirstOrDefault(x => x.Id == appointment.DoctorId)
            ?? throw new NotFoundException($"Doctor with the identifier of {appointment.DoctorId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("The selected user is not a doctor.");

        var timeslot = applicationDbContext.Timeslots
            .Include(x => x.Schedule)
            .FirstOrDefault(x => x.Id == appointment.TimeslotId)
            ?? throw new NotFoundException($"Timeslot with the identifier of {appointment.TimeslotId} could not be found.");

        if (timeslot.IsBooked)
            throw new BadRequestException("The selected timeslot is already booked.");

        if (timeslot.Schedule == null || timeslot.Schedule.DoctorId != doctor.Id)
            throw new BadRequestException("The selected timeslot does not belong to the selected doctor.");

        var fee = doctor.DoctorProfile?.ConsultationFee ?? 0m;

        var appointmentModel = new Appointment(
            doctor.Id,
            patient.Id,
            DateTime.Now,
            timeslot.Id,
            null,
            AppointmentStatus.Scheduled,
            appointment.Notes,
            appointment.Symptoms,
            fee);

        applicationDbContext.Appointments.Add(appointmentModel);

        timeslot.MarkBooked();

        applicationDbContext.Timeslots.Update(timeslot);

        applicationDbContext.SaveChanges();
    }

    public void UpdateAppointment(Guid appointmentId, UpdateAppointmentDto appointment)
    {
        if (appointmentId != appointment.AppointmentId)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var userId = applicationUserService.GetUserId;

        var appointmentModel = applicationDbContext.Appointments
            .Include(x => x.Timeslot)
            .FirstOrDefault(x => x.Id == appointment.AppointmentId)
            ?? throw new NotFoundException($"Appointment with the identifier of {appointmentId} could not be found.");

        if (appointmentModel.Status != AppointmentStatus.Scheduled)
            throw new BadRequestException("Only scheduled appointments can be updated.");

        if (appointmentModel.PatientId != userId)
            throw new BadRequestException("You can only update your own appointments.");

        if (appointmentModel.TimeslotId != appointment.TimeslotId)
        {
            var newTimeslot = applicationDbContext.Timeslots
                .Include(x => x.Schedule)
                .FirstOrDefault(x => x.Id == appointment.TimeslotId)
                ?? throw new NotFoundException($"Timeslot with the identifier of {appointment.TimeslotId} could not be found.");

            if (newTimeslot.IsBooked)
                throw new BadRequestException("The selected timeslot is already booked.");

            if (newTimeslot.Schedule == null || newTimeslot.Schedule.DoctorId != appointmentModel.DoctorId)
                throw new BadRequestException("The selected timeslot does not belong to the doctor.");

            if (appointmentModel.Timeslot != null)
            {
                appointmentModel.Timeslot.MarkAvailable();
                applicationDbContext.Timeslots.Update(appointmentModel.Timeslot);
            }

            newTimeslot.MarkBooked();
            applicationDbContext.Timeslots.Update(newTimeslot);

            appointmentModel.UpdateDetails(newTimeslot.Id, appointment.Notes, appointment.Symptoms);
            appointmentModel.Timeslot = newTimeslot;
        }
        else
        {
            appointmentModel.UpdateDetails(appointmentModel.TimeslotId, appointment.Notes, appointment.Symptoms);
        }

        applicationDbContext.SaveChanges();
    }

    public void CancelAppointment(Guid appointmentId, CancelAppointmentDto appointment)
    {
        if (appointmentId != appointment.AppointmentId)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var userId = applicationUserService.GetUserId;

        var appointmentModel = applicationDbContext.Appointments
            .Include(x => x.Timeslot)
            .FirstOrDefault(x => x.Id == appointment.AppointmentId)
            ?? throw new NotFoundException($"Appointment with the identifier of {appointmentId} could not be found.");

        if (appointmentModel.Status == AppointmentStatus.Canceled)
            throw new BadRequestException("The appointment has already been cancelled.");

        var isDoctor = appointmentModel.DoctorId == userId;
        var isPatient = appointmentModel.PatientId == userId;

        if (!isDoctor && !isPatient)
            throw new BadRequestException("You can only cancel your own appointments.");

        appointmentModel.Cancel(appointment.CancellationReason, DateTime.Now);

        if (appointmentModel.Timeslot != null)
        {
            appointmentModel.Timeslot.MarkAvailable();
            applicationDbContext.Timeslots.Update(appointmentModel.Timeslot);
        }

        applicationDbContext.SaveChanges();
    }

    public void ConsultAppointment(Guid appointmentId, ConsultAppointmentDto appointment)
    {
        if (appointmentId != appointment.AppointmentId)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var userId = applicationUserService.GetUserId;

        var doctor = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"Doctor with the identifier of {userId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("Only doctors can consult appointments.");

        var appointmentModel = applicationDbContext.Appointments
            .Include(x => x.MedicalRecord)
            .Include(x => x.AppointmentDiagnostics)
            .Include(x => x.AppointmentMedications)
            .FirstOrDefault(x => x.Id == appointment.AppointmentId)
            ?? throw new NotFoundException($"Appointment with the identifier of {appointmentId} could not be found.");

        if (appointmentModel.DoctorId != doctor.Id)
            throw new BadRequestException("You can only consult your own appointments.");

        if (appointmentModel.Status != AppointmentStatus.Scheduled)
            throw new BadRequestException("Only scheduled appointments can be consulted.");

        if (appointmentModel.MedicalRecord != null || appointmentModel.AppointmentDiagnostics.Count != 0 || appointmentModel.AppointmentMedications.Count != 0)
            throw new BadRequestException("This appointment has already been consulted.");

        if (appointmentModel.MedicalRecord == null)
        {
            var medicalRecord = new MedicalRecord(
                appointmentModel.Id,
                appointment.Diagnosis,
                appointment.Treatment,
                appointment.Prescriptions,
                appointment.Notes
            );

            applicationDbContext.MedicalRecords.Add(medicalRecord);
        }
        else
        {
            appointmentModel.MedicalRecord.Update(
                appointment.Diagnosis,
                appointment.Treatment,
                appointment.Prescriptions,
                appointment.Notes
            );
        }

        if (appointment.Diagnostics.Count != 0)
        {
            foreach (var diagnostic in appointment.Diagnostics)
            {
                var diagnosticsModel = new AppointmentDiagnostics(
                    appointmentModel.Id,
                    null,
                    diagnostic.Notes
                );

                applicationDbContext.AppointmentDiagnostics.Add(diagnosticsModel);

                foreach (var diagnosticTest in diagnostic.DiagnosticTests)
                {
                    var diagnosticTestModel = applicationDbContext.DiagnosticTests
                        .FirstOrDefault(x => x.Id == diagnosticTest.DiagnosticTestId)
                        ?? throw new NotFoundException($"Diagnostic test with the identifier of {diagnosticTest.DiagnosticTestId} could not be found.");

                    var appointmentDiagnosticTestModel = new AppointmentDiagnosticTests(
                        diagnosticsModel.Id,
                        diagnosticTestModel.Id
                    );

                    applicationDbContext.AppointmentDiagnosticTests.Add(appointmentDiagnosticTestModel);
                }
            }
        }

        if (appointment.Medications.Count != 0)
        {
            foreach (var medication in appointment.Medications)
            {
                var medicationsModel = new AppointmentMedications(
                    appointmentModel.Id,
                    null,
                    medication.Notes
                );

                applicationDbContext.AppointmentMedications.Add(medicationsModel);

                foreach (var drug in medication.Drugs)
                {
                    var medicineModel = applicationDbContext.Medicines
                        .FirstOrDefault(x => x.Id == drug.MedicineId)
                        ?? throw new NotFoundException($"Medicine with the identifier of {drug.MedicineId} could not be found.");

                    var appointmentMedicationDrugsModel = new AppointmentMedicationDrugs(
                        medicationsModel.Id,
                        medicineModel.Id,
                        drug.Dose,
                        drug.Frequency,
                        drug.Duration,
                        drug.Instructions
                    );

                    applicationDbContext.AppointmentMedicationDrugs.Add(appointmentMedicationDrugsModel);
                }
            }
        }

        appointmentModel.MarkCompleted();

        applicationDbContext.SaveChanges();
    }
}
