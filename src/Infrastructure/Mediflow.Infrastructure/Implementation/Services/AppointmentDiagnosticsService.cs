using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.Appointments.Diagnostics;

namespace Mediflow.Infrastructure.Implementation.Services;

public class AppointmentDiagnosticsService(
    IFileService fileService,
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService) : IAppointmentDiagnosticsService
{
    private const string DiagnosticReportsFilePath = Constants.FilePath.DiagnosticReportsFilePath;

    public List<AppointmentDiagnosticsDto> GetAllAppointmentDiagnostics(
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
        var normalizedGlobalSearch = globalSearch?.Trim().ToLower();

        var diagnosticsModels = applicationDbContext.AppointmentDiagnostics
            .Where(x =>
                (string.IsNullOrEmpty(normalizedGlobalSearch)
                    || x.Notes.ToLower().Contains(normalizedGlobalSearch)
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(normalizedGlobalSearch))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(normalizedGlobalSearch))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (labTechnicianId == null || x.LabTechnicianId == labTechnicianId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)))
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
            .OrderBy(x => orderBys);

        rowCount = diagnosticsModels.Count();

        return diagnosticsModels
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToAppointmentDiagnosticsDto())
            .ToList();
    }

    public List<AppointmentDiagnosticsDto> GetAllAppointmentDiagnostics(
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
        var normalizedGlobalSearch = globalSearch?.Trim().ToLower();

        var diagnosticsModels = applicationDbContext.AppointmentDiagnostics
            .Where(x =>
                (string.IsNullOrEmpty(normalizedGlobalSearch)
                    || x.Notes.ToLower().Contains(normalizedGlobalSearch)
                    || (x.Appointment != null && x.Appointment.Doctor != null && x.Appointment.Doctor.Name.ToLower().Contains(normalizedGlobalSearch))
                    || (x.Appointment != null && x.Appointment.Patient != null && x.Appointment.Patient.Name.ToLower().Contains(normalizedGlobalSearch))) &&
                (isActive == null || isActive.Contains(x.IsActive)) &&
                (appointmentId == null || x.AppointmentId == appointmentId) &&
                (doctorId == null || (x.Appointment != null && x.Appointment.DoctorId == doctorId)) &&
                (patientId == null || (x.Appointment != null && x.Appointment.PatientId == patientId)) &&
                (labTechnicianId == null || x.LabTechnicianId == labTechnicianId) &&
                (statusIdentifiers == null || statusIdentifiers.Contains(x.Status)) &&
                (!startDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date >= startDate.Value)) &&
                (!endDate.HasValue || (x.Appointment != null && x.Appointment.Timeslot != null && x.Appointment.Timeslot.Date <= endDate.Value)))
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
            .OrderBy(x => orderBys);

        return diagnosticsModels.Select(x => x.ToAppointmentDiagnosticsDto()).ToList();
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

        applicationDbContext.SaveChanges();
    }

    public void UploadDiagnosticReport(Guid appointmentDiagnosticTestId, UploadAppointmentDiagnosticReportDto report)
    {
        var appointmentDiagnosticTest = GetAppointmentDiagnosticTestForLab(appointmentDiagnosticTestId);

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
            appointmentDiagnosticTest.AppointmentDiagnosticTestResult.Update(
                result.Value,
                result.Unit,
                result.UpperRange,
                result.LowerRange,
                result.Interpretation
            );
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

        applicationDbContext.SaveChanges();
    }
}
