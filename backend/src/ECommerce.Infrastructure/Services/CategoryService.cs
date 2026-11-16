using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Categories;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IProductRepository _products;

    public CategoryService(ICategoryRepository repo, IUnitOfWork uow, IProductRepository products)
    { _repo = repo; _uow = uow; _products = products; }

    public async Task<PagedResult<CategoryDto>> GetAllAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var q = _repo.Query();
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(c => c.Name.Contains(search));
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(c => c.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var dtos = new List<CategoryDto>();
        foreach (var c in items)
        {
            var count = await _products.Query().CountAsync(p => p.CategoryId == c.Id, ct);
            dtos.Add(new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description, ImageUrl = c.ImageUrl, ParentCategoryId = c.ParentCategoryId, ParentCategoryName = c.ParentCategory?.Name, IsActive = c.IsActive, ProductCount = count });
        }
        return new PagedResult<CategoryDto> { Items = dtos, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct);
        if (c == null) return null;
        var count = await _products.Query().CountAsync(p => p.CategoryId == c.Id, ct);
        return new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description, ImageUrl = c.ImageUrl, ParentCategoryId = c.ParentCategoryId, IsActive = c.IsActive, ProductCount = count };
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var c = new Category { Name = request.Name, Description = request.Description, ImageUrl = request.ImageUrl, ParentCategoryId = request.ParentCategoryId, IsActive = request.IsActive };
        await _repo.AddAsync(c, ct);
        await _uow.SaveChangesAsync(ct);
        return (await GetByIdAsync(c.Id, ct))!;
    }

    public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct);
        if (c == null) return null;
        c.Name = request.Name; c.Description = request.Description; c.ImageUrl = request.ImageUrl; c.ParentCategoryId = request.ParentCategoryId; c.IsActive = request.IsActive; c.UpdatedAt = DateTime.UtcNow;
        _repo.Update(c);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct);
        if (c == null) return false;
        _repo.Remove(c);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
