namespace Mediflow.Application.DTOs.Reviews;

public class CreateDoctorReviewDto
{
    public Guid AppointmentId { get; set; }

    public int Rating { get; set; }

    public string? Review { get; set; }
}
