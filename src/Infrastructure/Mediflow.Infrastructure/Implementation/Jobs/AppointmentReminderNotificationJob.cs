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
        var reminderWindowEnd = now.AddMinutes(31);

        var candidateDates = new[]
        {
            DateOnly.FromDateTime(now),
            DateOnly.FromDateTime(reminderWindowEnd)
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
            // Queue the reminder once the appointment falls within the next ~30 minutes.
            // If a previous minute was missed, later job runs can still create it.
            if (appointmentStart <= now || appointmentStart > reminderWindowEnd)
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
