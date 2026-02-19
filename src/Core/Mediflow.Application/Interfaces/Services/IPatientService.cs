using Mediflow.Application.DTOs.Patients;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IPatientService : ITransientService
{
    PatientProfileDto GetPatientProfile();

    PatientProfileDto GetPatientProfile(Guid patientId);
}
