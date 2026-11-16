using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlists;
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _db;

    public WishlistService(IWishlistRepository wishlists, IProductRepository products, IUnitOfWork uow, AppDbContext db)
    { _wishlists = wishlists; _products = products; _uow = uow; _db = db; }

    private async Task<Wishlist> GetOrCreateAsync(string userId, CancellationToken ct)
    {
        var w = await _wishlists.GetByUserIdAsync(userId, ct);
        if (w == null)
        {
            w = new Wishlist { UserId = userId };
            await _wishlists.AddAsync(w, ct);
            await _uow.SaveChangesAsync(ct);
            w = await _wishlists.GetByUserIdAsync(userId, ct) ?? w;
        }
        return w;
    }

    public async Task<IReadOnlyList<Guid>> GetProductIdsAsync(string userId, CancellationToken ct = default)
    {
        var w = await GetOrCreateAsync(userId, ct);
        return w.Items.Select(i => i.ProductId).ToList();
    }

    public async Task AddAsync(string userId, Guid productId, CancellationToken ct = default)
    {
        var w = await GetOrCreateAsync(userId, ct);
        if (!w.Items.Any(i => i.ProductId == productId))
        {
            var item = new WishlistItem { WishlistId = w.Id, ProductId = productId };
            w.Items.Add(item);
            _db.Entry(item).State = EntityState.Added;
            await _uow.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveAsync(string userId, Guid productId, CancellationToken ct = default)
    {
        var w = await _wishlists.GetByUserIdAsync(userId, ct);
        if (w == null) return;
        var item = w.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null) { w.Items.Remove(item); await _uow.SaveChangesAsync(ct); }
    }
}