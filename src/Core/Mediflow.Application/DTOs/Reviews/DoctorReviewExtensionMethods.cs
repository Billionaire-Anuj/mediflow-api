using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Users;

namespace Mediflow.Application.DTOs.Reviews;

public static class DoctorReviewExtensionMethods
{
    public static DoctorReviewDto ToDoctorReviewDto(this DoctorReview review)
    {
        return new DoctorReviewDto
        {
            Id = review.Id,
            IsActive = review.IsActive,
            AppointmentId = review.AppointmentId,
            DoctorId = review.DoctorId,
            PatientId = review.PatientId,
            Rating = review.Rating,
            Review = review.Review,
            CreatedAt = review.CreatedAt,
            Patient = review.Patient?.ToUserDto()
        };
    }
}
