using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Mediflow.Domain.Common.Enum;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.DTOs.Reviews;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class DoctorReviewService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService,
    INotificationService notificationService) : IDoctorReviewService
{
    public void CreateDoctorReview(Guid appointmentId, CreateDoctorReviewDto review)
    {
        if (appointmentId != review.AppointmentId)
            throw new BadRequestException("Route identifier does not match payload identifier.");

        if (review.Rating is < 1 or > 5)
            throw new BadRequestException("Rating must be between 1 and 5.");

        var userId = applicationUserService.GetUserId;

        var patient = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == userId)
            ?? throw new NotFoundException($"User with the identifier of {userId} could not be found.");

        if (patient.Role?.Id.ToString() != Constants.Roles.Patient.Id)
            throw new BadRequestException("Only patients can submit reviews.");

        var appointmentModel = applicationDbContext.Appointments
            .Include(x => x.Review)
            .FirstOrDefault(x => x.Id == appointmentId)
            ?? throw new NotFoundException($"Appointment with the identifier of {appointmentId} could not be found.");

        if (appointmentModel.PatientId != userId)
            throw new BadRequestException("You can only review your own appointment.");

        if (appointmentModel.Status != AppointmentStatus.Completed)
            throw new BadRequestException("Only completed appointments can be reviewed.");

        if (appointmentModel.Review != null)
            throw new BadRequestException("This appointment has already been reviewed.");

        var doctor = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == appointmentModel.DoctorId)
            ?? throw new NotFoundException($"Doctor with the identifier of {appointmentModel.DoctorId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("The selected user is not a doctor.");

        var reviewModel = new DoctorReview(
            appointmentModel.Id,
            appointmentModel.DoctorId,
            appointmentModel.PatientId,
            review.Rating,
            review.Review);

        applicationDbContext.DoctorReviews.Add(reviewModel);
        applicationDbContext.SaveChanges();

        notificationService.QueueNotification(
            appointmentModel.DoctorId,
            NotificationType.System,
            "New patient review",
            $"{patient.Name} left a {review.Rating}/5 review for a completed appointment.",
            $"/doctor/appointments/{appointmentModel.Id}",
            $"doctor-review-created:{appointmentModel.Id:N}");

        applicationDbContext.SaveChanges();
    }

    public List<DoctorReviewDto> GetDoctorReviewsByDoctorId(Guid doctorId)
    {
        var doctor = applicationDbContext.Users
            .Include(x => x.Role)
            .FirstOrDefault(x => x.Id == doctorId)
            ?? throw new NotFoundException($"Doctor with the identifier of {doctorId} could not be found.");

        if (doctor.Role?.Id.ToString() != Constants.Roles.Doctor.Id)
            throw new BadRequestException("The selected user is not a doctor.");

        return applicationDbContext.DoctorReviews
            .AsNoTracking()
            .Where(x => x.DoctorId == doctorId)
            .Include(x => x.Patient)
                .ThenInclude(x => x!.Role)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToDoctorReviewDto())
            .ToList();
    }
}
