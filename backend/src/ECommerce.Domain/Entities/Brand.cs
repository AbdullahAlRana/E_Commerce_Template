using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public bool IsActive { get; set; } = true;
}