using Mediflow.Application.Common.Service;
using Mediflow.Application.DTOs.AuditLogs;

namespace Mediflow.Application.Interfaces.Services;

public interface IAuditLogService : ITransientService
{
    // TODO: Addition of Filters and Pagination Records.
    List<AuditLogDto> GetAuditLogsByEntityId(Guid entityId);
}