using System.ComponentModel.DataAnnotations;

namespace OperationIntelligence.DB;

public class Permission : AuditableEntity
{
    [Required]
    [MaxLength(150)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
