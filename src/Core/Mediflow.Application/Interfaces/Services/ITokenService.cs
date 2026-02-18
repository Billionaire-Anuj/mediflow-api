using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface ITokenService : ITransientService
{
    Task<bool> IsCurrentActiveTokenAsync();
    
    Task DeactivateCurrentAsync();
    
    Task<bool> IsActiveAsync(string token);
    
    Task DeactivateAsync(string token);
}