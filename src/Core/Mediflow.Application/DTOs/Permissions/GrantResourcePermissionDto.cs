namespace Mediflow.Application.DTOs.Permissions;

public class GrantResourcePermissionDto
{
    public Guid ResourceId { get; set; }
    
    public List<string> Actions { get; set; } = new();
}