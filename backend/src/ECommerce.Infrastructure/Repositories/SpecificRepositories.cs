using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext db) : base(db) { }
    public async Task<Category?> GetByNameAsync(string name, CancellationToken ct = default)
        => await _db.Categories.FirstOrDefaultAsync(c => c.Name == name && !c.IsDeleted, ct);
}

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
    public BrandRepository(AppDbContext db) : base(db) { }
}

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext db) : base(db) { }

    public async Task<(IReadOnlyList<Product> Items, int Total)> SearchAsync(string? search, Guid? categoryId, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? featured, int page, int pageSize, string? sort, CancellationToken ct = default)
    {
        var q = _db.Products.Include(p => p.Category).Include(p => p.Brand).Include(p => p.Images).Where(p => !p.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            q = q.Where(p => p.Name.ToLower().Contains(s) || (p.Description != null && p.Description.ToLower().Contains(s)) || p.SKU.ToLower().Contains(s));
        }
        if (categoryId.HasValue) q = q.Where(p => p.CategoryId == categoryId.Value);
        if (brandId.HasValue) q = q.Where(p => p.BrandId == brandId.Value);
        if (minPrice.HasValue) q = q.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) q = q.Where(p => p.Price <= maxPrice.Value);
        if (featured.HasValue) q = q.Where(p => p.IsFeatured == featured.Value);
        var total = await q.CountAsync(ct);
        q = sort switch
        {
            "price_asc" => q.OrderBy(p => p.Price),
            "price_desc" => q.OrderByDescending(p => p.Price),
            "name" => q.OrderBy(p => p.Name),
            "newest" => q.OrderByDescending(p => p.CreatedAt),
            _ => q.OrderByDescending(p => p.CreatedAt)
        };
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext db) : base(db) { }
    public async Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => await _db.Carts.Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Images).FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted, ct);
}

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext db) : base(db) { }

    public async Task<(IReadOnlyList<Order> Items, int Total)> GetByUserAsync(string userId, int page, int pageSize, CancellationToken ct = default)
    {
        var q = _db.Orders.Include(o => o.Items).Include(o => o.ShippingAddress).Where(o => o.UserId == userId && !o.IsDeleted);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task<(IReadOnlyList<Order> Items, int Total)> GetAllAsync(int page, int pageSize, Order? filter = null, CancellationToken ct = default)
    {
        var q = _db.Orders.Include(o => o.Items).Include(o => o.ShippingAddress).Where(o => !o.IsDeleted);
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(o => o.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}

public class AddressRepository : GenericRepository<Address>, IAddressRepository
{
    public AddressRepository(AppDbContext db) : base(db) { }
    public async Task<IReadOnlyList<Address>> GetByUserAsync(string userId, CancellationToken ct = default)
        => await _db.Addresses.Where(a => a.UserId == userId && !a.IsDeleted).ToListAsync(ct);
}

public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(AppDbContext db) : base(db) { }
    public async Task<IReadOnlyList<Review>> GetByProductAsync(Guid productId, CancellationToken ct = default)
        => await _db.Reviews.Where(r => r.ProductId == productId && !r.IsDeleted).OrderByDescending(r => r.CreatedAt).ToListAsync(ct);
}

public class WishlistRepository : GenericRepository<Wishlist>, IWishlistRepository
{
    public WishlistRepository(AppDbContext db) : base(db) { }
    public async Task<Wishlist?> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => await _db.Wishlists.Include(w => w.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(w => w.UserId == userId && !w.IsDeleted, ct);
}

public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
{
    public CouponRepository(AppDbContext db) : base(db) { }
    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted, ct);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public UnitOfWork(AppDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}