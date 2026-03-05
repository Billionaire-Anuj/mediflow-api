namespace Mediflow.Application.DTOs.Appointments;

public class CreateAppointmentByDoctorDto : AbstractAppointmentDto
{
    public Guid PatientId { get; set; }
}
