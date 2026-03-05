using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.Payments;

namespace Mediflow.Application.Interfaces.Services;

public interface IPatientCreditService : ITransientService
{
    Task<KhaltiInitiateResponseDto> InitiateKhaltiTopupAsync(CreditTopupRequestDto request);

    Task<bool> ConfirmKhaltiTopupAsync(KhaltiConfirmDto request);

    Task<EsewaInitiateResponseDto> InitiateEsewaTopupAsync(CreditTopupRequestDto request);

    Task<bool> ConfirmEsewaTopupAsync(EsewaConfirmDto request);
}
