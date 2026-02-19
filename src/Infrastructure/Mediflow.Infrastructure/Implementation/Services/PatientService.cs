using Mediflow.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Exceptions;
using Mediflow.Application.Common.User;
using Mediflow.Application.DTOs.Patients;
using Mediflow.Application.Interfaces.Data;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class PatientService(
    IApplicationDbContext applicationDbContext,
    IApplicationUserService applicationUserService) : IPatientService
{
    public PatientProfileDto GetPatientProfile()
    {
        var userId = applicationUserService.GetUserId;

        var patient = applicationDbContext.Users
                          .AsNoTracking()
                          .Include(x => x.Role)
                          .Include(x => x.Credit)
                          .FirstOrDefault(x => x.Id == userId)
                      ?? throw new NotFoundException($"Patient with the identifier of {userId} could not be found.");

        return patient.Role?.Id.ToString() != Constants.Roles.Patient.Id
            ? throw new BadRequestException("Only users with the patient role can access their patient profile.")
            : patient.ToPatientProfileDto();
    }

    public PatientProfileDto GetPatientProfile(Guid patientId)
    {
        var patient = applicationDbContext.Users
                          .AsNoTracking()
                          .Include(x => x.Role)
                          .Include(x => x.Credit)
                          .FirstOrDefault(x => x.Id == patientId)
                      ?? throw new NotFoundException($"Patient with the identifier of {patientId} could not be found.");

        return patient.Role?.Id.ToString() != Constants.Roles.Patient.Id ? throw new BadRequestException("The selected user is not a patient.") : patient.ToPatientProfileDto();
    }
}
