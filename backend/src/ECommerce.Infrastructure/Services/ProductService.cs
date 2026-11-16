using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly ICategoryRepository _categories;
    private readonly IBrandRepository _brands;
    private readonly IUnitOfWork _uow;

    public ProductService(IProductRepository repo, ICategoryRepository categories, IBrandRepository brands, IUnitOfWork uow)
    { _repo = repo; _categories = categories; _brands = brands; _uow = uow; }

    private static ProductDto Map(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        SKU = p.SKU,
        Price = p.Price,
        DiscountPrice = p.DiscountPrice,
        StockQuantity = p.StockQuantity,
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        BrandId = p.BrandId,
        BrandName = p.Brand?.Name,
        IsActive = p.IsActive,
        IsFeatured = p.IsFeatured,
        AverageRating = p.AverageRating,
        ReviewCount = p.ReviewCount,
        Images = p.Images.Select(i => new ProductImageDto { Id = i.Id, Url = i.Url, IsPrimary = i.IsPrimary, SortOrder = i.SortOrder }).ToList(),
        PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary)?.Url ?? p.Images.FirstOrDefault()?.Url,
        CreatedAt = p.CreatedAt
    };

    public async Task<PagedResult<ProductDto>> GetAllAsync(int page, int pageSize, string? search, Guid? categoryId, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? featured, string? sort, CancellationToken ct = default)
    {
        var (items, total) = await _repo.SearchAsync(search, categoryId, brandId, minPrice, maxPrice, featured, page, pageSize, sort, ct);
        return new PagedResult<ProductDto> { Items = items.Select(Map).ToList(), Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _repo.Query().Include(p => p.Category).Include(p => p.Brand).Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (p == null) return null;
        return Map(p);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var p = new Product
        {
            Name = request.Name,
            Description = request.Description,
            SKU = request.SKU,
            Price = request.Price,
            DiscountPrice = request.DiscountPrice,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            IsActive = request.IsActive,
            IsFeatured = request.IsFeatured
        };
        int i = 0;
        foreach (var url in request.ImageUrls)
        {
            p.Images.Add(new ProductImage { Url = url, IsPrimary = i == 0, SortOrder = i++ });
        }
        await _repo.AddAsync(p, ct);
        await _uow.SaveChangesAsync(ct);
        return (await GetByIdAsync(p.Id, ct))!;
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var p = await _repo.GetByIdAsync(id, ct);
        if (p == null) return null;
        // reload with images
        var tracked = await _repo.Query().Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (tracked == null) return null;
        tracked.Name = request.Name;
        tracked.Description = request.Description;
        tracked.SKU = request.SKU;
        tracked.Price = request.Price;
        tracked.DiscountPrice = request.DiscountPrice;
        tracked.StockQuantity = request.StockQuantity;
        tracked.CategoryId = request.CategoryId;
        tracked.BrandId = request.BrandId;
        tracked.IsActive = request.IsActive;
        tracked.IsFeatured = request.IsFeatured;
        tracked.UpdatedAt = DateTime.UtcNow;
        // replace images
        tracked.Images.Clear();
        int i = 0;
        foreach (var url in request.ImageUrls)
            tracked.Images.Add(new ProductImage { Url = url, IsPrimary = i == 0, SortOrder = i++, ProductId = tracked.Id });
        _repo.Update(tracked);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var p = await _repo.GetByIdAsync(id, ct);
        if (p == null) return false;
        _repo.Remove(p);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
