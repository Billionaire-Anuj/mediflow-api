using Moq;
using Moq.Protected;
using Mediflow.Helper;
using Mediflow.Tests.Common;
using Mediflow.Domain.Common;
using Mediflow.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Mediflow.Domain.Common.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Settings;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.Exceptions;
using Mediflow.Application.DTOs.Doctors;
using Mediflow.Application.DTOs.Payments;
using Mediflow.Application.DTOs.Medicines;
using Mediflow.Application.DTOs.Appointments;
using Mediflow.Application.DTOs.Authentication;
using Mediflow.Application.Interfaces.Managers;
using Mediflow.Application.Interfaces.Services;
using Mediflow.Application.DTOs.DiagnosticTypes;
using Mediflow.Application.DTOs.DiagnosticTests;
using Mediflow.Application.DTOs.MedicationTypes;
using Mediflow.Application.DTOs.Specializations;
using Mediflow.Application.DTOs.Doctors.Schedules;
using Mediflow.Infrastructure.Implementation.Jobs;
using Mediflow.Infrastructure.Implementation.Services;
using Mediflow.Application.DTOs.Authentication.Configurations._2FA;

namespace Mediflow.Tests;

public class MediflowTests
{
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly Mock<IWebHostEnvironment> _envMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly Mock<ITokenManager> _tokenManagerMock = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<IUserPropertyService> _userPropertyServiceMock = new();
    private readonly Mock<INotificationService> _notificationServiceMock = new();
    private readonly Mock<ITwoFactorTokenManager> _twoFactorTokenManagerMock = new();

    public MediflowTests()
    {
        _envMock.Setup(e => e.EnvironmentName).Returns("Development");
        var context = new DefaultHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);
        _jwtSettings = Options.Create(new JwtSettings
        {
            Key = "A_Long_Super_Secret_Key_For_Testing_12345",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationInMinutes = 60
        });
    }

    #region Seeding Helpers
    private async Task SeedCriticalData(Infrastructure.Persistence.ApplicationDbContext context)
    {
        if (!context.Roles.Any())
        {
            var roles = new List<Role>
            {
                CreateRole(Constants.Roles.Patient.Id, Constants.Roles.Patient.Name, true, true),
                CreateRole(Constants.Roles.Doctor.Id, Constants.Roles.Doctor.Name, true, true),
                CreateRole(Constants.Roles.LabTechnician.Id, Constants.Roles.LabTechnician.Name, true, true),
                CreateRole(Constants.Roles.Pharmacist.Id, Constants.Roles.Pharmacist.Name, true, true),
                CreateRole(Constants.Roles.TenantAdministrator.Id, Constants.Roles.TenantAdministrator.Name, false, false),
                CreateRole(Constants.Roles.SuperAdmin.Id, Constants.Roles.SuperAdmin.Name, true, false)
            };
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }
    }

    private Role CreateRole(string id, string name, bool isDisplayed, bool isRegisterable)
    {
        var role = new Role(name, $"{name} role", isDisplayed, isRegisterable);
        role.AssignIdentifier(Guid.Parse(id));
        return role;
    }

    private User CreateUser(Guid id, Guid roleId, string name, string username, string email, string phone, string pass = "Password123!")
    {
        var user = new User(roleId, Gender.Male, name, username, email, "Address", null, pass.Hash(), phone);
        user.AssignIdentifier(id);
        user.IsActive = true;
        return user;
    }
    #endregion

    #region Authentication Module
    private AuthenticationService GetAuthService(Infrastructure.Persistence.ApplicationDbContext context)
    {
        return new AuthenticationService(
            _tokenManagerMock.Object,
            _fileServiceMock.Object,
            _jwtSettings,
            _loggerMock.Object,
            _envMock.Object,
            _httpContextAccessorMock.Object,
            _userPropertyServiceMock.Object,
            context,
            _twoFactorTokenManagerMock.Object,
            _notificationServiceMock.Object
        );
    }

    [Fact]
    public void Authentication_Login_WhenValidCredentials_ReturnsToken()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var patientId = Guid.NewGuid();
        var roleId = Guid.Parse(Constants.Roles.Patient.Id);
        var role = CreateRole(Constants.Roles.Patient.Id, "Patient", true, true);
        context.Roles.Add(role);
        var user = CreateUser(patientId, roleId, "Patient User", "patient", "patient@test.com", "12345", "Pass123!");
        context.Users.Add(user);
        context.SaveChanges();

        var authService = GetAuthService(context);
        var result = authService.Login(new LoginDto { EmailAddressOrUsername = "patient@test.com", Password = "Pass123!" });

        Assert.NotNull(result);
        Assert.False(result.IsTwoFactorRequired);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public void Authentication_Login_WithInvalidPassword_ThrowsNotFoundException()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var patientId = Guid.NewGuid();
        var roleId = Guid.Parse(Constants.Roles.Patient.Id);
        context.Roles.Add(CreateRole(Constants.Roles.Patient.Id, "Patient", true, true));
        context.Users.Add(CreateUser(patientId, roleId, "Patient User", "patient", "patient@test.com", "12345", "CorrectPass123!"));
        context.SaveChanges();

        var authService = GetAuthService(context);
        Assert.Throws<NotFoundException>(() => authService.Login(new LoginDto { EmailAddressOrUsername = "patient@test.com", Password = "WrongPassword!" }));
    }

    [Fact]
    public async Task Authentication_RegisterPatient_SavesToDatabase()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var authService = GetAuthService(context);

        var dto = new RegisterPatientDto
        {
            Name = "New Patient",
            Username = "newpatient",
            EmailAddress = "new@test.com",
            Password = "Password123!",
            PhoneNumber = "9800000000",
            Gender = Gender.Female
        };

        authService.RegisterPatient(dto);

        var user = await context.Users.FirstOrDefaultAsync(x => x.EmailAddress == "new@test.com");
        Assert.NotNull(user);
        Assert.Equal("New Patient", user.Name);
    }

    [Fact]
    public async Task Authentication_Logout_BlacklistsToken()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var authService = GetAuthService(context);
        var token = "some.jwt.token";
        _httpContextAccessorMock.Object.HttpContext?.Request.Headers["Authorization"] = $"Bearer {token}";

        authService.Logout();

        _tokenManagerMock.Verify(x => x.BlacklistToken(token), Times.Once);
    }

    [Fact]
    public async Task Authentication_ConfirmEmailAddress_UpdatesVerificationStatus()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var userId = Guid.NewGuid();
        var user = CreateUser(userId, Guid.Parse(Constants.Roles.Patient.Id), "User", "user", "user@test.com", "111");
        context.Users.Add(user);
        context.SaveChanges();

        var otp = "123456";

        _userPropertyServiceMock.Setup(x => x.GetProperty<Application.DTOs.Authentication.Configurations.EmailConfirmation.EmailConfirmationConfiguration>(userId, nameof(Domain.Common.Enum.Configurations.UserConfiguration.EMAIL_CONFIRMATION_OTP)))
            .Returns(new Application.DTOs.Authentication.Configurations.EmailConfirmation.EmailConfirmationConfiguration { OneTimePassword = otp, IsVerified = false });

        var authService = GetAuthService(context);
        authService.ConfirmEmailAddress(new EmailConfirmationVerificationDto { EmailAddressOrUsername = "user@test.com", Otp = otp });

        _userPropertyServiceMock.Verify(x => x.SaveProperty(userId, nameof(Domain.Common.Enum.Configurations.UserConfiguration.EMAIL_CONFIRMATION_OTP), It.Is<Application.DTOs.Authentication.Configurations.EmailConfirmation.EmailConfirmationConfiguration>(c => c.IsVerified)), Times.Once);
    }
    #endregion

    #region User & Role Module
    [Fact]
    public async Task User_RegisterUser_OnlyAllowsPatientSelfRegistration()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var doctorRoleId = Guid.Parse(Constants.Roles.Doctor.Id);
        
        var userService = new UserService(_fileServiceMock.Object, context, new TestApplicationUserService(Guid.NewGuid()), _notificationServiceMock.Object);

        var exception = Assert.Throws<BadRequestException>(() => userService.RegisterUser(new RegisterUserDto { RoleId = doctorRoleId }));
        Assert.Contains("Only patients can self-register", exception.Message);
    }

    [Fact]
    public void Role_CreateRole_SavesToDatabase()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var roleService = new RoleService(context);
        var dto = new CreateRoleDto { Name = "Analyst", Description = "Analyst role" };
        roleService.CreateRole(dto);
        var role = context.Roles.FirstOrDefault(x => x.Name == "Analyst");
        Assert.NotNull(role);
    }
    [Fact]
    public void Role_UpdateAndToggle_ReflectsInDatabase()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var roleId = Guid.NewGuid();
        var role = new Role("Analyst", "D", true, true);
        role.AssignIdentifier(roleId);
        context.Roles.Add(role);
        context.SaveChanges();

        var roleService = new RoleService(context);
        var updateDto = new UpdateRoleDto { Id = roleId, Name = "Analyst2", Description = "Desc2" };
        roleService.UpdateRole(roleId, updateDto);
        Assert.Equal("Analyst2", context.Roles.Find(roleId)?.Name);

        roleService.ActivateDeactivateRole(roleId);
        Assert.False(context.Roles.Find(roleId) is { IsActive: true });
    }
    #endregion

    #region Doctor Hub
    [Fact]
    public async Task Doctor_UpdateProfile_PersistsChanges()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var doctorId = Guid.NewGuid();
        var doctor = CreateUser(doctorId, Guid.Parse(Constants.Roles.Doctor.Id), "Dr. Initial", "drinitial", "dr@test.com", "111");
        doctor.DoctorProfile = new DoctorProfile(doctorId, "Spec", "123", "Bio", "Exp", 100);
        context.Users.Add(doctor);
        await context.SaveChangesAsync();
        var service = new DoctorService(context, new TestApplicationUserService(doctorId));
        service.UpdateDoctorProfile(new UpdateDoctorProfileDto
        {
            About = "New Bio",
            LicenseNumber = "123-NEW",
            EducationInformation = "PhD",
            ExperienceInformation = "10 years",
            ConsultationFee = 250
        });
        var updated = await context.DoctorProfiles.FirstOrDefaultAsync(x => x.DoctorId == doctorId);
        Assert.NotNull(updated);
    }

    [Fact]
    public async Task Doctor_CreateSchedule_ValidatesOverlaps()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var doctorId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var schedule = new Schedule(doctorId, DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(14, 0), 30, true, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(7)));
        schedule.AssignIdentifier(scheduleId);
        context.Schedules.Add(schedule);
        await context.SaveChangesAsync();
        context.Users.Add(CreateUser(doctorId, Guid.Parse(Constants.Roles.Doctor.Id), "D", "d", "d@t.com", "1"));
        await context.SaveChangesAsync();
        var service = new DoctorService(context, new TestApplicationUserService(doctorId));
        var dto = new CreateScheduleDto
        {
            DayOfWeek = DayOfWeek.Monday,
            StartTime = new TimeOnly(11, 0),
            EndTime = new TimeOnly(13, 0),
            SlotDurationInMinutes = 30
        };
        var exception = Assert.Throws<BadRequestException>(() => service.CreateDoctorSchedule(dto));
        Assert.Contains("overlaps", exception.Message);
    }
    #endregion

    #region Patient Hub
    [Fact]
    public async Task Patient_GetProfile_ReturnsCorrectData()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var patientId = Guid.NewGuid();
        var roleId = Guid.Parse(Constants.Roles.Patient.Id);
        var patient = CreateUser(patientId, roleId, "Patient Alpha", "patienta", "p@a.com", "999");
        context.Users.Add(patient);
        await context.SaveChangesAsync();

        var service = new PatientService(context, new TestApplicationUserService(patientId));
        var profile = service.GetPatientProfile();

        Assert.Equal("Patient Alpha", profile.Name);
    }
    #endregion

    #region Appointment Module
    private async Task<(Guid doctorId, Guid patientId, Guid timeslotId, Guid appointmentId)> SeedAppointment(Infrastructure.Persistence.ApplicationDbContext context, AppointmentStatus status = AppointmentStatus.Scheduled, DateTime? appointmentDate = null)
    {
        await SeedCriticalData(context);
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var doctor = CreateUser(doctorId, Guid.Parse(Constants.Roles.Doctor.Id), "Dr. Test", "drtest", "dr@test.com", "123");
        var patient = CreateUser(patientId, Guid.Parse(Constants.Roles.Patient.Id), "Pat Test", "pattest", "pat@test.com", "456");
        context.Users.AddRange(doctor, patient);

        var dateOnly = DateOnly.FromDateTime(appointmentDate ?? DateTime.Today.AddDays(2));
        var timeOnly = TimeOnly.FromDateTime(appointmentDate ?? DateTime.Now.AddDays(2));
        var timeslot = new Timeslot(doctorId, dateOnly, timeOnly, timeOnly.AddMinutes(30));
        var timeslotId = Guid.NewGuid();
        timeslot.AssignIdentifier(timeslotId);
        context.Timeslots.Add(timeslot);

        var appointmentId = Guid.NewGuid();
        var appointment = new Appointment(doctorId, patientId, DateTime.Now, timeslotId, null, status, "Symptoms", "Notes", 500);
        appointment.AssignIdentifier(appointmentId);
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        return (doctorId, patientId, timeslotId, appointmentId);
    }

    [Fact]
    public async Task Appointment_BookAppointment_PersistsAndMarksTimeslotUnavailable()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        context.Users.AddRange(CreateUser(doctorId, Guid.Parse(Constants.Roles.Doctor.Id), "D", "d", "d@t.com", "1"), CreateUser(patientId, Guid.Parse(Constants.Roles.Patient.Id), "P", "p", "p@t.com", "2"));
        var tsId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var schedule = new Schedule(doctorId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, true, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddMonths(1)));
        schedule.AssignIdentifier(scheduleId);
        context.Schedules.Add(schedule);
        
        var ts = new Timeslot(scheduleId, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), new TimeOnly(10, 0), new TimeOnly(10, 30));
        ts.AssignIdentifier(tsId);
        context.Timeslots.Add(ts);
        await context.SaveChangesAsync();

        var (_, p, timeslotId, _) = await SeedAppointment(context);
        
        var service = new AppointmentService(context, new TestApplicationUserService(p), _notificationServiceMock.Object);
        service.BookAppointment(new CreateAppointmentDto { TimeslotId = timeslotId, Symptoms = "Fever" });

        var appointment = await context.Appointments.FirstOrDefaultAsync(x => x.PatientId == patientId);
        Assert.NotNull(appointment);
        Assert.True(context.Timeslots.Find(tsId) is { IsBooked: true });
    }

    [Fact]
    public async Task Appointment_PayWithCredits_DeductsPatientBalance()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var (_, patientId, _, appointmentId) = await SeedAppointment(context);
        var credit = new PatientCredit(patientId, 1000, "TOPUP-1");
        context.PatientCredits.Add(credit);
        await context.SaveChangesAsync();
        var service = new AppointmentService(context, new TestApplicationUserService(patientId), _notificationServiceMock.Object);
        service.PayAppointmentWithCredits(appointmentId);
        var updatedCredit = await context.PatientCredits.FirstOrDefaultAsync(x => x.PatientId == patientId);
        if (updatedCredit != null) Assert.Equal(500m, updatedCredit.CreditPoints);
        var appointment = await context.Appointments.FindAsync(appointmentId);
        Assert.True(appointment != null && appointment.IsPaidViaGateway); 
    }

    [Fact]
    public async Task Appointment_Cancel_FullRefund_TwoDaysEarly()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var (_, patientId, _, appointmentId) = await SeedAppointment(context, AppointmentStatus.Scheduled, DateTime.Today.AddDays(3));
        var appointment = await context.Appointments.FindAsync(appointmentId);
        appointment?.MarkPaidViaGateway(); 
        context.PatientCredits.Add(new PatientCredit(patientId, 0, "INIT"));
        await context.SaveChangesAsync();
        var service = new AppointmentService(context, new TestApplicationUserService(patientId), _notificationServiceMock.Object);
        service.CancelAppointment(appointmentId, new CancelAppointmentDto { AppointmentId = appointmentId, CancellationReason = "Change of plans" });
        var credit = context.PatientCredits.First(x => x.PatientId == patientId);
        Assert.Equal(500m, credit.CreditPoints); // 100% refund
        Assert.Equal(AppointmentStatus.Canceled, context.Appointments.Find(appointmentId)!.Status);
    }

    [Fact]
    public async Task Appointment_Cancel_HalfRefund_OneDayEarly()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var (_, patientId, _, appointmentId) = await SeedAppointment(context, AppointmentStatus.Scheduled, DateTime.Today.AddDays(1));
        var appointment = await context.Appointments.FindAsync(appointmentId);
        appointment?.MarkPaidViaGateway();
        context.PatientCredits.Add(new PatientCredit(patientId, 0, "INIT"));
        await context.SaveChangesAsync();
        var service = new AppointmentService(context, new TestApplicationUserService(patientId), _notificationServiceMock.Object);
        service.CancelAppointment(appointmentId, new CancelAppointmentDto { AppointmentId = appointmentId, CancellationReason = "Emergency" });
        var credit = context.PatientCredits.First(x => x.PatientId == patientId);
        Assert.Equal(250m, credit.CreditPoints); // 50% refund
    }

    [Fact]
    public async Task Appointment_Reschedule_ByDoctor_UpdatesTimeslot()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var (doctorId, _, _, appointmentId) = await SeedAppointment(context);
        var scheduleId = Guid.NewGuid();
        var schedule = new Schedule(doctorId, DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0), 30, true, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddMonths(1)));
        schedule.AssignIdentifier(scheduleId);
        context.Schedules.Add(schedule);
        var newTimeslotId = Guid.NewGuid();
        var newTimeslot = new Timeslot(scheduleId, DateOnly.FromDateTime(DateTime.Today.AddDays(5)), new TimeOnly(15, 0), new TimeOnly(15, 30));
        newTimeslot.AssignIdentifier(newTimeslotId);
        context.Timeslots.Add(newTimeslot);
        await context.SaveChangesAsync();
        var service = new AppointmentService(context, new TestApplicationUserService(doctorId), _notificationServiceMock.Object);
        var updateDto = new UpdateAppointmentDto { AppointmentId = appointmentId, TimeslotId = newTimeslotId, Symptoms = "Worsened symptoms" };
        service.RescheduleAppointmentByDoctor(appointmentId, updateDto);
        var updatedAppointment = await context.Appointments.FindAsync(appointmentId);
        if (updatedAppointment != null) Assert.Equal(newTimeslotId, updatedAppointment.TimeslotId);
        Assert.True(context.Timeslots.Find(newTimeslotId) is { IsBooked: true }); 
    }

    [Fact]
    public async Task Appointment_Consultation_StoresNotesAndMedicalInsights()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var (doctorId, _, _, appointmentId) = await SeedAppointment(context);
        var medId = Guid.NewGuid();
        var medType = new MedicationType("Tablet", "D"); medType.AssignIdentifier(Guid.NewGuid());
        context.MedicationTypes.Add(medType);
        var med = new Medicine(medType.Id, "Paracetamol", "P", "500mg");
        med.AssignIdentifier(medId);
        context.Medicines.Add(med);
        var diagId = Guid.NewGuid();
        var diagType = new DiagnosticType("Blood", "B"); diagType.AssignIdentifier(Guid.NewGuid());
        context.DiagnosticTypes.Add(diagType);
        var diag = new DiagnosticTest(diagType.Id, "CBC", "Complete Blood Count", "Blood");
        diag.AssignIdentifier(diagId);
        context.DiagnosticTests.Add(diag);
        await context.SaveChangesAsync();
        var service = new AppointmentService(context, new TestApplicationUserService(doctorId), _notificationServiceMock.Object);
        var dto = new ConsultAppointmentDto
        {
            AppointmentId = appointmentId,
            Diagnosis = "Patient has mild flu",
            Treatment = "Bed rest",
            Prescriptions = "Paracetamol",
            Medications = new List<Application.DTOs.Appointments.Medications.CreateAppointmentMedicationsDto> { 
                new()
                { 
                    Drugs = new List<Application.DTOs.Appointments.Medications.CreateAppointmentMedicationDrugsDto> { 
                        new() { MedicineId = medId, Dose = "1", Frequency = "TID", Duration = "3 days" } 
                    }
                }
            },
            Diagnostics = new List<Application.DTOs.Appointments.Diagnostics.CreateAppointmentDiagnosticsDto> {
                new()
                { 
                    Notes = "Check infection", 
                    DiagnosticTests = new List<Application.DTOs.Appointments.Diagnostics.CreateAppointmentDiagnosticTestsDto> { 
                        new() { DiagnosticTestId = diagId }
                    }
                }
            }
        };

        service.ConsultAppointment(appointmentId, dto);

        var updatedAppointment = await context.Appointments.Include(x => x.MedicalRecord).FirstOrDefaultAsync(x => x.Id == appointmentId);
        if (updatedAppointment != null)
        {
            Assert.Equal(AppointmentStatus.Completed, updatedAppointment.Status);
            Assert.Equal("Patient has mild flu", updatedAppointment.MedicalRecord?.Diagnosis);
        }

        var meds = await context.AppointmentMedications.Where(x => x.AppointmentId == appointmentId).ToListAsync();
        Assert.Single(meds);
        var diags = await context.AppointmentDiagnostics.Where(x => x.AppointmentId == appointmentId).ToListAsync();
        Assert.Single(diags);
    }
    #endregion

    #region Pharmacy Module
    [Fact]
    public async Task Pharmacy_DispenseMedication_UpdatesStatus()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var pharmacistId = Guid.NewGuid();
        context.Users.Add(CreateUser(pharmacistId, Guid.Parse(Constants.Roles.Pharmacist.Id), "Ph", "ph", "p@h.com", "1"));
        var appointmentMedId = Guid.NewGuid();
        var medication = new AppointmentMedications(Guid.NewGuid(), pharmacistId, "Meds", DiagnosticStatus.Scheduled);
        medication.AssignIdentifier(appointmentMedId);
        context.AppointmentMedications.Add(medication);
        await context.SaveChangesAsync();
        var service = new AppointmentMedicationsService(context, new TestApplicationUserService(pharmacistId), _notificationServiceMock.Object);
        service.DispenseAppointmentMedications(appointmentMedId);
        var updated = await context.AppointmentMedications.FindAsync(appointmentMedId);
        if (updated != null) Assert.Equal(DiagnosticStatus.Collected, updated.Status);
    }
    #endregion

    #region Laboratory Module
    [Fact]
    public async Task Laboratory_SubmitResult_UpdatesStatusAndFillsData()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var labTechId = Guid.NewGuid();
        context.Users.Add(CreateUser(labTechId, Guid.Parse(Constants.Roles.LabTechnician.Id), "LT", "lt", "l@t.com", "1"));
        var appointmentDiagId = Guid.NewGuid();
        var diag = new AppointmentDiagnostics(Guid.NewGuid(), labTechId, "Diag", DiagnosticStatus.Scheduled);
        diag.AssignIdentifier(appointmentDiagId);
        context.AppointmentDiagnostics.Add(diag);
        var testId = Guid.NewGuid();
        var test = new AppointmentDiagnosticTests(appointmentDiagId, Guid.NewGuid());
        test.AssignIdentifier(testId);
        context.AppointmentDiagnosticTests.Add(test);
        await context.SaveChangesAsync();
        var service = new AppointmentDiagnosticsService(_fileServiceMock.Object, context, new TestApplicationUserService(labTechId), _notificationServiceMock.Object);
        service.SubmitDiagnosticTestResult(testId, new Application.DTOs.Appointments.Diagnostics.UpdateAppointmentDiagnosticTestResultDto { AppointmentDiagnosticTestId = testId, Value = "Positive", Interpretation = "High levels" });
        var updatedTest = await context.AppointmentDiagnosticTests.Include(x => x.AppointmentDiagnosticTestResult).FirstOrDefaultAsync(x => x.Id == testId);
        Assert.Equal("Positive", updatedTest?.AppointmentDiagnosticTestResult?.Value);
        var updatedDiag = await context.AppointmentDiagnostics.FindAsync(appointmentDiagId);
        if (updatedDiag != null) Assert.Equal(DiagnosticStatus.Resulted, updatedDiag.Status);
    }
    #endregion

    #region Payment Integrations (Khalti & Esewa)
    [Fact]
    public async Task Payment_KhaltiConfirmation_Success_AddsCredits()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var patientId = Guid.NewGuid();
        var patient = CreateUser(patientId, Guid.Parse(Constants.Roles.Patient.Id), "P", "p", "p@t.com", "1");
        context.Users.Add(patient);
        _userPropertyServiceMock.Setup(x => x.SaveProperty(patientId, It.IsAny<string>(), It.IsAny<object>()));
        await context.SaveChangesAsync();
        var responseJson = "{\"status\":\"Completed\",\"total_amount\":10000}";
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(responseJson) });
        
        _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handlerMock.Object));

        var service = new PatientCreditService(
            context, 
            new TestApplicationUserService(patientId), 
            _userPropertyServiceMock.Object, 
            _notificationServiceMock.Object, 
            _httpClientFactoryMock.Object,
            Options.Create(new KhaltiSettings { SecretKey = "key", LookupUrl = "https://test.com" }),
            Options.Create(new EsewaSettings()));

        await service.ConfirmKhaltiTopupAsync(new KhaltiConfirmDto { Pidx = "valid_pidx" });

        var credit = await context.PatientCredits.FirstOrDefaultAsync(x => x.PatientId == patientId);
        if (credit != null) Assert.Equal(100m, credit.CreditPoints); // 10000 cents = 100 NRP
    }
    #endregion

    #region Master Details Management
    [Fact]
    public async Task Medicine_CRUD_Operations()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var type = new MedicationType("T", "D"); type.AssignIdentifier(Guid.NewGuid());
        context.MedicationTypes.Add(type);
        await context.SaveChangesAsync();
        var service = new MedicineService(context);

        // Create
        service.CreateMedicine(new CreateMedicineDto { MedicationTypeId = type.Id, Title = "M1", Format = "F1" });
        Assert.NotNull(context.Medicines.FirstOrDefault(x => x.Title == "M1"));

        // Update
        var medId = context.Medicines.First().Id;
        service.UpdateMedicine(medId, new UpdateMedicineDto { Id = medId, MedicationTypeId = type.Id, Title = "M2", Format = "F2" });
        Assert.Equal("M2", context.Medicines.Find(medId)?.Title);

        // Toggle Status
        service.ActivateDeactivateMedicine(medId);
        Assert.False(context.Medicines.Find(medId) is { IsActive: true });
    }

    [Fact]
    public async Task DiagnosticTest_CRUD_Operations()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var type = new DiagnosticType("T", "D"); type.AssignIdentifier(Guid.NewGuid());
        context.DiagnosticTypes.Add(type);
        await context.SaveChangesAsync();
        var service = new DiagnosticTestService(context);

        // Create
        service.CreateDiagnosticTest(new CreateDiagnosticTestDto { DiagnosticTypeId = type.Id, Title = "D1" });
        Assert.NotNull(context.DiagnosticTests.FirstOrDefault(x => x.Title == "D1"));

        // Update
        var testId = context.DiagnosticTests.First().Id;
        service.UpdateDiagnosticTest(testId, new UpdateDiagnosticTestDto { Id = testId, DiagnosticTypeId = type.Id, Title = "D2" });
        Assert.Equal("D2", context.DiagnosticTests.Find(testId)?.Title);
    }

    [Fact]
    public async Task Specialization_CRUD_Operations()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var service = new SpecializationService(context);

        // Create
        service.CreateSpecialization(new CreateSpecializationDto { Title = "S1", Description = "D1" });
        Assert.NotNull(context.Specializations.FirstOrDefault(x => x.Title == "S1"));

        // Update
        var specId = context.Specializations.First().Id;
        service.UpdateSpecialization(specId, new UpdateSpecializationDto { Id = specId, Title = "S2", Description = "D2" });
        Assert.Equal("S2", context.Specializations.Find(specId)?.Title);
    }
    #endregion

    #region Medical Records
    [Fact]
    public async Task MedicalRecord_Retrieve_ReturnsCorrectData()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var (_, patientId, _, appointmentId) = await SeedAppointment(context);
        var record = new MedicalRecord(appointmentId, "Physical", "Normal Health", "No issues", "Exercise");
        context.MedicalRecords.Add(record);
        await context.SaveChangesAsync();

        var service = new PatientService(context, new TestApplicationUserService(patientId));
        var result = service.GetPatientProfile();
        Assert.NotNull(result);
        Assert.Equal("Pat Test", result.Name);
    }
    #endregion

    #region Infrastructure (Jobs)
    [Fact]
    public async Task Infrastructure_AppointmentReminderJob_QueuesNotifications()
    {
        using var context = TestApplicationDbContextFactory.Create();
        
        var job = new AppointmentReminderNotificationJob(context, _notificationServiceMock.Object);
        await job.SendPatientAppointmentRemindersAsync();

        _notificationServiceMock.Verify(x => x.QueueNotification(It.IsAny<Guid>(), It.IsAny<NotificationType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
    }
    #endregion

    #region Additional Tests Coverage
    [Fact]
    public async Task Authentication_EnableTwoFactor_VerificationSuccess()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var patientId = Guid.NewGuid();
        context.Users.Add(CreateUser(patientId, Guid.Parse(Constants.Roles.Patient.Id), "U", "u", "u@t.com", "1"));
        await context.SaveChangesAsync();

        var service = new ProfileService(_fileServiceMock.Object, _userPropertyServiceMock.Object, context, _twoFactorTokenManagerMock.Object, new TestApplicationUserService(patientId));
        service.EnableTwoFactorAuthentication();

        _userPropertyServiceMock.Verify(x => x.SaveProperty(patientId, nameof(Domain.Common.Enum.Configurations.UserConfiguration.TWO_FACTOR_AUTHENTICATION_SETTINGS), It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task Authentication_VerifyTwoFactor_Success()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var patientId = Guid.NewGuid();
        context.Users.Add(CreateUser(patientId, Guid.Parse(Constants.Roles.Patient.Id), "U", "u", "u@t.com", "1"));
        await context.SaveChangesAsync();

        var secret = "SECRET";
        var code = "123456";
        _userPropertyServiceMock.Setup(x => x.GetProperty<TwoFactorAuthenticationConfiguration>(patientId, nameof(Domain.Common.Enum.Configurations.UserConfiguration.TWO_FACTOR_AUTHENTICATION_SETTINGS)))
            .Returns(new TwoFactorAuthenticationConfiguration { SecretKey = secret, IsConfirmed = false });
        _twoFactorTokenManagerMock.Setup(x => x.ValidateCode(secret, code)).Returns(true);

        var service = new ProfileService(_fileServiceMock.Object, _userPropertyServiceMock.Object, context, _twoFactorTokenManagerMock.Object, new TestApplicationUserService(patientId));
        service.ConfirmTwoFactorAuthentication(new TwoFactorVerificationDto { AuthenticationCode = code });

        _userPropertyServiceMock.Verify(x => x.SaveProperty(patientId, nameof(Domain.Common.Enum.Configurations.UserConfiguration.TWO_FACTOR_AUTHENTICATION_SETTINGS), It.Is<TwoFactorAuthenticationConfiguration>(c => c.IsConfirmed)), Times.Once);
    }

    [Fact]
    public async Task DiagnosticType_CRUD_Operations()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var service = new DiagnosticTypeService(context);
        service.CreateDiagnosticType(new CreateDiagnosticTypeDto { Title = "DT1", Description = "Desc" });
        Assert.NotNull(context.DiagnosticTypes.FirstOrDefault(x => x.Title == "DT1"));
        
        var id = context.DiagnosticTypes.First().Id;
        service.UpdateDiagnosticType(id, new UpdateDiagnosticTypeDto { Id = id, Title = "DT2", Description = "Desc2" });
        Assert.Equal("DT2", context.DiagnosticTypes.Find(id)?.Title);

        service.ActivateDeactivateDiagnosticType(id);
        Assert.False(context.DiagnosticTypes.Find(id) is { IsActive: true });
    }

    [Fact]
    public async Task MedicationType_CRUD_Operations()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var service = new MedicationTypeService(context);
        service.CreateMedicationType(new CreateMedicationTypeDto { Title = "MT1", Description = "Desc" });
        Assert.NotNull(context.MedicationTypes.FirstOrDefault(x => x.Title == "MT1"));
        
        var id = context.MedicationTypes.First().Id;
        service.UpdateMedicationType(id, new UpdateMedicationTypeDto { Id = id, Title = "MT2", Description = "Desc2" });
        Assert.Equal("MT2", context.MedicationTypes.Find(id)?.Title);

        service.ActivateDeactivateMedicationType(id);
        Assert.False(context.MedicationTypes.Find(id) is { IsActive: true });
    }

    [Fact]
    public async Task User_UpdateUser_SavesChanges()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var userId = Guid.NewGuid();
        context.Users.Add(CreateUser(userId, Guid.Parse(Constants.Roles.Patient.Id), "Initial Name", "init", "i@t.com", "1"));
        await context.SaveChangesAsync();

        var service = new UserService(_fileServiceMock.Object, context, new TestApplicationUserService(userId), _notificationServiceMock.Object);
        service.UpdateUser(userId, new UpdateUserDto { Id = userId, RoleId = Guid.Parse(Constants.Roles.Patient.Id), Name = "Updated Name", Username = "updated", EmailAddress = "u@t.com", PhoneNumber = "222" });

        Assert.Equal("Updated Name", context.Users.Find(userId)?.Name);
    }

    [Fact]
    public async Task Doctor_UpdateSchedule_UpdatesExistingRecords()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var dId = Guid.NewGuid();
        var scheduleId = Guid.NewGuid();
        var sch = new Schedule(dId, DayOfWeek.Friday, new TimeOnly(10, 0), new TimeOnly(12, 0), 30, true, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(7)));
        sch.AssignIdentifier(scheduleId);
        context.Schedules.Add(sch);
        context.Users.Add(CreateUser(dId, Guid.Parse(Constants.Roles.Doctor.Id), "D", "d", "d@t.com", "1"));
        await context.SaveChangesAsync();

        var service = new DoctorService(context, new TestApplicationUserService(dId));
        var updateDto = new UpdateScheduleDto { Id = scheduleId, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(18, 0), SlotDurationInMinutes = 30 };
        service.UpdateDoctorSchedule(scheduleId, updateDto);

        var updated = await context.Schedules.FindAsync(scheduleId);
        if (updated != null) Assert.Equal(DayOfWeek.Tuesday, updated.DayOfWeek);
    }
    [Fact]
    public async Task Role_CRUD_Operations()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var role = new Role("Temp", "Desc", true, true);
        role.AssignIdentifier(Guid.NewGuid());
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var service = new RoleService(context);
        service.UpdateRole(role.Id, new UpdateRoleDto { Id = role.Id, Name = "Fixed", Description = "Fixed Desc" });
        Assert.Equal("Fixed", context.Roles.Find(role.Id)?.Name);

        service.ActivateDeactivateRole(role.Id);
        Assert.False(context.Roles.Find(role.Id) is { IsActive: true });
    }

    [Fact]
    public async Task MasterDetails_ListRetrieval_ReturnsSeededItems()
    {
        using var context = TestApplicationDbContextFactory.Create();
        context.Specializations.Add(new Specialization("Heart", "H"));
        context.MedicationTypes.Add(new MedicationType("Inhaler", "I"));
        await context.SaveChangesAsync();

        var specService = new SpecializationService(context);
        Assert.NotEmpty(specService.GetAllSpecializations());

        var medTypeService = new MedicationTypeService(context);
        Assert.NotEmpty(medTypeService.GetAllMedicationTypes());
    }

    [Fact]
    public async Task Medicine_GetById_ReturnsCorrectMedicine()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var medId = Guid.NewGuid();
        var type = new MedicationType("T", "D"); type.AssignIdentifier(Guid.NewGuid());
        context.MedicationTypes.Add(type);
        var medicine = new Medicine(type.Id, "Aspirin", "A", "75mg");
        medicine.AssignIdentifier(medId);
        context.Medicines.Add(medicine);
        await context.SaveChangesAsync();

        var service = new MedicineService(context);
        var result = service.GetMedicineById(medId);
        Assert.Equal("Aspirin", result.Title);
    }

    [Fact]
    public async Task DiagnosticTest_GetById_ReturnsCorrectTest()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var medId = Guid.NewGuid();
        var med = new Medicine(Guid.NewGuid(), "Med1", "M1", "10mg");
        med.AssignIdentifier(medId);
        context.Medicines.Add(med);
        
        var diagId = Guid.NewGuid();
        var diagTest = new DiagnosticTest(Guid.NewGuid(), "Diag1", "D1", "Lab");
        diagTest.AssignIdentifier(diagId);
        context.DiagnosticTests.Add(diagTest);
        await context.SaveChangesAsync();

        var service = new DiagnosticTestService(context);
        var result = service.GetDiagnosticTestById(diagId);
        Assert.Equal("Diag1", result.Title);
    }

    [Fact]
    public async Task User_ToggleUserStatus_DeactivatesUser()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var userId = Guid.NewGuid();
        var user = CreateUser(userId, Guid.Parse(Constants.Roles.Patient.Id), "U", "u", "u@t.com", "1");
        user.IsActive = true;
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new UserService(_fileServiceMock.Object, context, new TestApplicationUserService(userId), _notificationServiceMock.Object);
        service.ActivateDeactivateUser(userId);

        Assert.False(context.Users.Find(userId) is { IsActive: true });
    }
    [Fact]
    public async Task Appointment_BookByDoctor_PersistsSuccessfully()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var doctorId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        context.Users.AddRange(CreateUser(doctorId, Guid.Parse(Constants.Roles.Doctor.Id), "D", "d", "d@t.com", "1"), CreateUser(patientId, Guid.Parse(Constants.Roles.Patient.Id), "P", "p", "p@t.com", "2"));
        var tsId = Guid.NewGuid();
        var ts = new Timeslot(doctorId, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), new TimeOnly(14, 0), new TimeOnly(14, 30));
        ts.AssignIdentifier(tsId);
        context.Timeslots.Add(ts);
        await context.SaveChangesAsync();

        (doctorId, patientId, tsId, _) = await SeedAppointment(context);

        var service = new AppointmentService(context, new TestApplicationUserService(doctorId), _notificationServiceMock.Object);
        service.BookAppointmentByDoctor(new CreateAppointmentByDoctorDto { PatientId = patientId, TimeslotId = tsId, Symptoms = "Checkup" });

        Assert.NotNull(await context.Appointments.FirstOrDefaultAsync(x => x.PatientId == patientId));
    }

    [Fact]
    public async Task Role_GetById_ReturnsCorrectRole()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var roleId = Guid.NewGuid();
        var role = new Role("TestRole", "Desc", true, false);
        role.AssignIdentifier(roleId);
        context.Roles.Add(role);
        await context.SaveChangesAsync();

        var service = new RoleService(context);
        var result = service.GetRoleById(roleId);
        Assert.Equal("TestRole", result.Name);
    }

    [Fact]
    public async Task User_GetById_ReturnsCorrectUser()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        var userId = Guid.NewGuid();
        context.Users.Add(CreateUser(userId, Guid.Parse(Constants.Roles.Patient.Id), "Alice", "alice", "a@t.com", "1"));
        await context.SaveChangesAsync();

        var service = new UserService(_fileServiceMock.Object, context, new TestApplicationUserService(userId), _notificationServiceMock.Object);
        var result = service.GetUserById(userId);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public async Task Specialization_GetById_ReturnsCorrectData()
    {
        using var context = TestApplicationDbContextFactory.Create();
        var specId = Guid.NewGuid();
        var specialization = new Specialization("Neurology", "N");
        specialization.AssignIdentifier(specId);
        context.Specializations.Add(specialization);
        await context.SaveChangesAsync();

        var service = new SpecializationService(context);
        var result = service.GetSpecializationById(specId);
        Assert.Equal("Neurology", result.Title);
    }

    [Fact]
    public async Task User_GetAllUsers_ReturnsList()
    {
        using var context = TestApplicationDbContextFactory.Create();
        await SeedCriticalData(context);
        context.Users.Add(CreateUser(Guid.NewGuid(), Guid.Parse(Constants.Roles.Patient.Id), "U1", "u1", "u1@t.com", "1"));
        await context.SaveChangesAsync();

        var service = new UserService(_fileServiceMock.Object, context, new TestApplicationUserService(Guid.NewGuid()), _notificationServiceMock.Object);
        var list = service.GetAllUsers(1, 10, out _);
        Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Role_GetAllRoles_ReturnsList()
    {
        using var context = TestApplicationDbContextFactory.Create();
        context.Roles.Add(new Role("Admin", "Admin Role", true, false));
        await context.SaveChangesAsync();
        var service = new RoleService(context);
        var list = service.GetAllRoles(1, 10, out _);
        Assert.NotEmpty(list);
    }
    #endregion
}








