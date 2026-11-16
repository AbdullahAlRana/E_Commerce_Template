using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);
}

public interface IBrandRepository : IRepository<Brand> { }

public interface IProductRepository : IRepository<Product>
{
    Task<(IReadOnlyList<Product> Items, int Total)> SearchAsync(string? search, Guid? categoryId, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? featured, int page, int pageSize, string? sort, CancellationToken ct = default);
}

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(string userId, CancellationToken ct = default);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<(IReadOnlyList<Order> Items, int Total)> GetByUserAsync(string userId, int page, int pageSize, CancellationToken ct = default);
    Task<(IReadOnlyList<Order> Items, int Total)> GetAllAsync(int page, int pageSize, Order? filter = null, CancellationToken ct = default);
}

public interface IAddressRepository : IRepository<Address>
{
    Task<IReadOnlyList<Address>> GetByUserAsync(string userId, CancellationToken ct = default);
}

public interface IReviewRepository : IRepository<Review>
{
    Task<IReadOnlyList<Review>> GetByProductAsync(Guid productId, CancellationToken ct = default);
}

public interface IWishlistRepository : IRepository<Wishlist>
{
    Task<Wishlist?> GetByUserIdAsync(string userId, CancellationToken ct = default);
}

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default);
}