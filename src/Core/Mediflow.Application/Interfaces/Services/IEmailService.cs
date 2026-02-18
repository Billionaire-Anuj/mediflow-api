using Mediflow.Application.DTOs.Emails;
using Mediflow.Application.Common.Service;

namespace Mediflow.Application.Interfaces.Services;

public interface IEmailService : ITransientService
{
    Task SendEmailAsync(EmailDto email);
}
