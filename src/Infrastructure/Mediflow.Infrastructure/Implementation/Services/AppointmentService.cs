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
    IApplicationUserService applicationUserService,
    INotificationService notificationService) : IAppointmentService
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
            .Include(x => x.Review)
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
            .Include(x => x.Review)
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
            .Include(x => x.Review)
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

        if (applicationDbContext.Appointments.Any(x => x.TimeslotId == timeslot.Id && x.Status != AppointmentStatus.Canceled))
            throw new BadRequestException("The selected timeslot is already booked.");

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

        var appointmentStart = timeslot.Date.ToDateTime(timeslot.StartTime).ToString("MMM d, yyyy h:mm tt");
        var doctorName = doctor.Name;

        notificationService.QueueNotification(
            patient.Id,
            NotificationType.Appointment,
            "Appointment booked",
            $"Your appointment with {doctorName} has been booked for {appointmentStart}.",
            PatientAppointmentUrl(appointmentModel.Id),
            $"patient-appointment-booked:{appointmentModel.Id:N}");

        notificationService.QueueNotification(
            doctor.Id,
            NotificationType.Appointment,
            "New appointment booked",
            $"{patient.Name} booked an appointment for {appointmentStart}.",
            DoctorAppointmentUrl(appointmentModel.Id),
            $"doctor-appointment-booked:{appointmentModel.Id:N}");

        applicationDbContext.SaveChanges();
    }

    public void BookAppointmentByDoctor(CreateAppointmentByDoctorDto appointment)
    {
        var doctorId = applicationUserService.GetUserId;

        var doctor = applicationDbContext.Users
            .Include(x => x.Role)
            .Include(x => x.DoctorProfile)
            .FirstOrDefault(x => x.Id == doctorId)
            ?? throw new NotFoundException($"Doctor with the identifier of {doctorId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("Only doctors can book appointments for patients.");

        var patient = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == appointment.PatientId)
            ?? throw new NotFoundException($"Patient with the identifier of {appointment.PatientId} could not be found.");

        if (patient.Role?.Id.ToString() != Constants.Roles.Patient.Id)
            throw new BadRequestException("The selected user is not a patient.");

        var timeslot = applicationDbContext.Timeslots
            .Include(x => x.Schedule)
            .FirstOrDefault(x => x.Id == appointment.TimeslotId)
            ?? throw new NotFoundException($"Timeslot with the identifier of {appointment.TimeslotId} could not be found.");

        if (timeslot.IsBooked)
            throw new BadRequestException("The selected timeslot is already booked.");

        if (timeslot.Schedule == null || timeslot.Schedule.DoctorId != doctor.Id)
            throw new BadRequestException("The selected timeslot does not belong to the doctor.");

        if (applicationDbContext.Appointments.Any(x => x.TimeslotId == timeslot.Id && x.Status != AppointmentStatus.Canceled))
            throw new BadRequestException("The selected timeslot is already booked.");

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

        var appointmentStart = timeslot.Date.ToDateTime(timeslot.StartTime).ToString("MMM d, yyyy h:mm tt");

        notificationService.QueueNotification(
            patient.Id,
            NotificationType.Appointment,
            "Appointment scheduled for you",
            $"{doctor.Name} scheduled your appointment for {appointmentStart}.",
            PatientAppointmentUrl(appointmentModel.Id),
            $"patient-appointment-booked-by-doctor:{appointmentModel.Id:N}");

        notificationService.QueueNotification(
            doctor.Id,
            NotificationType.Appointment,
            "Appointment added to your calendar",
            $"You scheduled an appointment for {patient.Name} on {appointmentStart}.",
            DoctorAppointmentUrl(appointmentModel.Id),
            $"doctor-appointment-booked-by-doctor:{appointmentModel.Id:N}");

        applicationDbContext.SaveChanges();
    }

    public void UpdateAppointment(Guid appointmentId, UpdateAppointmentDto appointment)
    {
        if (appointmentId != appointment.AppointmentId)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var userId = applicationUserService.GetUserId;

        var appointmentModel = applicationDbContext.Appointments
            .Include(x => x.Timeslot)
            .Include(x => x.Doctor)
            .Include(x => x.Patient)
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

            if (applicationDbContext.Appointments.Any(x =>
                    x.TimeslotId == newTimeslot.Id &&
                    x.Id != appointmentModel.Id &&
                    x.Status != AppointmentStatus.Canceled))
                throw new BadRequestException("The selected timeslot is already booked.");

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

        var updatedTimeslot = appointmentModel.Timeslot
            ?? applicationDbContext.Timeslots.First(x => x.Id == appointmentModel.TimeslotId);
        var appointmentStart = updatedTimeslot.Date.ToDateTime(updatedTimeslot.StartTime).ToString("MMM d, yyyy h:mm tt");
        var doctorName = appointmentModel.Doctor?.Name ?? "your doctor";
        var patientName = appointmentModel.Patient?.Name ?? "your patient";

        notificationService.QueueNotification(
            appointmentModel.PatientId,
            NotificationType.Appointment,
            "Appointment updated",
            $"Your appointment with {doctorName} has been updated to {appointmentStart}.",
            PatientAppointmentUrl(appointmentModel.Id),
            $"patient-appointment-updated:{appointmentModel.Id:N}");

        notificationService.QueueNotification(
            appointmentModel.DoctorId,
            NotificationType.Appointment,
            "Appointment rescheduled",
            $"{patientName}'s appointment has been updated to {appointmentStart}.",
            DoctorAppointmentUrl(appointmentModel.Id),
            $"doctor-appointment-updated:{appointmentModel.Id:N}");

        applicationDbContext.SaveChanges();
    }

    public void CancelAppointment(Guid appointmentId, CancelAppointmentDto appointment)
    {
        if (appointmentId != appointment.AppointmentId)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var userId = applicationUserService.GetUserId;

        var appointmentModel = applicationDbContext.Appointments
            .Include(x => x.Timeslot)
            .Include(x => x.Doctor)
            .Include(x => x.Patient)
            .FirstOrDefault(x => x.Id == appointment.AppointmentId)
            ?? throw new NotFoundException($"Appointment with the identifier of {appointmentId} could not be found.");

        if (appointmentModel.Status == AppointmentStatus.Canceled)
            throw new BadRequestException("The appointment has already been cancelled.");

        var isDoctor = appointmentModel.DoctorId == userId;
        var isPatient = appointmentModel.PatientId == userId;

        if (!isDoctor && !isPatient)
            throw new BadRequestException("You can only cancel your own appointments.");

        var refundPercentage = 0m;
        if (appointmentModel.Timeslot != null)
        {
            var bookingDateTime = appointmentModel.Timeslot.Date.ToDateTime(appointmentModel.Timeslot.StartTime);
            var now = DateTime.Now;

            if (now.Date == bookingDateTime.Date)
            {
                refundPercentage = 0m;
            }
            else if (now <= bookingDateTime.AddDays(-2))
            {
                refundPercentage = 100m;
            }
            else if (now < bookingDateTime)
            {
                refundPercentage = 50m;
            }
        }

        var refundAmount = Math.Round(appointmentModel.Fee * (refundPercentage / 100m), 2);

        if (refundAmount > 0 && appointmentModel.IsPaidViaGateway)
        {
            var patient = applicationDbContext.Users
                .Include(x => x.Credit)
                .FirstOrDefault(x => x.Id == appointmentModel.PatientId)
                ?? throw new NotFoundException("Patient not found.");

            var credit = patient.Credit;
            if (credit == null)
            {
                credit = new PatientCredit(patient.Id, 0m, string.Empty);
                applicationDbContext.PatientCredits.Add(credit);
            }

            credit.AddCredits(refundAmount, $"REFUND-APPT-{appointmentModel.Id:N}");
        }

        appointmentModel.Cancel(appointment.CancellationReason, DateTime.Now);

        if (appointmentModel.Timeslot != null)
        {
            appointmentModel.Timeslot.MarkAvailable();
            applicationDbContext.Timeslots.Update(appointmentModel.Timeslot);
        }

        applicationDbContext.SaveChanges();

        var actorLabel = isDoctor ? (appointmentModel.Doctor?.Name ?? "The doctor") : "The patient";
        var refundMessage = refundAmount <= 0
            ? "No refund applies."
            : $"Refunded {refundAmount:0.##} credits to the patient wallet.";

        notificationService.QueueNotification(
            appointmentModel.PatientId,
            NotificationType.Appointment,
            "Appointment cancelled",
            isPatient
                ? $"You cancelled your appointment. {refundMessage}"
                : $"{actorLabel} cancelled your appointment. {refundMessage}",
            PatientAppointmentUrl(appointmentModel.Id),
            $"patient-appointment-cancelled:{appointmentModel.Id:N}");

        notificationService.QueueNotification(
            appointmentModel.DoctorId,
            NotificationType.Appointment,
            "Appointment cancelled",
            isDoctor
                ? $"You cancelled the appointment with {appointmentModel.Patient?.Name ?? "the patient"}."
                : $"{appointmentModel.Patient?.Name ?? "The patient"} cancelled the appointment.",
            DoctorAppointmentUrl(appointmentModel.Id),
            $"doctor-appointment-cancelled:{appointmentModel.Id:N}");

        notificationService.QueueForAdministrativeUsers(
            NotificationType.Admin,
            "Appointment cancelled",
            $"{appointmentModel.Patient?.Name ?? "A patient"}'s appointment was cancelled. {refundMessage}",
            "/admin/dashboard",
            $"admin-appointment-cancelled:{appointmentModel.Id:N}");

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
            .Include(x => x.Patient)
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

        notificationService.QueueNotification(
            appointmentModel.PatientId,
            NotificationType.System,
            "Consultation summary ready",
            $"Your consultation summary with {doctor.Name} is now available.",
            PatientAppointmentUrl(appointmentModel.Id),
            $"patient-consultation-summary:{appointmentModel.Id:N}");

        var diagnosticsIds = applicationDbContext.AppointmentDiagnostics
            .Where(x => x.AppointmentId == appointmentModel.Id)
            .Select(x => x.Id)
            .ToList();

        var labUserIds = applicationDbContext.Users
            .Where(x => x.IsActive && x.Role != null && x.Role.Id.ToString() == Constants.Roles.LabTechnician.Id)
            .Select(x => x.Id)
            .ToList();

        foreach (var diagnosticsId in diagnosticsIds)
        {
            notificationService.QueueNotifications(
                labUserIds,
                NotificationType.Lab,
                "New lab request",
                $"{appointmentModel.Patient?.Name ?? "A patient"} has a new diagnostic request ready for the lab queue.",
                $"/lab/request/{diagnosticsId}",
                $"lab-request-created:{diagnosticsId:N}");
        }

        var medicationIds = applicationDbContext.AppointmentMedications
            .Where(x => x.AppointmentId == appointmentModel.Id)
            .Select(x => x.Id)
            .ToList();

        var pharmacistUserIds = applicationDbContext.Users
            .Where(x => x.IsActive && x.Role != null && x.Role.Id.ToString() == Constants.Roles.Pharmacist.Id)
            .Select(x => x.Id)
            .ToList();

        foreach (var medicationId in medicationIds)
        {
            notificationService.QueueNotifications(
                pharmacistUserIds,
                NotificationType.Prescription,
                "New prescription order",
                $"A new prescription from {doctor.Name} is ready for pharmacy fulfillment.",
                $"/pharmacist/prescription/{medicationId}",
                $"pharmacy-request-created:{medicationId:N}");
        }

        applicationDbContext.SaveChanges();
    }

    public void PayAppointmentWithCredits(Guid appointmentId)
    {
        var userId = applicationUserService.GetUserId;

        var patient = applicationDbContext.Users
            .Include(x => x.Role)
            .Include(x => x.Credit)
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException("Patient not found.");

        if (patient.Role?.Id.ToString() != Constants.Roles.Patient.Id)
        {
            throw new BadRequestException("Only patients can pay with credits.");
        }

        var appointment = applicationDbContext.Appointments
            .Include(x => x.Doctor)
            .FirstOrDefault(x => x.Id == appointmentId)
            ?? throw new NotFoundException($"Appointment with the identifier of {appointmentId} could not be found.");

        if (appointment.PatientId != userId)
        {
            throw new ForbiddenException("You can only pay for your own appointments.");
        }

        if (appointment.Status == AppointmentStatus.Canceled)
        {
            throw new BadRequestException("Canceled appointments cannot be paid.");
        }

        if (appointment.IsPaidViaGateway || appointment.IsPaidViaOfflineMedium)
        {
            throw new BadRequestException("This appointment has already been paid.");
        }

        if (appointment.Fee <= 0)
        {
            appointment.MarkPaidViaGateway();
            applicationDbContext.SaveChanges();
            return;
        }

        var credit = patient.Credit ?? throw new BadRequestException("No credits are available.");

        if (credit.CreditPoints < appointment.Fee)
        {
            throw new BadRequestException("Insufficient credits to complete this payment.");
        }

        credit.DeductCredits(appointment.Fee, $"APPT-{appointment.Id:N}");
        appointment.MarkPaidViaGateway();

        applicationDbContext.SaveChanges();

        notificationService.QueueNotification(
            appointment.PatientId,
            NotificationType.Appointment,
            "Appointment payment completed",
            $"You paid {appointment.Fee:0.##} credits for your appointment.",
            PatientAppointmentUrl(appointment.Id),
            $"patient-appointment-paid:{appointment.Id:N}");

        notificationService.QueueNotification(
            appointment.DoctorId,
            NotificationType.Appointment,
            "Appointment payment received",
            $"{patient.Name} completed payment for their appointment.",
            DoctorAppointmentUrl(appointment.Id),
            $"doctor-appointment-paid:{appointment.Id:N}");

        applicationDbContext.SaveChanges();
    }

    private static string PatientAppointmentUrl(Guid appointmentId) => $"/patient/appointments/{appointmentId}";

    private static string DoctorAppointmentUrl(Guid appointmentId) => $"/doctor/appointments/{appointmentId}";
}
