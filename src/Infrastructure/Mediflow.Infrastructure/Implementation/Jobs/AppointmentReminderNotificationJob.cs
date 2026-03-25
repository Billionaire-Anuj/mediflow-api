using Mediflow.Domain.Common.Enum;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Implementation.Jobs;

public class AppointmentReminderNotificationJob(
    IApplicationDbContext applicationDbContext,
    INotificationService notificationService)
{
    public Task SendPatientAppointmentRemindersAsync()
    {
        var now = DateTime.Now;
        var windowStart = now.AddMinutes(29);
        var windowEnd = now.AddMinutes(31);

        var candidateDates = new[]
        {
            DateOnly.FromDateTime(windowStart),
            DateOnly.FromDateTime(windowEnd)
        };

        var appointments = applicationDbContext.Appointments
            .Include(x => x.Doctor)
            .Include(x => x.Timeslot)
            .Where(x =>
                x.Status == AppointmentStatus.Scheduled &&
                x.PatientId != Guid.Empty &&
                x.Timeslot != null &&
                candidateDates.Contains(x.Timeslot.Date))
            .ToList();

        foreach (var appointment in appointments)
        {
            if (appointment.Timeslot == null)
            {
                continue;
            }

            var appointmentStart = appointment.Timeslot.Date.ToDateTime(appointment.Timeslot.StartTime);
            if (appointmentStart < windowStart || appointmentStart > windowEnd)
            {
                continue;
            }

            var doctorName = appointment.Doctor?.Name ?? "your doctor";
            notificationService.QueueNotification(
                appointment.PatientId,
                NotificationType.Appointment,
                "Appointment starts in 30 minutes",
                $"Your appointment with {doctorName} is coming up shortly.",
                $"/patient/appointments/{appointment.Id}",
                $"patient-appointment-reminder-30m:{appointment.Id:N}");
        }

        if (applicationDbContext is DbContext dbContext && dbContext.ChangeTracker.HasChanges())
        {
            applicationDbContext.SaveChanges();
        }

        return Task.CompletedTask;
    }
}
