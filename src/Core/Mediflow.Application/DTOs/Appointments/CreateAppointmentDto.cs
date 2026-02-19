namespace Mediflow.Application.DTOs.Appointments;

public class CreateAppointmentDto : AbstractAppointmentDto
{
    public Guid DoctorId { get; set; }
}