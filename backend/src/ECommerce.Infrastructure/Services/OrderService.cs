using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.Addresses;
using ECommerce.Application.DTOs.Orders;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly ICartRepository _carts;
    private readonly IProductRepository _products;
    private readonly IAddressRepository _addresses;
    private readonly ICouponRepository _coupons;
    private readonly IUnitOfWork _uow;

    public OrderService(IOrderRepository orders, ICartRepository carts, IProductRepository products, IAddressRepository addresses, ICouponRepository coupons, IUnitOfWork uow)
    { _orders = orders; _carts = carts; _products = products; _addresses = addresses; _coupons = coupons; _uow = uow; }

    private AddressDto MapAddress(Address a) => new() { Id = a.Id, FullName = a.FullName, Phone = a.Phone, Street = a.Street, City = a.City, State = a.State, ZipCode = a.ZipCode, Country = a.Country, IsDefault = a.IsDefault };

    private OrderDto Map(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        UserId = o.UserId,
        Status = o.Status,
        PaymentStatus = o.PaymentStatus,
        PaymentMethod = o.PaymentMethod,
        SubTotal = o.SubTotal,
        ShippingCost = o.ShippingCost,
        Tax = o.Tax,
        Discount = o.Discount,
        Total = o.Total,
        ShippingAddressId = o.ShippingAddressId,
        ShippingAddress = o.ShippingAddress == null ? null : MapAddress(o.ShippingAddress),
        Notes = o.Notes,
        CouponCode = o.CouponCode,
        Items = o.Items.Select(i => new OrderItemDto { Id = i.Id, ProductId = i.ProductId, ProductName = i.ProductName, ProductImage = i.ProductImage, UnitPrice = i.UnitPrice, Quantity = i.Quantity, Total = i.Total }).ToList(),
        CreatedAt = o.CreatedAt
    };

    public async Task<OrderDto> CreateOrderAsync(string userId, CreateOrderRequest request, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct);
        if (cart == null || !cart.Items.Any()) throw new InvalidOperationException("Cart is empty.");

        Address? shippingAddress = null;
        if (request.ShippingAddressId.HasValue)
        {
            shippingAddress = await _addresses.GetByIdAsync(request.ShippingAddressId.Value, ct);
        }
        else if (request.ShippingAddress != null)
        {
            var a = new Address
            {
                UserId = userId,
                FullName = request.ShippingAddress.FullName,
                Phone = request.ShippingAddress.Phone,
                Street = request.ShippingAddress.Street,
                City = request.ShippingAddress.City,
                State = request.ShippingAddress.State,
                ZipCode = request.ShippingAddress.ZipCode,
                Country = request.ShippingAddress.Country,
                IsDefault = request.ShippingAddress.IsDefault
            };
            await _addresses.AddAsync(a, ct);
            await _uow.SaveChangesAsync(ct);
            shippingAddress = a;
        }

        // stock check
        foreach (var item in cart.Items)
        {
            var product = await _products.GetByIdAsync(item.ProductId, ct);
            if (product == null) throw new InvalidOperationException("Product not found.");
            if (product.StockQuantity < item.Quantity) throw new InvalidOperationException($"Insufficient stock for {product.Name}.");
        }

        var subTotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
        decimal discount = 0;
        string? couponCode = null;
        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var coupon = await _coupons.GetByCodeAsync(request.CouponCode, ct);
            if (coupon != null && coupon.IsActive && coupon.StartDate <= DateTime.UtcNow && coupon.EndDate >= DateTime.UtcNow)
            {
                if (!coupon.MinOrderAmount.HasValue || subTotal >= coupon.MinOrderAmount.Value)
                {
                    if (coupon.Type == CouponType.Percentage)
                        discount = subTotal * (coupon.Value / 100m);
                    else
                        discount = coupon.Value;
                    if (coupon.MaxDiscount.HasValue) discount = Math.Min(discount, coupon.MaxDiscount.Value);
                    coupon.UsedCount++;
                    _coupons.Update(coupon);
                    couponCode = coupon.Code;
                }
            }
        }
        var shippingCost = subTotal > 100 ? 0 : 10;
        var tax = (subTotal - discount) * 0.1m; // 10% tax
        var total = subTotal - discount + shippingCost + tax;

        var order = new Order
        {
            UserId = userId,
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            PaymentMethod = request.PaymentMethod,
            SubTotal = subTotal,
            ShippingCost = shippingCost,
            Tax = tax,
            Discount = discount,
            Total = total,
            ShippingAddressId = shippingAddress?.Id,
            ShippingAddress = shippingAddress,
            Notes = request.Notes,
            CouponCode = couponCode
        };

        foreach (var item in cart.Items)
        {
            var product = await _products.GetByIdAsync(item.ProductId, ct);
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductImage = item.Product.Images.FirstOrDefault(x => x.IsPrimary)?.Url ?? item.Product.Images.FirstOrDefault()?.Url,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                Total = item.UnitPrice * item.Quantity
            });
            // decrement stock
            if (product != null)
            {
                product.StockQuantity -= item.Quantity;
                _products.Update(product);
            }
        }

        await _orders.AddAsync(order, ct);
        // clear cart
        cart.Items.Clear();
        await _uow.SaveChangesAsync(ct);

        var created = await _orders.GetByIdAsync(order.Id, ct);
        return Map(created ?? order);
    }

    public async Task<PagedResult<OrderDto>> GetMyOrdersAsync(string userId, int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await _orders.GetByUserAsync(userId, page, pageSize, ct);
        return new PagedResult<OrderDto> { Items = items.Select(Map).ToList(), Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, string? userId, bool isAdmin, CancellationToken ct = default)
    {
        var o = await _orders.Query().Include(x => x.Items).Include(x => x.ShippingAddress).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (o == null) return null;
        if (!isAdmin && o.UserId != userId) return null;
        return Map(o);
    }

    public async Task<PagedResult<OrderDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var (items, total) = await _orders.GetAllAsync(page, pageSize, null, ct);
        return new PagedResult<OrderDto> { Items = items.Select(Map).ToList(), Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<OrderDto?> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken ct = default)
    {
        var o = await _orders.GetByIdAsync(id, ct);
        if (o == null) return null;
        o.Status = request.Status;
        if (request.PaymentStatus.HasValue) o.PaymentStatus = request.PaymentStatus.Value;
        o.UpdatedAt = DateTime.UtcNow;
        _orders.Update(o);
        await _uow.SaveChangesAsync(ct);
        return await GetByIdAsync(id, null, true, ct);
    }
}
