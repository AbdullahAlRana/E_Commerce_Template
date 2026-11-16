using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Coupon> Coupons => Set<Coupon>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>()
            .HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Product>()
            .HasOne(p => p.Brand).WithMany(b => b.Products).HasForeignKey(p => p.BrandId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<ProductImage>()
            .HasOne(i => i.Product).WithMany(p => p.Images).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CartItem>()
            .HasOne(i => i.Cart).WithMany(c => c.Items).HasForeignKey(i => i.CartId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CartItem>()
            .HasOne(i => i.Product).WithMany(p => p.CartItems).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<OrderItem>()
            .HasOne(i => i.Order).WithMany(o => o.Items).HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<OrderItem>()
            .HasOne(i => i.Product).WithMany(p => p.OrderItems).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Order>()
            .HasOne(o => o.ShippingAddress).WithMany().HasForeignKey(o => o.ShippingAddressId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Review>()
            .HasOne(r => r.Product).WithMany(p => p.Reviews).HasForeignKey(r => r.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<WishlistItem>()
            .HasOne(w => w.Wishlist).WithMany(wl => wl.Items).HasForeignKey(w => w.WishlistId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<WishlistItem>()
            .HasOne(w => w.Product).WithMany(p => p.WishlistItems).HasForeignKey(w => w.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Category>()
            .HasOne(c => c.ParentCategory).WithMany(c => c.SubCategories).HasForeignKey(c => c.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);

        // decimal precision
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var prop in entityType.GetProperties())
            {
                if (prop.ClrType == typeof(decimal) || prop.ClrType == typeof(decimal?))
                {
                    prop.SetPrecision(18);
                    prop.SetScale(2);
                }
            }
        }
    }
}