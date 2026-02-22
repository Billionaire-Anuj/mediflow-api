namespace Mediflow.Application.DTOs.Doctors;

public class DoctorDirectoryRecordDto
{
    public string Category { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal Rating { get; set; }
}