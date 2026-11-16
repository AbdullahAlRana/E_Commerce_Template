namespace ECommerce.Application.DTOs.Products;

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal EffectivePrice => DiscountPrice ?? Price;
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public string? PrimaryImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public List<string> ImageUrls { get; set; } = new();
}

public class UpdateProductRequest : CreateProductRequest { }