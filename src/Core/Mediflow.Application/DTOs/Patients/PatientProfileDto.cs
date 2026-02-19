using Mediflow.Application.DTOs.Users;

namespace Mediflow.Application.DTOs.Patients;

public class PatientProfileDto : UserDto
{
    public decimal CreditPoints { get; set; }

    public string PaymentIndex { get; set; } = string.Empty;
}