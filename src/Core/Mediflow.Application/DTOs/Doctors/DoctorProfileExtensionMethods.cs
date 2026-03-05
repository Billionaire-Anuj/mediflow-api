using Mediflow.Application.DTOs.Assets;
using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Specializations;
using Mediflow.Application.DTOs.Doctors.Schedules;
using Mediflow.Application.DTOs.Roles;

namespace Mediflow.Application.DTOs.Doctors;

public static class DoctorProfileExtensionMethods
{
    public static DoctorProfileDto ToDoctorProfileDto(this User doctor)
    {
        var doctorProfile = doctor.DoctorProfile ?? DoctorProfile.Default;
        var reviewCount = doctor.DoctorReviews.Count;
        var averageRating = reviewCount == 0
            ? 0m
            : Math.Round(doctor.DoctorReviews.Sum(x => x.Rating) / (decimal)reviewCount, 1);

        return new DoctorProfileDto
        {
            Id = doctor.Id,
            Name = doctor.Name,
            Gender = doctor.Gender,
            Address = doctor.Address,
            IsActive = doctor.IsActive,
            Username = doctor.Username,
            About = doctorProfile.About,
            PhoneNumber = doctor.PhoneNumber,
            EmailAddress = doctor.EmailAddress,
            LicenseNumber = doctorProfile.LicenseNumber,
            ConsultationFee = doctorProfile.ConsultationFee,
            AverageRating = averageRating,
            ReviewCount = reviewCount,
            Role = (doctor.Role ?? Role.Default).ToRoleDto(),
            ProfileImage = doctor.ProfileImage?.ToAssetDto(),
            EducationInformation = doctorProfile.EducationInformation,
            ExperienceInformation = doctorProfile.ExperienceInformation,
            Is2FactorAuthenticationEnabled = doctor.Is2FactorAuthenticationEnabled,
            Schedules = doctor.Schedules.Select(x => x.ToScheduleDto()).ToList(),
            Specializations = doctor.DoctorSpecializations.Select(x => (x.Specialization ?? Specialization.Default).ToSpecializationDto()).ToList()
        };
    }
}
