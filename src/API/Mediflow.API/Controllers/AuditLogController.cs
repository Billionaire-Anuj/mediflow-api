using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.AuditLogs;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class AuditLogController(IAuditLogService auditLogService) : BaseController<AuditLogController>
{
    [HttpGet("{entityId:guid}")]
    [Documentation("GetAuditLogsByEntityId", "Returns the audited logs for a respective entity.")]
    public ResponseDto<List<AuditLogDto>> GetAuditLogsByEntityId([FromRoute] Guid entityId)
    {
        var result = auditLogService.GetAuditLogsByEntityId(entityId);

        return new ResponseDto<List<AuditLogDto>>(
            (int)HttpStatusCode.OK,
            "Audit logs successfully fetched.", 
            result);
    }
}