namespace Mediflow.Application.DTOs.Appointments;

public class CancelAppointmentDto
{
    public Guid AppointmentId { get; set; }

    public string CancellationReason { get; set; } = string.Empty;
}