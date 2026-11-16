using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Application.DTOs.Reviews;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviews;
    private readonly IProductRepository _products;
    private readonly IUnitOfWork _uow;

    public ReviewService(IReviewRepository reviews, IProductRepository products, IUnitOfWork uow)
    { _reviews = reviews; _products = products; _uow = uow; }

    public async Task<IReadOnlyList<ReviewDto>> GetByProductAsync(Guid productId, CancellationToken ct = default)
    {
        var list = await _reviews.GetByProductAsync(productId, ct);
        return list.Select(r => new ReviewDto { Id = r.Id, ProductId = r.ProductId, UserId = r.UserId, UserName = r.UserName, Rating = r.Rating, Comment = r.Comment, CreatedAt = r.CreatedAt }).ToList();
    }

    public async Task<ReviewDto> CreateAsync(string userId, string userName, CreateReviewRequest request, CancellationToken ct = default)
    {
        var review = new Review { ProductId = request.ProductId, UserId = userId, UserName = userName, Rating = request.Rating, Comment = request.Comment };
        await _reviews.AddAsync(review, ct);
        await _uow.SaveChangesAsync(ct);
        // update product rating
        var product = await _products.GetByIdAsync(request.ProductId, ct);
        if (product != null)
        {
            var all = await _reviews.Query().Where(r => r.ProductId == request.ProductId).ToListAsync(ct);
            product.ReviewCount = all.Count;
            product.AverageRating = all.Count == 0 ? 0 : all.Average(r => r.Rating);
            _products.Update(product);
            await _uow.SaveChangesAsync(ct);
        }
        return new ReviewDto { Id = review.Id, ProductId = review.ProductId, UserId = review.UserId, UserName = review.UserName, Rating = review.Rating, Comment = review.Comment, CreatedAt = review.CreatedAt };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var r = await _reviews.GetByIdAsync(id, ct);
        if (r == null) return false;
        _reviews.Remove(r);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
