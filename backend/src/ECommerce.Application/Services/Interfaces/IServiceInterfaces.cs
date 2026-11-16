using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Addresses;
using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.DTOs.Brands;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.DTOs.Categories;
using ECommerce.Application.DTOs.Coupons;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.DTOs.Reviews;
using ECommerce.Application.DTOs.Users;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct = default);
}

public interface ICategoryService
{
    Task<PagedResult<CategoryDto>> GetAllAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IBrandService
{
    Task<PagedResult<BrandDto>> GetAllAsync(int page, int pageSize, string? search, CancellationToken ct = default);
    Task<BrandDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<BrandDto> CreateAsync(CreateBrandRequest request, CancellationToken ct = default);
    Task<BrandDto?> UpdateAsync(Guid id, UpdateBrandRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAllAsync(int page, int pageSize, string? search, Guid? categoryId, Guid? brandId, decimal? minPrice, decimal? maxPrice, bool? featured, string? sort, CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface ICartService
{
    Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default);
    Task<CartDto> AddItemAsync(string userId, AddToCartRequest request, CancellationToken ct = default);
    Task<CartDto> UpdateItemAsync(string userId, Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default);
    Task<CartDto> RemoveItemAsync(string userId, Guid itemId, CancellationToken ct = default);
    Task ClearAsync(string userId, CancellationToken ct = default);
}

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken ct = default);
    Task<PagedResult<OrderDto>> GetMyOrdersAsync(string userId, int page, int pageSize, CancellationToken ct = default);
    Task<OrderDto?> GetByIdAsync(Guid id, string? userId, bool isAdmin, CancellationToken ct = default);
    Task<PagedResult<OrderDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<OrderDto?> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken ct = default);
}

public interface IAddressService
{
    Task<IReadOnlyList<AddressDto>> GetByUserAsync(string userId, CancellationToken ct = default);
    Task<AddressDto> CreateAsync(string userId, CreateAddressRequest request, CancellationToken ct = default);
    Task<AddressDto?> UpdateAsync(string userId, Guid id, CreateAddressRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(string userId, Guid id, CancellationToken ct = default);
}

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetByProductAsync(Guid productId, CancellationToken ct = default);
    Task<ReviewDto> CreateAsync(string userId, string userName, CreateReviewRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IWishlistService
{
    Task<IReadOnlyList<Guid>> GetProductIdsAsync(string userId, CancellationToken ct = default);
    Task AddAsync(string userId, Guid productId, CancellationToken ct = default);
    Task RemoveAsync(string userId, Guid productId, CancellationToken ct = default);
}

public interface ICouponService
{
    Task<PagedResult<CouponDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<CouponDto> CreateAsync(CreateCouponRequest request, CancellationToken ct = default);
    Task<CouponDto?> UpdateAsync(Guid id, UpdateCouponRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<(bool Valid, string? Message, decimal Discount)> ValidateAsync(ValidateCouponRequest request, CancellationToken ct = default);
}

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<UserDto?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<bool> SetActiveAsync(string id, bool active, CancellationToken ct = default);
    Task<bool> AssignRoleAsync(string id, string role, CancellationToken ct = default);
}

public interface IDashboardService
{
    Task<object> GetStatsAsync(CancellationToken ct = default);
}