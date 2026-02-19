namespace Mediflow.Application.DTOs.Appointments;

public abstract class AbstractAppointmentDto
{
    public Guid TimeslotId { get; set; }

    public string Notes { get; set; } = string.Empty;

    public string Symptoms { get; set; } = string.Empty;
}