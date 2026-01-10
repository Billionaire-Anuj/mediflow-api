namespace mediflow.Contracts;

public sealed record DoctorListItemDto(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string? Phone,
    string Specialty,
    string? LicenseNumber,
    decimal? ConsultationFee,
    string? ProfilePictureUrl
);

public sealed record DoctorDetailDto(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string? Phone,
    string Specialty,
    string? LicenseNumber,
    decimal? ConsultationFee,
    string? Bio,
    int DefaultSlotMinutes,
    string? ProfilePictureUrl
);

public sealed record CreateDoctorRequest(
    string FullName,
    string Email,
    string Password,
    string Specialty,
    string? LicenseNumber,
    decimal? ConsultationFee,
    int? DefaultSlotMinutes,
    string? Bio,
    string? Phone
);

public sealed record UpdateDoctorRequest(
    string? Specialty,
    string? LicenseNumber,
    decimal? ConsultationFee,
    int? DefaultSlotMinutes,
    string? Bio,
    string? Phone,
    string? FullName,
    string? ProfilePictureUrl
);