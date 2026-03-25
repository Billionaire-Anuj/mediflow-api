using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Appointments.Diagnostics;

namespace Mediflow.Infrastructure.Implementation.Services;

public class AppointmentDiagnosticsService(
    IFileService fileService,
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService,
    INotificationService notificationService) : IAppointmentDiagnosticsService
{
    private const string DiagnosticReportsFilePath = Constants.FilePath.DiagnosticReportsFilePath;

    public List<AppointmentDto> GetAllAppointmentDiagnostics(
        int pageNumber,
        int pageSize,
        out int rowCount,
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? appointmentId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? labTechnicianId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        DiagnosticStatus[]? statuses = null)
    {
        var statusIdentifiers = statuses != null ? new HashSet<DiagnosticStatus>(statuses) : null;

        var appointmentDiagnosticsModel = applicationDbContext.AppointmentDiagnostics
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Notes.ToLower().Contains(globalSearch.Trim().ToLower())
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(globalSearch.Trim().ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (labTechnicianId == null || x.LabTechnicianId == labTechnicianId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)));

        var appointmentIds = appointmentDiagnosticsModel
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

    public List<AppointmentDto> GetAllAppointmentDiagnostics(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? appointmentId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? labTechnicianId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        DiagnosticStatus[]? statuses = null)
    {
        var statusIdentifiers = statuses != null ? new HashSet<DiagnosticStatus>(statuses) : null;

        var appointmentDiagnosticsModel = applicationDbContext.AppointmentDiagnostics
            .Where(x =>
                (string.IsNullOrEmpty(globalSearch)
                    || x.Notes.ToLower().Contains(globalSearch.Trim().ToLower())
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(globalSearch.Trim().ToLower()))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(globalSearch.Trim().ToLower()))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (labTechnicianId == null || x.LabTechnicianId == labTechnicianId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)));

        var appointmentIds = appointmentDiagnosticsModel
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

    public AppointmentDiagnosticsDto GetAppointmentDiagnosticsById(Guid appointmentDiagnosticsId)
    {
        var diagnosticsModel = applicationDbContext.AppointmentDiagnostics
            .Include(x => x.LabTechnician)
                .ThenInclude(x => x!.Role)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Doctor)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Patient)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Timeslot)
            .Include(x => x.DiagnosticTests)
                .ThenInclude(x => x.DiagnosticTest)
                    .ThenInclude(x => x!.DiagnosticType)
            .Include(x => x.DiagnosticTests)
                .ThenInclude(x => x.AppointmentDiagnosticTestResult)
            .FirstOrDefault(x => x.Id == appointmentDiagnosticsId)
            ?? throw new NotFoundException($"Appointment diagnostics with the identifier of {appointmentDiagnosticsId} could not be found.");

        return diagnosticsModel.ToAppointmentDiagnosticsDto();
    }

    public void AssignLabTechnician(Guid appointmentDiagnosticsId)
    {
        var userId = applicationUserService.GetUserId;

        var labTechnician = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"Lab technician with the identifier of {userId} could not be found.");

        if (labTechnician.Role?.Id.ToString() != Constants.Roles.LabTechnician.Id)
            throw new BadRequestException("Only lab technicians can be assigned to diagnostics.");

        var diagnosticsModel = applicationDbContext.AppointmentDiagnostics
            .FirstOrDefault(x => x.Id == appointmentDiagnosticsId)
            ?? throw new NotFoundException($"Appointment diagnostics with the identifier of {appointmentDiagnosticsId} could not be found.");

        if (diagnosticsModel.Status == DiagnosticStatus.Resulted)
            throw new BadRequestException("The diagnostics has already been completed.");

        if (diagnosticsModel.LabTechnicianId != null && diagnosticsModel.LabTechnicianId != userId)
            throw new BadRequestException("This diagnostics is already assigned to another lab technician.");

        diagnosticsModel.AssignLabTechnician(userId);
        diagnosticsModel.UpdateStatus(DiagnosticStatus.Scheduled);

        var patientName = applicationDbContext.Appointments
            .Where(x => x.Id == diagnosticsModel.AppointmentId)
            .Select(x => x.Patient != null ? x.Patient.Name : "the patient")
            .FirstOrDefault() ?? "the patient";

        notificationService.QueueNotification(
            userId,
            NotificationType.Lab,
            "Lab request assigned",
            $"You are now assigned to process diagnostics for {patientName}.",
            $"/lab/request/{diagnosticsModel.Id}",
            $"lab-request-assigned:{diagnosticsModel.Id:N}:{userId:N}");

        applicationDbContext.SaveChanges();
    }

    public void UploadDiagnosticReport(Guid appointmentDiagnosticTestId, UploadAppointmentDiagnosticReportDto report)
    {
        var appointmentDiagnosticTest = GetAppointmentDiagnosticTestForLab(appointmentDiagnosticTestId);

        if (appointmentDiagnosticTest.DiagnosticReport != null)
            throw new BadRequestException("A diagnostic report has already been uploaded for this test.");

        if (appointmentDiagnosticTest.AppointmentDiagnosticTestResult != null)
            throw new BadRequestException("Results have already been submitted for this test.");

        var uploadedReport = fileService.UploadDocument(report.Report, DiagnosticReportsFilePath);

        appointmentDiagnosticTest.AttachDiagnosticReport(uploadedReport.ToAssetModel());

        applicationDbContext.SaveChanges();

        TryMarkDiagnosticsCompleted(appointmentDiagnosticTest.AppointmentDiagnosticsId);
    }

    public void SubmitDiagnosticTestResult(Guid appointmentDiagnosticTestId, UpdateAppointmentDiagnosticTestResultDto result)
    {
        if (appointmentDiagnosticTestId != result.AppointmentDiagnosticTestId)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        var appointmentDiagnosticTest = GetAppointmentDiagnosticTestForLab(appointmentDiagnosticTestId);

        if (appointmentDiagnosticTest.DiagnosticReport != null)
            throw new BadRequestException("A diagnostic report has already been uploaded for this test.");

        if (appointmentDiagnosticTest.AppointmentDiagnosticTestResult == null)
        {
            var resultModel = new AppointmentDiagnosticTestResult(
                appointmentDiagnosticTest.Id,
                result.Value,
                result.Unit,
                result.UpperRange,
                result.LowerRange,
                result.Interpretation
            );

            applicationDbContext.AppointmentDiagnosticTestResults.Add(resultModel);
        }
        else
        {
            throw new BadRequestException("Results have already been submitted for this test.");
        }

        applicationDbContext.SaveChanges();

        TryMarkDiagnosticsCompleted(appointmentDiagnosticTest.AppointmentDiagnosticsId);
    }

    private AppointmentDiagnosticTests GetAppointmentDiagnosticTestForLab(Guid appointmentDiagnosticTestId)
    {
        var userId = applicationUserService.GetUserId;

        var labTechnician = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"Lab technician with the identifier of {userId} could not be found.");

        if (labTechnician.Role?.Id.ToString() != Constants.Roles.LabTechnician.Id)
            throw new BadRequestException("Only lab technicians can update diagnostic tests.");

        var appointmentDiagnosticTest = applicationDbContext.AppointmentDiagnosticTests
            .Include(x => x.AppointmentDiagnosticTestResult)
            .Include(x => x.AppointmentDiagnostics)
            .FirstOrDefault(x => x.Id == appointmentDiagnosticTestId)
            ?? throw new NotFoundException($"Appointment diagnostic test with the identifier of {appointmentDiagnosticTestId} could not be found.");

        if (appointmentDiagnosticTest.AppointmentDiagnostics == null)
            throw new NotFoundException("Appointment diagnostics not found for the diagnostic test.");

        if (appointmentDiagnosticTest.AppointmentDiagnostics.LabTechnicianId != null && appointmentDiagnosticTest.AppointmentDiagnostics.LabTechnicianId != userId)
            throw new BadRequestException("This diagnostic test is assigned to another lab technician.");

        if (appointmentDiagnosticTest.AppointmentDiagnostics.LabTechnicianId == null)
        {
            appointmentDiagnosticTest.AppointmentDiagnostics.AssignLabTechnician(userId);
            appointmentDiagnosticTest.AppointmentDiagnostics.UpdateStatus(DiagnosticStatus.Scheduled);
        }

        return appointmentDiagnosticTest;
    }

    private void TryMarkDiagnosticsCompleted(Guid appointmentDiagnosticsId)
    {
        var diagnosticsModel = applicationDbContext.AppointmentDiagnostics
            .Include(x => x.DiagnosticTests)
                .ThenInclude(x => x.AppointmentDiagnosticTestResult)
            .FirstOrDefault(x => x.Id == appointmentDiagnosticsId);

        if (diagnosticsModel == null) return;

        if (diagnosticsModel.DiagnosticTests.Count == 0) return;

        var completed = diagnosticsModel.DiagnosticTests.All(x => x.AppointmentDiagnosticTestResult != null || x.DiagnosticReport != null);

        if (!completed) return;

        diagnosticsModel.MarkCompleted(DateTime.Now, DiagnosticStatus.Resulted);

        var appointment = applicationDbContext.Appointments
            .Include(x => x.Doctor)
            .Include(x => x.Patient)
            .FirstOrDefault(x => x.Id == diagnosticsModel.AppointmentId);

        if (appointment != null)
        {
            var doctorName = appointment.Doctor?.Name ?? "your doctor";
            var patientName = appointment.Patient?.Name ?? "your patient";

            notificationService.QueueNotification(
                appointment.PatientId,
                NotificationType.Lab,
                "Lab results ready",
                $"Your lab results from the appointment with {doctorName} are now available.",
                $"/patient/appointments/{appointment.Id}",
                $"patient-lab-resulted:{diagnosticsModel.Id:N}");

            notificationService.QueueNotification(
                appointment.DoctorId,
                NotificationType.Lab,
                "Lab results available",
                $"Diagnostic results for {patientName} are ready for review.",
                $"/doctor/appointments/{appointment.Id}",
                $"doctor-lab-resulted:{diagnosticsModel.Id:N}");
        }

        applicationDbContext.SaveChanges();
    }
}
