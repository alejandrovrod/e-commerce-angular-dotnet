using System.ComponentModel.DataAnnotations;

namespace ECommerce.BuildingBlocks.Common.Models;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? CreatedBy { get; set; }
    
    public string? UpdatedBy { get; set; }
    
    public bool IsDeleted { get; set; } = false;
    
    public DateTime? DeletedAt { get; set; }
    
    public string? DeletedBy { get; set; }
}

public abstract class BaseAuditableEntity : BaseEntity
{
    public string Version { get; set; } = "1.0";
    
    public Dictionary<string, object>? Metadata { get; set; }
}





