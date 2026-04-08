using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Domain.Common;
using Mediflow.Domain.Common.Enum;
using Mediflow.Domain.Entities;
using Mediflow.Infrastructure.Implementation.Services;
using Mediflow.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Mediflow.Tests.Services;

public class AppointmentServiceTests
{
    [Fact]
    public void CancelAppointment_WhenPaidAndCancelledTwoDaysEarly_RefundsCreditsAndReleasesTimeslot()
    {
        using var context = TestApplicationDbContextFactory.Create();

        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();

        SeedPatientRole(context);

        var patient = new User(Guid.Parse(Constants.Roles.Patient.Id), Gender.Female, "Patient Refund", "patientrefund", "patientrefund@test.com", null, null, "hash", "9800000002");
        patient.AssignIdentifier(patientId);

        var doctor = new User(Guid.NewGuid(), Gender.Male, "Dr. Refund", "drrefund", "drrefund@test.com", null, null, "hash", "9800000003");
        doctor.AssignIdentifier(doctorId);

        var slotStart = DateTime.Now.AddDays(3).Date.AddHours(11);
        var timeslot = new Timeslot(Guid.NewGuid(), DateOnly.FromDateTime(slotStart), TimeOnly.FromDateTime(slotStart), TimeOnly.FromDateTime(slotStart.AddMinutes(30)), true);
        timeslot.AssignIdentifier(Guid.NewGuid());

        var appointment = new Appointment(doctorId, patientId, DateTime.Now, timeslot.Id, null, AppointmentStatus.Scheduled, null, "Visit", 200m, isPaidViaGateway: true);
        appointment.AssignIdentifier(appointmentId);

        context.Users.AddRange(patient, doctor);
        context.Timeslots.Add(timeslot);
        context.Appointments.Add(appointment);
        context.SaveChanges();

        var notificationService = new Mock<INotificationService>();
        var service = new AppointmentService(context, new TestApplicationUserService(patientId), notificationService.Object);

        service.CancelAppointment(appointmentId, new CancelAppointmentDto
        {
            AppointmentId = appointmentId,
            CancellationReason = "Need to reschedule"
        });

        var savedAppointment = context.Appointments.Include(x => x.Timeslot).Single(x => x.Id == appointmentId);
        var wallet = context.PatientCredits.Single(x => x.PatientId == patientId);

        Assert.Equal(AppointmentStatus.Canceled, savedAppointment.Status);
        Assert.False(savedAppointment.Timeslot!.IsBooked);
        Assert.Equal(200m, wallet.CreditPoints);

        notificationService.Verify(
            x => x.QueueNotification(
                patientId,
                NotificationType.Appointment,
                "Appointment cancelled",
                It.Is<string>(message => message.Contains("Refunded 200 credits")),
                $"/patient/appointments/{appointmentId}",
                $"patient-appointment-cancelled:{appointmentId:N}"),
            Times.Once);
    }

    [Fact]
    public void PayAppointmentWithCredits_WhenWalletHasEnoughBalance_DeductsBalanceAndMarksAppointmentPaid()
    {
        using var context = TestApplicationDbContextFactory.Create();

        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();

        SeedPatientRole(context);

        var patient = new User(Guid.Parse(Constants.Roles.Patient.Id), Gender.Male, "Wallet Patient", "walletpatient", "wallet@test.com", null, null, "hash", "9800000004");
        patient.AssignIdentifier(patientId);

        var doctor = new User(Guid.NewGuid(), Gender.Male, "Dr. Paid", "drpaid", "drpaid@test.com", null, null, "hash", "9800000005");
        doctor.AssignIdentifier(doctorId);

        var credit = new PatientCredit(patientId, 300m, "TOPUP-1");
        credit.AssignIdentifier(Guid.NewGuid());

        var slotStart = DateTime.Now.AddDays(1).Date.AddHours(9);
        var timeslot = new Timeslot(Guid.NewGuid(), DateOnly.FromDateTime(slotStart), TimeOnly.FromDateTime(slotStart), TimeOnly.FromDateTime(slotStart.AddMinutes(30)));
        timeslot.AssignIdentifier(Guid.NewGuid());

        var appointment = new Appointment(doctorId, patientId, DateTime.Now, timeslot.Id, null, AppointmentStatus.Scheduled, null, "Visit", 150m);
        appointment.AssignIdentifier(appointmentId);

        context.Users.AddRange(patient, doctor);
        context.PatientCredits.Add(credit);
        context.Timeslots.Add(timeslot);
        context.Appointments.Add(appointment);
        context.SaveChanges();

        var notificationService = new Mock<INotificationService>();
        var service = new AppointmentService(context, new TestApplicationUserService(patientId), notificationService.Object);

        service.PayAppointmentWithCredits(appointmentId);

        var savedAppointment = context.Appointments.Single(x => x.Id == appointmentId);
        var savedCredit = context.PatientCredits.Single(x => x.PatientId == patientId);

        Assert.True(savedAppointment.IsPaidViaGateway);
        Assert.Equal(150m, savedCredit.CreditPoints);

        notificationService.Verify(
            x => x.QueueNotification(
                patientId,
                NotificationType.Appointment,
                "Appointment payment completed",
                It.Is<string>(message => message.Contains("150")),
                $"/patient/appointments/{appointmentId}",
                $"patient-appointment-paid:{appointmentId:N}"),
            Times.Once);
    }

    [Fact]
    public void UpdateAppointment_WhenDoctorOwnsAppointment_AllowsReschedulingPatientAppointment()
    {
        using var context = TestApplicationDbContextFactory.Create();

        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();

        var patient = new User(Guid.Parse(Constants.Roles.Patient.Id), Gender.Female, "Reschedule Patient", "reschedulepatient", "reschedulepatient@test.com", null, null, "hash", "9800000006");
        patient.AssignIdentifier(patientId);

        var doctor = new User(Guid.NewGuid(), Gender.Male, "Dr. Reschedule", "drreschedule", "drreschedule@test.com", null, null, "hash", "9800000007");
        doctor.AssignIdentifier(doctorId);

        var schedule = new Schedule(
            doctorId,
            DayOfWeek.Monday,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            validStartDate: DateOnly.FromDateTime(DateTime.Today),
            validEndDate: DateOnly.FromDateTime(DateTime.Today.AddDays(14)));
        schedule.AssignIdentifier(scheduleId);

        var originalSlotStart = DateTime.Now.AddDays(2).Date.AddHours(9);
        var originalTimeslot = new Timeslot(scheduleId, DateOnly.FromDateTime(originalSlotStart), TimeOnly.FromDateTime(originalSlotStart), TimeOnly.FromDateTime(originalSlotStart.AddMinutes(30)), true);
        originalTimeslot.AssignIdentifier(Guid.NewGuid());

        var newSlotStart = DateTime.Now.AddDays(3).Date.AddHours(11);
        var newTimeslot = new Timeslot(scheduleId, DateOnly.FromDateTime(newSlotStart), TimeOnly.FromDateTime(newSlotStart), TimeOnly.FromDateTime(newSlotStart.AddMinutes(30)));
        newTimeslot.AssignIdentifier(Guid.NewGuid());

        var appointment = new Appointment(doctorId, patientId, DateTime.Now, originalTimeslot.Id, null, AppointmentStatus.Scheduled, "Old note", "Old symptoms", 100m);
        appointment.AssignIdentifier(appointmentId);

        context.Users.AddRange(patient, doctor);
        context.Schedules.Add(schedule);
        context.Timeslots.AddRange(originalTimeslot, newTimeslot);
        context.Appointments.Add(appointment);
        context.SaveChanges();

        var persistedAppointment = context.Appointments.Single(x => x.Id == appointmentId);
        Assert.Equal(doctorId, persistedAppointment.DoctorId);

        var currentUserService = new TestApplicationUserService(doctorId);
        Assert.Equal(doctorId, currentUserService.GetUserId);

        var notificationService = new Mock<INotificationService>();
        var service = new AppointmentService(context, currentUserService, notificationService.Object);

        service.RescheduleAppointmentByDoctor(appointmentId, new UpdateAppointmentDto
        {
            AppointmentId = appointmentId,
            TimeslotId = newTimeslot.Id,
            Notes = "Doctor updated note",
            Symptoms = "Updated symptoms"
        });

        var savedAppointment = context.Appointments.Include(x => x.Timeslot).Single(x => x.Id == appointmentId);
        var oldSlot = context.Timeslots.Single(x => x.Id == originalTimeslot.Id);
        var rescheduledSlot = context.Timeslots.Single(x => x.Id == newTimeslot.Id);

        Assert.Equal(newTimeslot.Id, savedAppointment.TimeslotId);
        Assert.Equal("Doctor updated note", savedAppointment.Notes);
        Assert.Equal("Updated symptoms", savedAppointment.Symptoms);
        Assert.False(oldSlot.IsBooked);
        Assert.True(rescheduledSlot.IsBooked);

        notificationService.Verify(
            x => x.QueueNotification(
                patientId,
                NotificationType.Appointment,
                "Appointment updated",
                It.Is<string>(message => message.Contains("rescheduled")),
                $"/patient/appointments/{appointmentId}",
                $"patient-appointment-updated:{appointmentId:N}"),
            Times.Once);
    }

    private static void SeedPatientRole(Mediflow.Infrastructure.Persistence.ApplicationDbContext context)
    {
        if (context.Roles.Any(x => x.Id == Guid.Parse(Constants.Roles.Patient.Id)))
        {
            return;
        }

        var role = new Role(
            Constants.Roles.Patient.Name,
            Constants.Roles.Patient.Description,
            Constants.Roles.Patient.IsDisplayed,
            Constants.Roles.Patient.IsRegisterable);

        role.AssignIdentifier(Guid.Parse(Constants.Roles.Patient.Id));
        context.Roles.Add(role);
        context.SaveChanges();
    }
}
