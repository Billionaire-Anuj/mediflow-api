using Mediflow.Domain.Common;
using Mediflow.API.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mediflow.Domain.Common.Enum;
using Mediflow.API.Controllers.Base;
using Mediflow.Application.DTOs.Emails;
using Mediflow.Application.Common.Response;
using Mediflow.Application.Interfaces.Services;

namespace Mediflow.API.Controllers;

public class EmailOutboxController(IEmailOutboxService emailOutboxService) : BaseController<EmailOutboxController>
{
    [HttpGet]
    [Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("GetAllEmailOutboxes", "Returns paginated email outbox details with search and active filters.")]
    public CollectionDto<EmailOutboxDto> GetAllEmailOutboxes(
        [FromQuery] PaginationQueryDto paginationQuery,
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? toEmail = null,
        [FromQuery] string? name = null,
        [FromQuery] string? subject = null,
        [FromQuery] List<EmailProcess>? emailProcess = null,
        [FromQuery] int? minimumAttemptCount = null,
        [FromQuery] int? maximumAttemptCount = null,
        [FromQuery] DateTime? minimumNextAttemptDate = null,
        [FromQuery] DateTime? maximumNextAttemptDate = null,
        [FromQuery] List<OutboxStatus>? outboxStatuses = null,
        [FromQuery] DateTime? minimumScheduledDate = null,
        [FromQuery] DateTime? maximumScheduledDate = null,
        [FromQuery] DateTime? minimumSentDate = null,
        [FromQuery] DateTime? maximumSentDate = null)
    {
        var result = emailOutboxService.GetAllEmailOutboxes(
            paginationQuery.PageNumber,
            paginationQuery.PageSize,
            out var rowCount,
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            toEmail,
            name,
            subject,
            emailProcess,
            minimumAttemptCount,
            maximumAttemptCount,
            minimumNextAttemptDate,
            maximumNextAttemptDate,
            outboxStatuses,
            minimumScheduledDate,
            maximumScheduledDate,
            minimumSentDate,
            maximumSentDate);

        return new CollectionDto<EmailOutboxDto>(
            (int)HttpStatusCode.OK,
            "Email outboxes successfully fetched.",
            result,
            rowCount,
            paginationQuery.PageNumber,
            paginationQuery.PageSize);
    }

    [HttpGet("list")]
    [Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("GetAllEmailOutboxesList", "Returns paginated email outbox details with search and active filters.")]
    public ResponseDto<List<EmailOutboxDto>> GetAllEmailOutboxes(
        [FromQuery] SearchAndActiveFlagQueryDto searchAndActiveFlagQuery,
        [FromQuery] OrderQueryDto orderQueryDto,
        [FromQuery] string? toEmail = null,
        [FromQuery] string? name = null,
        [FromQuery] string? subject = null,
        [FromQuery] List<EmailProcess>? emailProcess = null,
        [FromQuery] int? minimumAttemptCount = null,
        [FromQuery] int? maximumAttemptCount = null,
        [FromQuery] DateTime? minimumNextAttemptDate = null,
        [FromQuery] DateTime? maximumNextAttemptDate = null,
        [FromQuery] List<OutboxStatus>? outboxStatuses = null,
        [FromQuery] DateTime? minimumScheduledDate = null,
        [FromQuery] DateTime? maximumScheduledDate = null,
        [FromQuery] DateTime? minimumSentDate = null,
        [FromQuery] DateTime? maximumSentDate = null)
    {
        var result = emailOutboxService.GetAllEmailOutboxes(
            searchAndActiveFlagQuery.GlobalSearch,
            searchAndActiveFlagQuery.IsActive,
            orderQueryDto.OrderBys,
            toEmail,
            name,
            subject,
            emailProcess,
            minimumAttemptCount,
            maximumAttemptCount,
            minimumNextAttemptDate,
            maximumNextAttemptDate,
            outboxStatuses,
            minimumScheduledDate,
            maximumScheduledDate,
            minimumSentDate,
            maximumSentDate);

        return new ResponseDto<List<EmailOutboxDto>>(
            (int)HttpStatusCode.OK,
            "Email outboxes successfully fetched.",
            result);
    }

    [HttpGet("{emailOutboxId:guid}")]
    [Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("GetEmailOutboxById", "Returns email outbox details via its identifier.")]
    public ResponseDto<EmailOutboxDto> GetEmailOutboxById([FromRoute] Guid emailOutboxId)
    {
        var result = emailOutboxService.GetEmailOutboxById(emailOutboxId);

        return new ResponseDto<EmailOutboxDto>(
            (int)HttpStatusCode.OK,
            "Email outbox successfully fetched.",
            result);
    }

    [HttpPut("{emailOutboxId:guid}/process")]
    [Authorize(Constants.Roles.SuperAdmin.Name, Constants.Roles.TenantAdministrator.Name)]
    [Documentation("ProcessEmailOutboxAsync", "Processes a respective email outbox via its identifier.")]
    public async Task<ResponseDto<bool>> ProcessEmailOutboxAsync([FromRoute] Guid emailOutboxId)
    {
        await emailOutboxService.ProcessEmailOutboxAsync(emailOutboxId);

        return new ResponseDto<bool>(
            (int)HttpStatusCode.OK,
            "Email outbox successfully processed.",
            true);
    }
}