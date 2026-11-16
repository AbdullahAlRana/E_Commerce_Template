using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _carts;
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _db;

    public CartService(ICartRepository carts, IProductRepository products, IUnitOfWork uow, AppDbContext db)
    { _carts = carts; _products = products; _uow = uow; _db = db; }

    private async Task<Cart> GetOrCreateCartAsync(string userId, CancellationToken ct)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await _carts.AddAsync(cart, ct);
            await _uow.SaveChangesAsync(ct);
            cart = await _carts.GetByUserIdAsync(userId, ct) ?? cart;
        }
        return cart;
    }

    private static CartDto Map(Cart cart) => new()
    {
        Id = cart.Id,
        Items = cart.Items.Select(i => new CartItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductImage = i.Product.Images.FirstOrDefault(x => x.IsPrimary)?.Url ?? i.Product.Images.FirstOrDefault()?.Url,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            StockQuantity = i.Product.StockQuantity
        }).ToList()
    };

    public async Task<CartDto> GetCartAsync(string userId, CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartAsync(userId, ct);
        return Map(cart);
    }

    public async Task<CartDto> AddItemAsync(string userId, AddToCartRequest request, CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartAsync(userId, ct);
        var product = await _products.GetByIdAsync(request.ProductId, ct);
        if (product == null) throw new InvalidOperationException("Product not found.");
        if (product.StockQuantity < request.Quantity) throw new InvalidOperationException("Insufficient stock.");
        var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (existing != null)
        {
            existing.Quantity += request.Quantity;
            existing.UnitPrice = product.DiscountPrice ?? product.Price;
        }
        else
        {
            var item = new CartItem { CartId = cart.Id, ProductId = product.Id, Quantity = request.Quantity, UnitPrice = product.DiscountPrice ?? product.Price };
            cart.Items.Add(item);
            _db.Entry(item).State = EntityState.Added;
        }
        await _uow.SaveChangesAsync(ct);
        cart = await _carts.GetByUserIdAsync(userId, ct) ?? cart;
        return Map(cart);
    }

    public async Task<CartDto> UpdateItemAsync(string userId, Guid itemId, UpdateCartItemRequest request, CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartAsync(userId, ct);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) throw new InvalidOperationException("Item not found.");
        if (request.Quantity <= 0)
        {
            cart.Items.Remove(item);
        }
        else
        {
            item.Quantity = request.Quantity;
        }
        await _uow.SaveChangesAsync(ct);
        cart = await _carts.GetByUserIdAsync(userId, ct) ?? cart;
        return Map(cart);
    }

    public async Task<CartDto> RemoveItemAsync(string userId, Guid itemId, CancellationToken ct = default)
    {
        var cart = await GetOrCreateCartAsync(userId, ct);
        var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null) cart.Items.Remove(item);
        await _uow.SaveChangesAsync(ct);
        cart = await _carts.GetByUserIdAsync(userId, ct) ?? cart;
        return Map(cart);
    }

    public async Task ClearAsync(string userId, CancellationToken ct = default)
    {
        var cart = await _carts.GetByUserIdAsync(userId, ct);
        if (cart != null)
        {
            cart.Items.Clear();
            await _uow.SaveChangesAsync(ct);
        }
    }
}
