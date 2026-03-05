using Mediflow.Application.Common.Response;
using Mediflow.Application.DTOs.Users;

namespace Mediflow.Application.DTOs.Reviews;

public class DoctorReviewDto : BaseDto
{
    public Guid AppointmentId { get; set; }

    public Guid DoctorId { get; set; }

    public Guid PatientId { get; set; }

    public int Rating { get; set; }

    public string? Review { get; set; }

    public DateTime CreatedAt { get; set; }

    public UserDto? Patient { get; set; }
}
