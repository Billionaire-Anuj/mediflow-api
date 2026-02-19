using Mediflow.Application.DTOs.AuditLogs;
using Mediflow.Application.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.Infrastructure.Implementation.Services;

public class AuditLogService(IApplicationDbContext applicationDbContext) : IAuditLogService
{
    // TODO: Abstraction for Tenant Administrator Logs
    public List<AuditLogDto> GetAuditLogsByEntityId(Guid entityId)
    {
        var auditLogModels = applicationDbContext.AuditLogs
            .Where(x => x.EntityId == entityId.ToString())
            .AsNoTracking()
            .AsQueryable();

        return auditLogModels.Select(x => x.ToAuditLogDto()).ToList();
    }
}