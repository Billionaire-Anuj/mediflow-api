namespace Mediflow.Application.DTOs.Permissions;

public class GrantPermissionsDto
{
    public Guid RoleId { get; set; } 
    
    public List<GrantResourcePermissionDto> Permissions { get; set; } = new();
}