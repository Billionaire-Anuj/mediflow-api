using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.DTOs.Appointments.Medications;

namespace Mediflow.Application.Interfaces.Services;

public interface IAppointmentMedicationsService : ITransientService
{
    List<AppointmentDto> GetAllAppointmentMedications(
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
        DiagnosticStatus[]? statuses = null
    );

    List<AppointmentDto> GetAllAppointmentMedications(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? appointmentId = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? pharmacistId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        DiagnosticStatus[]? statuses = null
    );

    AppointmentMedicationsDto GetAppointmentMedicationsById(Guid appointmentMedicationsId);

    void DispenseAppointmentMedications(Guid appointmentMedicationsId);
}
