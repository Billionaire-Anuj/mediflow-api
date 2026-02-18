using Mediflow.Domain.Common.Enum;
using Mediflow.Domain.Entities.Audits;
using Mediflow.Application.DTOs.Users;

namespace Mediflow.Application.DTOs.AuditLogs;

public static class AuditLogHistoryExtensionMethods
{
    public static AuditLogDto ToAuditLogDto(this AuditLog auditLog)
    {
        return new AuditLogDto
        {
            Id = auditLog.Id,
            IsActive = auditLog.IsActive,
            ChangeType = auditLog.ChangeType,
            Remarks = auditLog.Remarks,
            IsAutomation = auditLog.IsAutomation,
            AuditedDate = auditLog.AuditedDate,
            User = auditLog.Auditor?.ToUserDto(),
            AuditLogHistories = auditLog.AuditLogHistories?.Select(x => x.ToAuditLogHistoryDto()).ToList() ?? []
        };
    }

    private static AuditLogHistoryDto ToAuditLogHistoryDto(this AuditLogHistory entityChangeLogDetails)
    {
        return new AuditLogHistoryDto
        {
            Id = entityChangeLogDetails.Id,
            IsActive = entityChangeLogDetails.IsActive,
            FieldName = entityChangeLogDetails.FieldName,
            FieldDataType = entityChangeLogDetails.FieldDataType,
            OldValue = ParseValue(entityChangeLogDetails.OldValue, entityChangeLogDetails.FieldDataType, entityChangeLogDetails.FieldName),
            NewValue = ParseValue(entityChangeLogDetails.NewValue, entityChangeLogDetails.FieldDataType, entityChangeLogDetails.FieldName),
            Remarks = entityChangeLogDetails.Remarks
        };
    }

    private static object? ParseValue(string? value, FieldDataType fieldDataType, string propertyName)
    {
        if (value is null) return value;

        return fieldDataType switch
        {
            FieldDataType.STRING => value,
            FieldDataType.DECIMAL => decimal.TryParse(value, out var @decimal)
                ? @decimal
                : throw new FormatException($"Invalid Decimal Value: {value}."),
            FieldDataType.INTEGER => int.TryParse(value, out var integer)
                ? integer
                : throw new FormatException($"Invalid Integer Value: {value}."),
            FieldDataType.BOOLEAN => bool.TryParse(value, out var boolean)
                ? boolean
                : throw new FormatException($"Invalid Boolean Value: {value}."),
            FieldDataType.DATE_ONLY => DateOnly.TryParse(value, out var dateOnly)
                ? dateOnly
                : throw new FormatException($"Invalid Date Only Value: {value}."),
            FieldDataType.DATE_TIME => DateTime.TryParse(value, out var dateTime)
                ? dateTime
                : throw new FormatException($"Invalid Date Time Value: {value}."),
            FieldDataType.GUID => GetNameIfRequired(propertyName, value) 
                                  ?? throw new FormatException($"Invalid GUID Value: {value}"),
            FieldDataType.OTHER => value,
            _ => throw new NotSupportedException($"Unsupported Data Type: {fieldDataType}")
        };
    }

    private static string? GetNameIfRequired(string propertyName, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        Console.WriteLine($"Property Name of {propertyName} and Value Assigned with {value}");
        return value;
    }
}