namespace Mediflow.Application.DTOs.Recommendations;

public class DoctorDirectoryDto
{
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal Rating { get; set; }
}
