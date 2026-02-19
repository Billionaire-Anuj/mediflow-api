using Mediflow.Domain.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class DoctorProfile(
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

    public virtual User? Doctor { get; set; }

    public static DoctorProfile Default => new(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0m);

    public void Update(string about, string licenseNumber, string educationInformation, string experienceInformation, decimal consultationFee)
    {
        if (About != about) About = about;
        if (LicenseNumber != licenseNumber) LicenseNumber = licenseNumber;
        if (ConsultationFee != consultationFee) ConsultationFee = consultationFee;
        if (EducationInformation != educationInformation) EducationInformation = educationInformation;
        if (ExperienceInformation != experienceInformation) ExperienceInformation = experienceInformation;
    }
}