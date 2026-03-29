using Mediflow.Domain.Common.Enum;
using Mediflow.Domain.Entities;
using Mediflow.Infrastructure.Implementation.Jobs;
using Mediflow.Tests.Common;
using Moq;

namespace Mediflow.Tests.Jobs;

public class AppointmentReminderNotificationJobTests
{
    [Fact]
    public async Task SendPatientAppointmentRemindersAsync_QueuesReminderOnlyForAppointmentsInThirtyMinuteWindow()
    {
        using var context = TestApplicationDbContextFactory.Create();

        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();

        var doctor = new User(Guid.NewGuid(), Gender.Male, "Dr. Green", "drgreen", "doctor@test.com", null, null, "hash", "9800000000");
        doctor.AssignIdentifier(doctorId);

        var patient = new User(Guid.NewGuid(), Gender.Female, "Patient One", "patientone", "patient@test.com", null, null, "hash", "9800000001");
        patient.AssignIdentifier(patientId);

        var reminderStart = DateTime.Now.AddMinutes(30);
        var reminderTimeslot = new Timeslot(Guid.NewGuid(), DateOnly.FromDateTime(reminderStart), TimeOnly.FromDateTime(reminderStart), TimeOnly.FromDateTime(reminderStart.AddMinutes(30)), true);
        reminderTimeslot.AssignIdentifier(Guid.NewGuid());

        var reminderAppointment = new Appointment(doctorId, patientId, DateTime.Now, reminderTimeslot.Id, null, AppointmentStatus.Scheduled, null, "Checkup", 100m);
        reminderAppointment.AssignIdentifier(Guid.NewGuid());

        var laterStart = DateTime.Now.AddHours(2);
        var laterTimeslot = new Timeslot(Guid.NewGuid(), DateOnly.FromDateTime(laterStart), TimeOnly.FromDateTime(laterStart), TimeOnly.FromDateTime(laterStart.AddMinutes(30)), true);
        laterTimeslot.AssignIdentifier(Guid.NewGuid());

        var laterAppointment = new Appointment(doctorId, patientId, DateTime.Now, laterTimeslot.Id, null, AppointmentStatus.Scheduled, null, "Follow up", 100m);
        laterAppointment.AssignIdentifier(Guid.NewGuid());

        context.Users.AddRange(doctor, patient);
        context.Timeslots.AddRange(reminderTimeslot, laterTimeslot);
        context.Appointments.AddRange(reminderAppointment, laterAppointment);
        context.SaveChanges();

        var notificationService = new Mock<Mediflow.Application.Interfaces.Services.INotificationService>();
        var job = new AppointmentReminderNotificationJob(context, notificationService.Object);

        await job.SendPatientAppointmentRemindersAsync();

        notificationService.Verify(
            x => x.QueueNotification(
                patientId,
                NotificationType.Appointment,
                "Appointment starts in 30 minutes",
                It.Is<string>(message => message.Contains("Dr. Green")),
                $"/patient/appointments/{reminderAppointment.Id}",
                $"patient-appointment-reminder-30m:{reminderAppointment.Id:N}"),
            Times.Once);

        notificationService.Verify(
            x => x.QueueNotification(
                patientId,
                NotificationType.Appointment,
                It.IsAny<string>(),
                It.IsAny<string>(),
                $"/patient/appointments/{laterAppointment.Id}",
                It.IsAny<string>()),
            Times.Never);
    }
}
