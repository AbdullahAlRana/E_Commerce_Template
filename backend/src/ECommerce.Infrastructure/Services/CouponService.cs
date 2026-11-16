using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Coupons;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class CouponService : ICouponService
{
    private readonly ICouponRepository _repo;
    private readonly IUnitOfWork _uow;

    public CouponService(ICouponRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }

    public async Task<PagedResult<CouponDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var q = _repo.Query();
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(c => c.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<CouponDto> { Items = items.Select(Map).ToList(), Page = page, PageSize = pageSize, TotalCount = total };
    }

    private static CouponDto Map(Coupon c) => new() { Id = c.Id, Code = c.Code, Type = c.Type, Value = c.Value, MinOrderAmount = c.MinOrderAmount, MaxDiscount = c.MaxDiscount, StartDate = c.StartDate, EndDate = c.EndDate, UsageLimit = c.UsageLimit, UsedCount = c.UsedCount, IsActive = c.IsActive };

    public async Task<CouponDto> CreateAsync(CreateCouponRequest request, CancellationToken ct = default)
    {
        var c = new Coupon { Code = request.Code.ToUpper(), Type = request.Type, Value = request.Value, MinOrderAmount = request.MinOrderAmount, MaxDiscount = request.MaxDiscount, StartDate = request.StartDate, EndDate = request.EndDate, UsageLimit = request.UsageLimit, IsActive = request.IsActive };
        await _repo.AddAsync(c, ct);
        await _uow.SaveChangesAsync(ct);
        return Map(c);
    }

    public async Task<CouponDto?> UpdateAsync(Guid id, UpdateCouponRequest request, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct);
        if (c == null) return null;
        c.Code = request.Code.ToUpper(); c.Type = request.Type; c.Value = request.Value; c.MinOrderAmount = request.MinOrderAmount; c.MaxDiscount = request.MaxDiscount; c.StartDate = request.StartDate; c.EndDate = request.EndDate; c.UsageLimit = request.UsageLimit; c.IsActive = request.IsActive; c.UpdatedAt = DateTime.UtcNow;
        _repo.Update(c);
        await _uow.SaveChangesAsync(ct);
        return Map(c);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var c = await _repo.GetByIdAsync(id, ct);
        if (c == null) return false;
        _repo.Remove(c);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<(bool Valid, string? Message, decimal Discount)> ValidateAsync(ValidateCouponRequest request, CancellationToken ct = default)
    {
        var coupon = await _repo.GetByCodeAsync(request.Code, ct);
        if (coupon == null) return (false, "Coupon not found.", 0);
        if (!coupon.IsActive) return (false, "Coupon inactive.", 0);
        if (coupon.StartDate > DateTime.UtcNow) return (false, "Coupon not yet active.", 0);
        if (coupon.EndDate < DateTime.UtcNow) return (false, "Coupon expired.", 0);
        if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value) return (false, "Usage limit reached.", 0);
        if (coupon.MinOrderAmount.HasValue && request.OrderAmount < coupon.MinOrderAmount.Value) return (false, "Minimum order amount not met.", 0);
        decimal discount = coupon.Type == CouponType.Percentage ? request.OrderAmount * (coupon.Value / 100m) : coupon.Value;
        if (coupon.MaxDiscount.HasValue) discount = Math.Min(discount, coupon.MaxDiscount.Value);
        return (true, "Valid.", discount);
    }
}
