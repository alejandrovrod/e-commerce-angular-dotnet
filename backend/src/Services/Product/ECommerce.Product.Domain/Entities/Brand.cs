using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Domain.Entities;

public class Brand : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string LogoUrl { get; private set; } = string.Empty;
    public string Website { get; private set; } = string.Empty;
    public string? Country { get; private set; }
    public int? FoundedYear { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public List<Product> Products { get; private set; } = new();
    
    // Private constructor for EF Core
    private Brand() { }
    
    public Brand(string name, string description, string logoUrl = "", string website = "", string? country = null, int? foundedYear = null)
    {
        Name = name;
        Description = description;
        LogoUrl = logoUrl;
        Website = website;
        Country = country;
        FoundedYear = foundedYear;
        IsActive = true;
    }
    
    public void Update(string name, string description, string logoUrl, string website, string? country = null, int? foundedYear = null)
    {
        Name = name;
        Description = description;
        LogoUrl = logoUrl;
        Website = website;
        Country = country;
        FoundedYear = foundedYear;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetLogo(string logoUrl)
    {
        LogoUrl = logoUrl;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetWebsite(string website)
    {
        Website = website;
        UpdatedAt = DateTime.UtcNow;
    }
}

