using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Brands;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repo;
    private readonly IUnitOfWork _uow;

    public BrandService(IBrandRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<PagedResult<BrandDto>> GetAllAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var q = _repo.Query();
        if (!string.IsNullOrWhiteSpace(search)) q = q.Where(b => b.Name.Contains(search));
        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(b => b.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<BrandDto>
        {
            Items = items.Select(b => new BrandDto { Id = b.Id, Name = b.Name, Description = b.Description, LogoUrl = b.LogoUrl, IsActive = b.IsActive }).ToList(),
            Page = page, PageSize = pageSize, TotalCount = total
        };
    }

    public async Task<BrandDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var b = await _repo.GetByIdAsync(id, ct);
        if (b == null) return null;
        return new BrandDto { Id = b.Id, Name = b.Name, Description = b.Description, LogoUrl = b.LogoUrl, IsActive = b.IsActive };
    }

    public async Task<BrandDto> CreateAsync(CreateBrandRequest request, CancellationToken ct = default)
    {
        var b = new Brand { Name = request.Name, Description = request.Description, LogoUrl = request.LogoUrl, IsActive = request.IsActive };
        await _repo.AddAsync(b, ct);
        await _uow.SaveChangesAsync(ct);
        return (await GetByIdAsync(b.Id, ct))!;
    }

    public async Task<BrandDto?> UpdateAsync(Guid id, UpdateBrandRequest request, CancellationToken ct = default)
    {
        var b = await _repo.GetByIdAsync(id, ct);
        if (b == null) return null;
        b.Name = request.Name; b.Description = request.Description; b.LogoUrl = request.LogoUrl; b.IsActive = request.IsActive; b.UpdatedAt = DateTime.UtcNow;
        _repo.Update(b);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var b = await _repo.GetByIdAsync(id, ct);
        if (b == null) return false;
        _repo.Remove(b);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
