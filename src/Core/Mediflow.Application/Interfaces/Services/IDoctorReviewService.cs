using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Reviews;

namespace Mediflow.Application.Interfaces.Services;

public interface IDoctorReviewService : ITransientService
{
    void CreateDoctorReview(Guid appointmentId, CreateDoctorReviewDto review);

    List<DoctorReviewDto> GetDoctorReviewsByDoctorId(Guid doctorId);
}
