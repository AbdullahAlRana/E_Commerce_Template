using ECommerce.Application.Abstractions.Repositories;
using ECommerce.Domain.Common;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _set;

    public GenericRepository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _set.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.Where(e => !e.IsDeleted).ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) { entity.IsDeleted = true; entity.UpdatedAt = DateTime.UtcNow; _set.Update(entity); }

    public IQueryable<T> Query() => _set.Where(e => !e.IsDeleted).AsQueryable();
}