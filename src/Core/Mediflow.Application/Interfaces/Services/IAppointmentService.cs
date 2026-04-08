using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Appointments;

namespace Mediflow.Application.Interfaces.Services;

public interface IAppointmentService : ITransientService
{
    List<AppointmentDto> GetAllAppointments(
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
        AppointmentStatus[]? statuses = null
    );

    List<AppointmentDto> GetAllAppointments(
        string? globalSearch = null,
        bool[]? isActive = null,
        string[]? orderBys = null,
        Guid? doctorId = null,
        Guid? patientId = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null,
        AppointmentStatus[]? statuses = null
    );

    AppointmentDto GetAppointmentById(Guid appointmentId);

    void BookAppointment(CreateAppointmentDto appointment);

    void BookAppointmentByDoctor(CreateAppointmentByDoctorDto appointment);

    void UpdateAppointment(Guid appointmentId, UpdateAppointmentDto appointment);

    void RescheduleAppointmentByDoctor(Guid appointmentId, UpdateAppointmentDto appointment);

    void CancelAppointment(Guid appointmentId, CancelAppointmentDto appointment);

    void ConsultAppointment(Guid appointmentId, ConsultAppointmentDto appointment);

    void PayAppointmentWithCredits(Guid appointmentId);
}
