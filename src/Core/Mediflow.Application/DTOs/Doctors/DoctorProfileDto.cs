using Mediflow.Application.DTOs.Users;
using Mediflow.Application.DTOs.Specializations;
using Mediflow.Application.DTOs.Doctors.Schedules;

namespace Mediflow.Application.DTOs.Doctors;

public class DoctorProfileDto : UserDto
{
    public string About { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public string EducationInformation { get; set; } = string.Empty;

    public string ExperienceInformation { get; set; } = string.Empty;

    public decimal ConsultationFee { get; set; }

    public decimal AverageRating { get; set; }

    public int ReviewCount { get; set; }

    public List<ScheduleDto> Schedules { get; set; } = new();

    public List<SpecializationDto> Specializations { get; set; } = new();
}
