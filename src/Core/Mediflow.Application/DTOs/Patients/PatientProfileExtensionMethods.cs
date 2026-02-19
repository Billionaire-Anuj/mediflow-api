using Mediflow.Domain.Entities;
using Mediflow.Application.DTOs.Roles;
using Mediflow.Application.DTOs.Assets;

namespace Mediflow.Application.DTOs.Patients;

public static class PatientProfileExtensionMethods
{
    public static PatientProfileDto ToPatientProfileDto(this User patient)
    {
        var patientCredit = patient.Credit ?? PatientCredit.Default;

        return new PatientProfileDto
        {
            Id = patient.Id,
            Name = patient.Name,
            Gender = patient.Gender,
            Address = patient.Address,
            IsActive = patient.IsActive,
            Username = patient.Username,
            PhoneNumber = patient.PhoneNumber,
            EmailAddress = patient.EmailAddress,
            CreditPoints = patientCredit.CreditPoints,
            PaymentIndex = patientCredit.PaymentIndex,
            Role = (patient.Role ?? Role.Default).ToRoleDto(),
            ProfileImage = patient.ProfileImage?.ToAssetDto(),
            Is2FactorAuthenticationEnabled = patient.Is2FactorAuthenticationEnabled,
        };
    }
}