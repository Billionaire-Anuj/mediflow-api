using Mediflow.Domain.Common.Enum;
using Mediflow.Application.DTOs.Users;
using Mediflow.Application.Common.Response;

namespace Mediflow.Application.DTOs.Appointments.Medications;

public class AppointmentMedicationsDto : BaseDto
{
    public UserDto? Pharmacist { get; set; }

    public string Notes { get; set; } = string.Empty;

    public DiagnosticStatus Status { get; set; }

    public DateTime? CompletedDate { get; set; }

    public List<AppointmentMedicationDrugsDto> Drugs { get; set; } = new();
}