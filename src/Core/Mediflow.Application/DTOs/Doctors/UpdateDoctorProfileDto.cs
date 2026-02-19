namespace Mediflow.Application.DTOs.Doctors;

public class UpdateDoctorProfileDto
{
    public string About { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public string EducationInformation { get; set; } = string.Empty;

    public string ExperienceInformation { get; set; } = string.Empty;

    public decimal ConsultationFee { get; set; }

    public List<Guid> SpecializationIds { get; set; } = new();
}