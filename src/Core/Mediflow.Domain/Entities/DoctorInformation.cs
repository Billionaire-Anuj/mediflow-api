using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class DoctorInformation(
    Guid doctorId,
    string about,
    string licenseNumber,
    string educationInformation,
    string experienceInformation,
    decimal consultationFee) : AuditableEntity<Guid>
{
    [ForeignKey(nameof(Doctor))]
    public Guid DoctorId { get; private set; } = doctorId;

    public string About { get; private set; } = about;

    public string LicenseNumber { get; private set; } = licenseNumber;

    public string EducationInformation { get; private set; } = educationInformation;

    public string ExperienceInformation { get; private set; } = experienceInformation;

    public decimal ConsultationFee { get; private set; } = consultationFee;

    public virtual User Doctor { get; set; } = User.Default;
}