using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Common.Enum;
using Mediflow.Domain.Entities.Audits;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mediflow.Domain.Entities;

public class User(
    Guid roleId,
    Gender gender,
    string name, 
    string username,
    string emailAddress,
    string? address,
    Asset? profileImage,
    string passwordHash,
    string phoneNumber,
    bool is2FactorAuthenticationEnabled = false
) : BaseEntity<Guid>
{
    [ForeignKey(nameof(Role))]
    public Guid RoleId { get; private set; } = roleId;

    public Gender Gender { get; private set; } = gender;

    public string Name { get; private set; } = name;

    public string Username { get; private set; } = username;

    public string EmailAddress { get; private set; } = emailAddress;

    public string? Address { get; private set; } = address;

    public Asset? ProfileImage { get; private set; } = profileImage;

    public string PasswordHash { get; private set; } = passwordHash;

    public string PhoneNumber { get; private set; } = phoneNumber;

    public bool Is2FactorAuthenticationEnabled { get; private set; } = is2FactorAuthenticationEnabled;

    public virtual Role? Role { get; set; }

    #region Generic Navigation Properties
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<UserLoginLog> UserLoginLogs { get; set; } = new List<UserLoginLog>();
    #endregion

    #region Patient Specific Navigation Properties
    public virtual PatientCredit? Credit { get; set; }

    public virtual ICollection<Appointment> PatientAppointments { get; set; } = new List<Appointment>();
    #endregion

    #region Pharmacits & Lab Technician Specific Navigation Properties
    public virtual ICollection<AppointmentDiagnostics> AppointmentDiagnostics { get; set; } = new List<AppointmentDiagnostics>();

    public virtual ICollection<AppointmentMedications> AppointmentMedications { get; set; } = new List<AppointmentMedications>();
    #endregion

    #region Doctor Specific Navigation Properties
    public virtual DoctorInformation? DoctorInformation { get; set; }

    public virtual ICollection<Appointment>? DoctorAppointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Schedule>? Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<DoctorSpecialization>? DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
    #endregion

    public static User Default => new(Guid.Empty, Gender.Male, string.Empty, string.Empty, string.Empty, null, null, string.Empty, string.Empty);

    public void Update(Guid roleId, Gender gender, string name, string username, string emailAddress, string? address, string phoneNumber)
    {
        RoleId = roleId;
        Gender = gender;
        Name = name;
        Username = username;
        EmailAddress = emailAddress;
        Address = address;
        PhoneNumber = phoneNumber;
    }

    public void Enable2FactorAuthentication()
    {
        Is2FactorAuthenticationEnabled = true;
    }

    public void Disable2FactorAuthentication()
    {
        Is2FactorAuthenticationEnabled = false;
    }

    public void UpdateProfileImage(Asset profileImage)
    {
        ProfileImage = profileImage;
    }
    
    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }
}