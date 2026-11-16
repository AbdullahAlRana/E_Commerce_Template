using ECommerce.Application.Services.Interfaces;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<object> GetStatsAsync(CancellationToken ct = default)
    {
        var totalUsers = await _db.Users.CountAsync(ct);
        var totalProducts = await _db.Products.CountAsync(p => !p.IsDeleted, ct);
        var totalOrders = await _db.Orders.CountAsync(o => !o.IsDeleted, ct);
        var totalRevenue = await _db.Orders.Where(o => !o.IsDeleted).SumAsync(o => (decimal?)o.Total, ct) ?? 0;
        var pendingOrders = await _db.Orders.CountAsync(o => !o.IsDeleted && o.Status == Domain.Enums.OrderStatus.Pending, ct);
        var lowStock = await _db.Products.CountAsync(p => !p.IsDeleted && p.StockQuantity <= 5, ct);
        var recentOrders = await _db.Orders.Include(o => o.Items).Where(o => !o.IsDeleted).OrderByDescending(o => o.CreatedAt).Take(5).Select(o => new { o.Id, o.OrderNumber, o.Total, o.Status, o.CreatedAt }).ToListAsync(ct);
        var topProducts = await _db.OrderItems.GroupBy(i => new { i.ProductId, i.ProductName })
            .Select(g => new { g.Key.ProductId, g.Key.ProductName, Quantity = g.Sum(x => x.Quantity), Revenue = g.Sum(x => x.Total) })
            .OrderByDescending(x => x.Quantity).Take(5).ToListAsync(ct);
        return new
        {
            totalUsers,
            totalProducts,
            totalOrders,
            totalRevenue,
            pendingOrders,
            lowStock,
            recentOrders,
            topProducts
        };
    }
}
