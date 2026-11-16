using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;
}