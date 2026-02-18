namespace Mediflow.Application.DTOs.Permissions;

public class PermissionDto
{
    public string Action { get; set; } = string.Empty;
    
    public bool IsAssigned { get; set; }
}