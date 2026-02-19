using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.DTOs.Assets;
using Mediflow.Application.DTOs.DiagnosticTests;

namespace Mediflow.Application.DTOs.Appointments.Diagnostics;

public static class AppointmentDiagnosticsExtensionMethods
{
    public static AppointmentDiagnosticsDto ToAppointmentDiagnosticsDto(this AppointmentDiagnostics appointmentDiagnostics)
    {
        return new AppointmentDiagnosticsDto()
        {
            Id = appointmentDiagnostics.Id,
            Notes = appointmentDiagnostics.Notes,
            Status = appointmentDiagnostics.Status,
            IsActive = appointmentDiagnostics.IsActive,
            CompletedDate = appointmentDiagnostics.CompletedDate,
            LabTechnician = appointmentDiagnostics.LabTechnician?.ToUserDto(),
            DiagnosticTests = appointmentDiagnostics.DiagnosticTests.Select(x => x.ToAppointmentDiagnosticTestsDto()).ToList()
        };
    }

    private static AppointmentDiagnosticTestsDto ToAppointmentDiagnosticTestsDto(this AppointmentDiagnosticTests appointmentDiagnosticTests)
    {
        return new AppointmentDiagnosticTestsDto()
        {
            Id = appointmentDiagnosticTests.Id,
            IsActive = appointmentDiagnosticTests.IsActive,
            Report = appointmentDiagnosticTests.DiagnosticReport?.ToAssetDto(),
            Result = appointmentDiagnosticTests.AppointmentDiagnosticTestResult?.ToAppointmentDiagnosticTestResultDto(),
            DiagnosticTest = (appointmentDiagnosticTests.DiagnosticTest ?? DiagnosticTest.Default).ToDiagnosticTestDto()
        };
    }

    private static AppointmentDiagnosticTestResultDto ToAppointmentDiagnosticTestResultDto(this AppointmentDiagnosticTestResult appointmentDiagnosticTestResult)
    {
        return new AppointmentDiagnosticTestResultDto()
        {
            Id = appointmentDiagnosticTestResult.Id,
            Unit = appointmentDiagnosticTestResult.Unit,
            Value = appointmentDiagnosticTestResult.Value,
            IsActive = appointmentDiagnosticTestResult.IsActive,
            UpperRange = appointmentDiagnosticTestResult.UpperRange,
            LowerRange = appointmentDiagnosticTestResult.LowerRange,
            Interpretation = appointmentDiagnosticTestResult.Interpretation
        };
    }
}