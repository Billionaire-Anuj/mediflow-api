using Mediflow.Domain.Common.Base;
using Mediflow.Domain.Entities.Audits;
using System.ComponentModel.DataAnnotations.Schema;
using Mediflow.Domain.Common.Enum;

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

    public virtual ICollection<AuditLog>? AuditLogs { get; set; }

    public virtual ICollection<UserLoginLog>? UserLoginLogs { get; set; }

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